using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.RabbitMQ;

public static class RabbitMqServiceCollectionExtensions
{
	public static IServiceCollection AddRabbitMq(
		this IServiceCollection services,
		IConfiguration config)
	{
		services.Configure<RabbitMQConnentionOptions>(
			config.GetSection("RabbitMQ"));

		services.AddSingleton<RabbitMQConnectionFactory>(sp =>
		{
			var options = sp.GetRequiredService<IOptions<RabbitMQConnentionOptions>>().Value;

			// Aspire case
			if (options.Mode == RabbitMQConnentionMode.FromEnvironment)
			{
				options.ConnectionString = config.GetConnectionString("PagQueue");
			}

			return new RabbitMQConnectionFactory(options);
		});

		return services;
	}
}