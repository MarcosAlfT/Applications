using ClientIdentity.Application.Abstractions;
using ClientIdentity.Infrastructure.Notifications;
using ClientIdentity.Infrastructure.Policies;
using ClientIdentity.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace ClientIdentity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddClientIdentityInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ICurrentActorProvider, CurrentActorProvider>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenGenerator, SecureTokenGenerator>();
        services.AddSingleton<ITokenHasher, Sha256TokenHasher>();
        services.AddSingleton<IPolicyProvider, ConfigurationPolicyProvider>();
        services.AddSingleton<INotificationPublisher, ConsoleNotificationPublisher>();
        services.AddScoped<ITokenService, OpenIddictTokenService>();

        return services;
    }
}
