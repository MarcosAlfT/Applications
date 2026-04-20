using Pagarte.Worker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pagarte.Worker.Interfaces
{
	public interface IServiceRepository
	{
		Task<IEnumerable<Service>> GetAllActiveAsync(string? category = null);
		Task<Service?> GetByIdAsync(Guid id);
	}
}
