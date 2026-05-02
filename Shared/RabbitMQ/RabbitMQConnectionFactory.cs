using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    /// <summary>
    /// Manages a single persistent RabbitMQ connection with auto-recovery.
    /// Register as Singleton.
    /// </summary>
    public class RabbitMQConnectionFactory : IDisposable
    {
<<<<<<< HEAD
		private readonly RabbitMQConnentionOptions _connectionOptions;

        private IConnection? _connection;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public RabbitMQConnectionFactory(RabbitMQConnentionOptions connOptions)
        {
            _connectionOptions = connOptions;
=======
		private readonly string _connectionString;

		//private readonly string _host;
  //      private readonly int _port;
  //      private readonly string _username;
  //      private readonly string _password;
        private IConnection? _connection;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public RabbitMQConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
>>>>>>> origin/main
		}

        public async Task<IConnection> GetConnectionAsync()
        {
            await _semaphore.WaitAsync();
			try
            {
                if (_connection == null || !_connection.IsOpen)
                {
<<<<<<< HEAD

					var connectionString = BuildConnectionString();

                    Console.WriteLine($"RabbitMQ connection string: {connectionString}");

					var factory = new ConnectionFactory
					{
						Uri = new Uri(connectionString),
=======
					var factory = new ConnectionFactory
					{
						Uri = new Uri(_connectionString),
>>>>>>> origin/main
						AutomaticRecoveryEnabled = true,
						NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
					};
					_connection = await factory.CreateConnectionAsync();
                }
                return _connection;
            }
            finally
            {
                _semaphore.Release();
			}
		}

<<<<<<< HEAD
		private string BuildConnectionString()
		{
            var hostName = _connectionOptions.HostName;
            var userName = _connectionOptions.UserName;

			return _connectionOptions.Mode switch
			{
				RabbitMQConnentionMode.FromEnvironment =>
					_connectionOptions.ConnectionString
					?? throw new Exception("ConnectionString is null"),

				RabbitMQConnentionMode.FromConfiguration =>
					$"amqp://{userName}:{_connectionOptions.Password}@{hostName}:{_connectionOptions.Port}/",

				_ => throw new Exception("Unsupported RabbitMQ mode")
			};
		}

		public void Dispose()
=======
        public void Dispose()
>>>>>>> origin/main
        {
            _connection?.CloseAsync().GetAwaiter().GetResult();
            _connection?.Dispose();
        }
    }

<<<<<<< HEAD
=======

	public class RabbitMQConnectionFactoryNoAspire : IDisposable
	{
		private readonly string _host;
		private readonly int _port;
		private readonly string _username;
		private readonly string _password;
		private IConnection? _connection;
		private readonly SemaphoreSlim _semaphore = new(1, 1);

		public RabbitMQConnectionFactoryNoAspire(string host, int port, string username, string password)
		{
			_host = host;
			_port = port;
			_username = username;
			_password = password;
		}

		public async Task<IConnection> GetConnectionAsync()
		{
			await _semaphore.WaitAsync();
			try
			{
				if (_connection == null || !_connection.IsOpen)
				{
					var factory = new ConnectionFactory
					{
						HostName = _host,
						Port = _port,
						UserName = _username,
						Password = _password,
						AutomaticRecoveryEnabled = true,
						NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
					};
					_connection = await factory.CreateConnectionAsync();
				}
				return _connection;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public void Dispose()
		{
			_connection?.CloseAsync().GetAwaiter().GetResult();
			_connection?.Dispose();
		}
	}
>>>>>>> origin/main
}
