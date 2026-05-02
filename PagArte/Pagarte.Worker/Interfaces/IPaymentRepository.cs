<<<<<<< HEAD
using Pagarte.Worker.Domain.Entities;
=======
﻿using Pagarte.Worker.Domain.Entities;
>>>>>>> origin/main
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pagarte.Worker.Interfaces
{
	public interface IPaymentRepository
	{
		Task<Payment?> GetByIdAsync(Guid id);
		Task<IEnumerable<Payment>> GetByClientIdAsync(string clientId, int page, int pageSize);
		Task<int> GetCountByClientIdAsync(string clientId);
		Task<Payment> CreateAsync(Payment payment);
		Task UpdateAsync(Payment payment);
		Task<IEnumerable<Payment>> GetPendingRefundsAsync();
	}
}
