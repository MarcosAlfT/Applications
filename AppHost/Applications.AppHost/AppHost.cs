var builder = DistributedApplication.CreateBuilder(args);

// RabbitMQ
var rabbitUser = builder.AddParameter("rabbit-user", secret: false);
var rabbitPassword = builder.AddParameter("rabbit-pass", secret: true);
var rabbitmq = builder.AddRabbitMQ("PagQueue", rabbitUser, rabbitPassword)
	.WithManagementPlugin()		// enables RabbitMQ management UI
	.WithEndpoint("amqp", e =>
	{
		e.Port = 5672;
		e.TargetPort = 5672;
	});




// Client Identity Service
var identity = builder.AddProject<Projects.ClientIdentity_Api>("client-identity", launchProfileName: "https");

// Clients API
var clientsApi = builder.AddProject<Projects.Clients_API>("clients", launchProfileName: "https")
	.WithReference(identity)
	.WithEnvironment("AuthSettings__Authority", identity.GetEndpoint("https"));

// Pagarte Services
var pagarteServices = builder.AddProject<Projects.Pagarte_Services>("pagarte-services", launchProfileName: "Pagarte.Services")
	.WithReference(rabbitmq);

// Pagarte API
var pagarteApi = builder.AddProject<Projects.Pagarte_API>("pagarte-api")
	.WithReference(identity)
	.WithReference(pagarteServices)
	.WithEnvironment("AuthSettings__Authority", identity.GetEndpoint("https"))
	.WithEnvironment("PagarteServices__GrpcUrl", pagarteServices.GetEndpoint("https"));

// Pagarte Engine
builder.AddProject<Projects.Pagarte_Engine>("pagarte-engine")
	.WithReference(rabbitmq);


builder.Build().Run();
