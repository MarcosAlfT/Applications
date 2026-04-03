var builder = DistributedApplication.CreateBuilder(args);

// RabbitMQ
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
	.WithManagementPlugin();  // enables RabbitMQ management UI

// Identity Service
var identity = builder.AddProject<Projects.IdentityService>("identity");

// Clients API
var clientsApi = builder.AddProject<Projects.Clients_API>("clients")
	.WithReference(identity);

// Pagarte API
var pagarteApi = builder.AddProject<Projects.Pagarte_API>("pagarte-api")
	.WithReference(identity)
	.WithReference(rabbitmq);

// Pagarte Worker
builder.AddProject<Projects.Pagarte_Worker>("pagarte-worker")
	.WithReference(rabbitmq)
	.WithReference(pagarteApi);  // for SignalR hub URL



builder.Build().Run();
