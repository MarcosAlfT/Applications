using Microsoft.EntityFrameworkCore;
using Pagarte.API.Domain.Entities;

namespace Pagarte.API.Infrastructure.Repository
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

            var companies = new List<Company>
            {
                new() {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "City Water Company",
                    ApiEndpoint = "https://api.citywater.com/payments",
                    ApiKey = "water-api-key-encrypted",
                    IsActive = true
                },
                new() {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "National Electric",
                    ApiEndpoint = "https://api.nationalelectric.com/payments",
                    ApiKey = "electric-api-key-encrypted",
                    IsActive = true
                },
                new() {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Internet Provider Co",
                    ApiEndpoint = "https://api.internetprovider.com/payments",
                    ApiKey = "internet-api-key-encrypted",
                    IsActive = true
                },
                new() {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Gas Utility Corp",
                    ApiEndpoint = "https://api.gasutility.com/payments",
                    ApiKey = "gas-api-key-encrypted",
                    IsActive = true
                }
            };

            context.Companies.AddRange(companies);
            await context.SaveChangesAsync();
        }

        private static async Task SeedServicesAsync(PagarteDbContext context)
        {
            if (await context.Services.IgnoreQueryFilters().AnyAsync()) return;

            var services = new List<Service>
            {
                // Water services
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Water Bill",
                    Description = "Monthly water service payment",
                    Category = "Utilities",
                    CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    BaseAmount = 0,   // variable amount
                    Currency = "USD",
                    IsActive = true
                },
                // Electricity services
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Electricity Bill",
                    Description = "Monthly electricity service payment",
                    Category = "Utilities",
                    CompanyId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    BaseAmount = 0,
                    Currency = "USD",
                    IsActive = true
                },
                // Internet services
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Internet Basic",
                    Description = "Basic internet plan - 100Mbps",
                    Category = "Telecom",
                    CompanyId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    BaseAmount = 49.99m,
                    Currency = "USD",
                    IsActive = true
                },
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Internet Premium",
                    Description = "Premium internet plan - 500Mbps",
                    Category = "Telecom",
                    CompanyId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    BaseAmount = 89.99m,
                    Currency = "USD",
                    IsActive = true
                },
                // Gas services
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Gas Bill",
                    Description = "Monthly gas service payment",
                    Category = "Utilities",
                    CompanyId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    BaseAmount = 0,
                    Currency = "USD",
                    IsActive = true
                }
            };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();
        }

        private static async Task SeedFeeConfigurationsAsync(PagarteDbContext context)
        {
            if (await context.FeeConfigurations.IgnoreQueryFilters().AnyAsync()) return;

            var fees = new List<FeeConfiguration>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Type = Domain.Enums.FeeType.DLocal,
                    CalculationType = Domain.Enums.CalculationType.Percentage,
                    Value = 2.9m,     // 2.9%
                    Currency = "USD",
                    IsActive = true,
                    EffectiveDate = DateTime.UtcNow.AddYears(-1)
                },
                new() {
                    Id = Guid.NewGuid(),
                    Type = Domain.Enums.FeeType.Pagarte,
                    CalculationType = Domain.Enums.CalculationType.Percentage,
                    Value = 1.5m,     // 1.5%
                    Currency = "USD",
                    IsActive = true,
                    EffectiveDate = DateTime.UtcNow.AddYears(-1)
                },
                new() {
                    Id = Guid.NewGuid(),
                    Type = Domain.Enums.FeeType.Company,
                    CalculationType = Domain.Enums.CalculationType.FixedAmount,
                    Value = 0.30m,    // $0.30 flat fee
                    Currency = "USD",
                    IsActive = true,
                    EffectiveDate = DateTime.UtcNow.AddYears(-1)
                }
            };

            context.FeeConfigurations.AddRange(fees);
            await context.SaveChangesAsync();
        }
    }
}
