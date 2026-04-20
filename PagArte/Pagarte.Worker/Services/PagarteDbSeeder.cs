using Microsoft.EntityFrameworkCore;
using Pagarte.Worker.Domain.Entities;
using Pagarte.Worker.Domain.Enums;
using Pagarte.Worker.Infrastructure;
using Pagarte.Worker.Infrastructure.Repository;

namespace Pagarte.Worker.Services
{
	public static class PagarteDbSeeder
	{
		public static async Task SeedAsync(PagarteDbContext context)
		{
			await SeedCompaniesAsync(context);
			await SeedServicesAsync(context);
			await SeedFeeConfigurationsAsync(context);
		}

		private static async Task SeedCompaniesAsync(PagarteDbContext context)
		{
			if (await context.Companies.IgnoreQueryFilters().AnyAsync()) return;

			context.Companies.AddRange(
				new Company { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "City Water Company", ApiEndpoint = "https://api.citywater.com/payments", ApiKey = "water-key-encrypted", IsActive = true },
				new Company { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "National Electric", ApiEndpoint = "https://api.nationalelectric.com/payments", ApiKey = "electric-key-encrypted", IsActive = true },
				new Company { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Internet Provider Co", ApiEndpoint = "https://api.internet.com/payments", ApiKey = "internet-key-encrypted", IsActive = true },
				new Company { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Gas Utility Corp", ApiEndpoint = "https://api.gasutility.com/payments", ApiKey = "gas-key-encrypted", IsActive = true }
			);
			await context.SaveChangesAsync();
		}

		private static async Task SeedServicesAsync(PagarteDbContext context)
		{
			if (await context.Services.IgnoreQueryFilters().AnyAsync()) return;

			context.Services.AddRange(
				new Service { Id = Guid.NewGuid(), Name = "Water Bill", Description = "Monthly water service", Category = "Utilities", CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"), BaseAmount = 0, Currency = "USD", IsActive = true },
				new Service { Id = Guid.NewGuid(), Name = "Electricity Bill", Description = "Monthly electricity service", Category = "Utilities", CompanyId = Guid.Parse("22222222-2222-2222-2222-222222222222"), BaseAmount = 0, Currency = "USD", IsActive = true },
				new Service { Id = Guid.NewGuid(), Name = "Internet Basic", Description = "100Mbps plan", Category = "Telecom", CompanyId = Guid.Parse("33333333-3333-3333-3333-333333333333"), BaseAmount = 49.99m, Currency = "USD", IsActive = true },
				new Service { Id = Guid.NewGuid(), Name = "Internet Premium", Description = "500Mbps plan", Category = "Telecom", CompanyId = Guid.Parse("33333333-3333-3333-3333-333333333333"), BaseAmount = 89.99m, Currency = "USD", IsActive = true },
				new Service { Id = Guid.NewGuid(), Name = "Gas Bill", Description = "Monthly gas service", Category = "Utilities", CompanyId = Guid.Parse("44444444-4444-4444-4444-444444444444"), BaseAmount = 0, Currency = "USD", IsActive = true }
			);
			await context.SaveChangesAsync();
		}

		private static async Task SeedFeeConfigurationsAsync(PagarteDbContext context)
		{
			if (await context.FeeConfigurations.IgnoreQueryFilters().AnyAsync()) return;

			context.FeeConfigurations.AddRange(
				new FeeConfiguration { Id = Guid.NewGuid(), Type = FeeType.DLocal, CalculationType = CalculationType.Percentage, Value = 2.9m, Currency = "USD", IsActive = true, EffectiveDate = DateTime.UtcNow.AddYears(-1) },
				new FeeConfiguration { Id = Guid.NewGuid(), Type = FeeType.Pagarte, CalculationType = CalculationType.Percentage, Value = 1.5m, Currency = "USD", IsActive = true, EffectiveDate = DateTime.UtcNow.AddYears(-1) },
				new FeeConfiguration { Id = Guid.NewGuid(), Type = FeeType.Company, CalculationType = CalculationType.FixedAmount, Value = 0.30m, Currency = "USD", IsActive = true, EffectiveDate = DateTime.UtcNow.AddYears(-1) }
			);
			await context.SaveChangesAsync();
		}
	}
}
