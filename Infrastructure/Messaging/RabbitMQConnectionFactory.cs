using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class RabbitMQConnectionFactory
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private IConnection? _connection;
        private readonly object _lock = new();

        public RabbitMQConnectionFactory(string host, int port, string username, string password)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
        }

        public IConnection GetConnection()
        {
            lock (_lock)
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
                    _connection = factory.CreateConnection();
                }
                return _connection;
            }
        }
    }
}
