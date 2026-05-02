<<<<<<< HEAD
using Pagarte.Worker.Domain.Entities;
=======
﻿using Pagarte.Worker.Domain.Entities;
>>>>>>> origin/main

namespace Pagarte.Worker.Interfaces
{
	public interface ICreditCardRepository
	{
		Task<IEnumerable<CreditCard>> GetByClientIdAsync(string clientId);
		Task<CreditCard?> GetByIdAsync(Guid id);
		Task<CreditCard> CreateAsync(CreditCard card);
		Task UpdateAsync(CreditCard card);
		Task DeleteAsync(Guid id, string clientId);
	}
}
