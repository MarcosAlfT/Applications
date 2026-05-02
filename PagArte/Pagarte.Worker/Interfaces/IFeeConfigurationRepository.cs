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
	public interface IFeeConfigurationRepository
	{
		Task<IEnumerable<FeeConfiguration>> GetActiveFeesAsync();
	}
}
