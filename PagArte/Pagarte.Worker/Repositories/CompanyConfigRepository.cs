using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Pagarte.Worker.Services;

namespace Pagarte.Worker.Repositories
{
    public class CompanyConfigRepository(IConfiguration configuration) : ICompanyConfigRepository
    {
        private readonly string _connectionString = configuration.GetConnectionString("PagarteDb")
            ?? throw new InvalidOperationException("PagarteDb connection string not configured.");

        public async Task<CompanyConfig?> GetByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT Id, Name, ApiEndpoint, ApiKey FROM Companies WHERE Id = @Id AND IsActive = 1";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CompanyConfig(
                    reader.GetGuid(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3));
            }

            return null;
        }
    }
}
