using ClientIdentity.Application.Abstractions;
using ClientIdentity.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientIdentity.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddClientIdentityPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClientIdentityDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ClientIdentityDb"));
            options.UseOpenIddict();
        });

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ClientIdentityDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IEmailConfirmationTokenRepository, EmailConfirmationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        return services;
    }
}
