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







// Identity Service
var identity = builder.AddProject<Projects.IdentityService>("identity", launchProfileName: "https");

// Clients API
var clientsApi = builder.AddProject<Projects.Clients_API>("clients", launchProfileName: "https")
	.WithReference(identity)
	.WithEnvironment("AuthSettings__Authority", identity.GetEndpoint("https"));

// Pagarte Worker
var pagarteWorker = builder.AddProject<Projects.Pagarte_Worker>("pagarte-worker", launchProfileName: "Pagarte.Worker")
	.WithReference(rabbitmq);

// Pagarte API
var pagarteApi = builder.AddProject<Projects.Pagarte_API>("pagarte-api")
	.WithReference(identity)
	.WithReference(pagarteWorker)
	.WithEnvironment("AuthSettings__Authority", identity.GetEndpoint("https"))
	.WithEnvironment("PagarteWorker__GrpcUrl", pagarteWorker.GetEndpoint("https"));

// Pagarte Engine
builder.AddProject<Projects.Pagarte_Engine>("pagarte-engine")
	.WithReference(rabbitmq);


builder.Build().Run();
