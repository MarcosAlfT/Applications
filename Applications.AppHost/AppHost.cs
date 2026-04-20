var builder = DistributedApplication.CreateBuilder(args);

// RabbitMQ
var rabbitUser = builder.AddParameter("rabbit-user", secret: false);
var rabbitPassword = builder.AddParameter("rabbit-pass", secret: true);
var rabbitmq = builder.AddRabbitMQ("PagQueue", rabbitUser, rabbitPassword)
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
