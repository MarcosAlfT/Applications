using Pagarte.Services.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pagarte.Services.Interfaces
{
	public interface IFeeConfigurationRepository
	{
		Task<IEnumerable<FeeConfiguration>> GetActiveFeesAsync();
	}
}
