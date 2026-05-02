<<<<<<< HEAD
namespace Pagarte.Worker.Domain.Entities
=======
﻿namespace Pagarte.Worker.Domain.Entities
>>>>>>> origin/main
{
	public class Company
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string ApiEndpoint { get; set; } = string.Empty;
		public string ApiKey { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
		public ICollection<Service> Services { get; set; } = [];
	}
}
