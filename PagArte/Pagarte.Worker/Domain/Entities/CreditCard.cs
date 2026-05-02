using Pagarte.Worker.Domain.Enums;

namespace Pagarte.Worker.Domain.Entities
{
	public class CreditCard
	{
		public Guid Id { get; set; }
		public string ClientId { get; set; } = string.Empty;
<<<<<<< HEAD
		public string OperatorCardToken { get; set; } = string.Empty;
		public string CardNumber { get; set; } = string.Empty;
=======
		public string DLocalCardToken { get; set; } = string.Empty;
>>>>>>> origin/main
		public string CardHolderName { get; set; } = string.Empty;
		public string Last4Digits { get; set; } = string.Empty;
		public CardType CardType { get; set; }
		public int ExpiryMonth { get; set; }
		public int ExpiryYear { get; set; }
		public bool IsDefault { get; set; } = false;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
		public bool IsDeleted { get; set; } = false;
		public DateTime? DeletedAt { get; set; }

		public ICollection<Payment> Payments { get; set; } = [];

<<<<<<< HEAD
		public static CreditCard Create(string clientId, string operatorCardToken,
			string cardNumber, string cardHolderName,
			string last4Digits, CardType cardType, int expiryMonth,
			int expiryYear, bool isDefault)
=======
		public static CreditCard Create(string clientId, string dLocalCardToken,
			string cardHolderName, string last4Digits, CardType cardType,
			int expiryMonth, int expiryYear, bool isDefault)
>>>>>>> origin/main
		{
			return new CreditCard
			{
				Id = Guid.NewGuid(),
				ClientId = clientId,
<<<<<<< HEAD
				OperatorCardToken = operatorCardToken,
				CardNumber = cardNumber,
=======
				DLocalCardToken = dLocalCardToken,
>>>>>>> origin/main
				CardHolderName = cardHolderName,
				Last4Digits = last4Digits,
				CardType = cardType,
				ExpiryMonth = expiryMonth,
				ExpiryYear = expiryYear,
				IsDefault = isDefault,
				CreatedAt = DateTime.UtcNow
			};
		}

		public void Update(string cardHolderName, bool isDefault)
		{
			CardHolderName = cardHolderName;
			IsDefault = isDefault;
			UpdatedAt = DateTime.UtcNow;
		}

		public void Delete()
		{
			IsDeleted = true;
			DeletedAt = DateTime.UtcNow;
		}
	}
}
