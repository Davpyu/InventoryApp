using Microsoft.Data.SqlClient;

namespace DotNetService.Infrastructure.Queues
{
    public class ConnectionPoolCheckerService(ILogger<ConnectionPoolCheckerService> logger, string connectionString, int interval) : BackgroundService
    {
        private readonly ILogger<ConnectionPoolCheckerService> _logger = logger;
        private readonly string _connectionString = connectionString;

        private readonly int _interval = interval;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Connection Pool Checker Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckConnectionPoolSize();
                    await Task.Delay(TimeSpan.FromSeconds(_interval), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking connection pool size.");
                }
            }

            _logger.LogInformation("Connection Pool Checker Service is stopping.");
        }

        private async Task CheckConnectionPoolSize()
        {
            var query = @"
                SELECT COUNT(connection_id) AS active_connections,
                       net_transport,
                       auth_scheme
                FROM sys.dm_exec_connections
                GROUP BY net_transport, auth_scheme;";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // date now
                var now = DateTime.Now;
                int activeConnections = reader.GetInt32(0);
                string netTransport = reader.GetString(1);
                string authScheme = reader.GetString(2);

                _logger.LogInformation(
                    "{Now} - Active Connections: {ActiveConnections}, Net Transport: {NetTransport}, Auth Scheme: {AuthScheme}",
                    now, activeConnections, netTransport, authScheme);
            }
        }
    }
}
