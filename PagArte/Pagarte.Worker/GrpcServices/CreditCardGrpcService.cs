using Pagarte.Contracts;
using Pagarte.Worker.Domain.Enums;
using Pagarte.Worker.Interfaces;
using Pagarte.Connections.DLocal;
using Grpc.Core;

namespace Pagarte.Worker.GrpcServices
{
	public class CreditCardGrpcService(
		ICreditCardRepository creditCardRepository,
		IDLocalAdapter dLocalAdapter,
		ILogger<CreditCardGrpcService> logger)
		: Pagarte.Contracts.CreditCardService.CreditCardServiceBase
	{
		private readonly ICreditCardRepository _creditCardRepository = creditCardRepository;
		private readonly IDLocalAdapter _dLocalAdapter = dLocalAdapter;
		private readonly ILogger<CreditCardGrpcService> _logger = logger;

		public override async Task<GetCardsResponse> GetCards(
			GetCardsRequest request, ServerCallContext context)
		{
			var cards = await _creditCardRepository.GetByClientIdAsync(request.ClientId);
			var response = new GetCardsResponse();
			response.Cards.AddRange(cards.Select(MapCard));
			return response;
		}

		public override async Task<GetCardResponse> GetCard(
			GetCardRequest request, ServerCallContext context)
		{
			var card = await _creditCardRepository.GetByIdAsync(Guid.Parse(request.CardId));
			if (card == null || card.ClientId != request.ClientId)
				return new GetCardResponse { Found = false };

			return new GetCardResponse { Found = true, Card = MapCard(card) };
		}

		public override async Task<RegisterCardResponse> RegisterCard(
			RegisterCardRequest request, ServerCallContext context)
		{
			_logger.LogInformation("Registering card for client {ClientId}", request.ClientId);

			// Call dLocal with encrypted data — they decrypt, we get back a token
			var result = await _dLocalAdapter.RegisterCardAsync(
				request.EncryptedCardData, request.CardHolderName);

			if (!result.Success)
				return new RegisterCardResponse
				{
					Success = false,
					ErrorMessage = result.ErrorMessage ?? "Card registration failed."
				};

			// Remove existing default if needed
			if (request.IsDefault)
			{
				var existing = await _creditCardRepository.GetByClientIdAsync(request.ClientId);
				foreach (var card in existing.Where(c => c.IsDefault))
				{
					card.Update(card.CardHolderName, false);
					await _creditCardRepository.UpdateAsync(card);
				}
			}

			// Save token — never store plain card data
			var newCard = Domain.Entities.CreditCard.Create(
				request.ClientId,
				result.CardToken!,
				request.CardHolderName,
				result.Last4Digits!,
				Enum.Parse<CardType>(result.CardType!),
				result.ExpiryMonth,
				result.ExpiryYear,
				request.IsDefault);

			await _creditCardRepository.CreateAsync(newCard);

			_logger.LogInformation("Card registered for client {ClientId} ending in {Last4}",
				request.ClientId, result.Last4Digits);

			return new RegisterCardResponse
			{
				Success = true,
				CardId = newCard.Id.ToString(),
				Last4Digits = result.Last4Digits,
				CardType = result.CardType
			};
		}

		public override async Task<MutationResponse> UpdateCard(
			UpdateCardRequest request, ServerCallContext context)
		{
			var card = await _creditCardRepository.GetByIdAsync(Guid.Parse(request.CardId));
			if (card == null || card.ClientId != request.ClientId)
				return new MutationResponse
				{
					Success = false,
					ErrorMessage = "Card not found."
				};

			if (request.IsDefault)
			{
				var existing = await _creditCardRepository.GetByClientIdAsync(request.ClientId);
				foreach (var c in existing.Where(c => c.IsDefault && c.Id != card.Id))
				{
					c.Update(c.CardHolderName, false);
					await _creditCardRepository.UpdateAsync(c);
				}
			}

			card.Update(request.CardHolderName, request.IsDefault);
			await _creditCardRepository.UpdateAsync(card);

			return new MutationResponse { Success = true };
		}

		public override async Task<MutationResponse> DeleteCard(
			DeleteCardRequest request, ServerCallContext context)
		{
			await _creditCardRepository.DeleteAsync(
				Guid.Parse(request.CardId), request.ClientId);
			return new MutationResponse { Success = true };
		}

		private static CreditCardDto MapCard(Domain.Entities.CreditCard card) =>
			new()
			{
				Id = card.Id.ToString(),
				CardHolderName = card.CardHolderName,
				Last4Digits = card.Last4Digits,
				CardType = card.CardType.ToString(),
				ExpiryMonth = card.ExpiryMonth,
				ExpiryYear = card.ExpiryYear,
				IsDefault = card.IsDefault,
				CreatedAt = card.CreatedAt.ToString("O")
			};
	}
}
