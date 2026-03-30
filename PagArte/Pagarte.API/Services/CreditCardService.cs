using Mapster;
using Pagarte.API.Constants;
using Pagarte.API.Domain.Entities;
using Pagarte.API.Domain.Enums;
using Pagarte.API.DTOs.Requests;
using Pagarte.API.DTOs.Responses;
using Pagarte.API.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Messages.Email;
using Shared.Responses;

namespace Pagarte.API.Services
{
    public class CreditCardService(
        ICreditCardRepository creditCardRepository,
        IDLocalService dLocalService,
        IMessagePublisher messagePublisher) : ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository = creditCardRepository;
        private readonly IDLocalService _dLocalService = dLocalService;
        private readonly IMessagePublisher _messagePublisher = messagePublisher;

        public async Task<ApiResponse<IEnumerable<CreditCardResponse>>> GetByClientIdAsync(string clientId)
        {
            try
            {
                var cards = await _creditCardRepository.GetByClientIdAsync(clientId);
                return ApiResponse<IEnumerable<CreditCardResponse>>.CreateSuccess(
                    cards.Adapt<IEnumerable<CreditCardResponse>>());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CreditCardResponse>>.CreateFailure(
                    $"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CreditCardResponse>> RegisterAsync(string clientId, RegisterCreditCardRequest request)
        {
            try
            {
                // Register card with dLocal
                var dLocalResult = await _dLocalService.RegisterCardAsync(
                    request.CardNumber, request.CardHolderName,
                    request.ExpiryMonth, request.ExpiryYear, request.Cvv);

                if (!dLocalResult.Success)
                    return ApiResponse<CreditCardResponse>.CreateFailure(Messages.CreditCard.DLocalError);

                // If setting as default, remove existing default
                if (request.IsDefault)
                {
                    var existingCards = await _creditCardRepository.GetByClientIdAsync(clientId);
                    foreach (var existingCard in existingCards.Where(c => c.IsDefault))
                    {
                        existingCard.Update(existingCard.CardHolderName, false);
                        await _creditCardRepository.UpdateAsync(existingCard);
                    }
                }

                var card = CreditCard.Create(
                    clientId,
                    dLocalResult.CardToken!,
                    request.CardHolderName,
                    dLocalResult.Last4Digits!,
                    Enum.Parse<CardType>(dLocalResult.CardType!),
                    int.Parse(request.ExpiryMonth),
                    int.Parse(request.ExpiryYear),
                    request.IsDefault);

                await _creditCardRepository.CreateAsync(card);

                // Send confirmation email async via RabbitMQ
                await _messagePublisher.PublishAsync(new EmailMessage
                {
                    To = clientId, // will be resolved to email by worker
                    Subject = "Credit card registered successfully",
                    Body = $"Your card ending in {dLocalResult.Last4Digits} has been registered."
                });

                return ApiResponse<CreditCardResponse>.CreateSuccess(
                    card.Adapt<CreditCardResponse>(), Messages.CreditCard.Created);
            }
            catch (Exception ex)
            {
                return ApiResponse<CreditCardResponse>.CreateFailure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(string clientId, Guid cardId, UpdateCreditCardRequest request)
        {
            try
            {
                var card = await _creditCardRepository.GetByIdAsync(cardId);
                if (card == null || card.ClientId != clientId)
                    return ApiResponse.CreateFailure(Messages.CreditCard.NotFound);

                if (request.IsDefault)
                {
                    var existingCards = await _creditCardRepository.GetByClientIdAsync(clientId);
                    foreach (var existingCard in existingCards.Where(c => c.IsDefault && c.Id != cardId))
                    {
                        existingCard.Update(existingCard.CardHolderName, false);
                        await _creditCardRepository.UpdateAsync(existingCard);
                    }
                }

                card.Update(request.CardHolderName, request.IsDefault);
                await _creditCardRepository.UpdateAsync(card);
                return ApiResponse.CreateSuccess(Messages.CreditCard.Updated);
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(string clientId, Guid cardId)
        {
            try
            {
                var card = await _creditCardRepository.GetByIdAsync(cardId);
                if (card == null || card.ClientId != clientId)
                    return ApiResponse.CreateFailure(Messages.CreditCard.NotFound);

                await _creditCardRepository.DeleteAsync(cardId, clientId);
                return ApiResponse.CreateSuccess(Messages.CreditCard.Deleted);
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailure($"An error occurred: {ex.Message}");
            }
        }
    }
}
