using Pagarte.Contracts;

namespace Pagarte.API.GrpcClients
{
	public class CreditCardGrpcClient(
			CreditCardService.CreditCardServiceClient client)
	{
		private readonly CreditCardService.CreditCardServiceClient _client = client;

		public async Task<GetCardsResponse> GetCardsAsync(string clientId)
			=> await _client.GetCardsAsync(new GetCardsRequest { ClientId = clientId });

		public async Task<GetCardResponse> GetCardAsync(string cardId, string clientId)
			=> await _client.GetCardAsync(
				new GetCardRequest { CardId = cardId, ClientId = clientId });

		public async Task<RegisterCardResponse> RegisterCardAsync(
			string clientId, string encryptedCardData,
			string cardHolderName, bool isDefault)
			=> await _client.RegisterCardAsync(new RegisterCardRequest
			{
				ClientId = clientId,
				EncryptedCardData = encryptedCardData,
				CardHolderName = cardHolderName,
				IsDefault = isDefault
			});

		public async Task<MutationResponse> UpdateCardAsync(
			string cardId, string clientId,
			string cardHolderName, bool isDefault)
			=> await _client.UpdateCardAsync(new UpdateCardRequest
			{
				CardId = cardId,
				ClientId = clientId,
				CardHolderName = cardHolderName,
				IsDefault = isDefault
			});

		public async Task<MutationResponse> DeleteCardAsync(
			string cardId, string clientId)
			=> await _client.DeleteCardAsync(
				new DeleteCardRequest { CardId = cardId, ClientId = clientId });
	}
}
