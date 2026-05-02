using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Pagarte.Engine.Interfaces;

namespace Pagarte.Engine.Services
{
    /// <summary>
    /// Updates payment status in PagarteDb via raw SQL.
    /// Engine does not reference Pagarte.Worker — no circular dependency.
    /// Only updates, never reads complex object graphs.
    /// </summary>
    public class PaymentStatusRepository(IConfiguration configuration) : IPaymentStatusRepository
    {
        private readonly string _connectionString =
            configuration.GetConnectionString("PagarteDb")
            ?? throw new InvalidOperationException("PagarteDb not configured.");

        public async Task UpdateStatusAsync(Guid paymentId, string status,
            string? companyReference = null, string? errorMessage = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                UPDATE Payments
                SET Status = @Status,
                    CompanyReference = COALESCE(@CompanyReference, CompanyReference),
                    ErrorMessage = COALESCE(@ErrorMessage, ErrorMessage),
                    LastUpdatedAt = @UpdatedAt,
                    ProcessedAt = CASE
                        WHEN @Status IN ('Completed','Failed','Refunded','RefundFailed')
                        THEN @UpdatedAt ELSE ProcessedAt END
                WHERE Id = @PaymentId";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@CompanyReference",
                (object?)companyReference ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ErrorMessage",
                (object?)errorMessage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@PaymentId", paymentId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task IncrementRetryAsync(Guid paymentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                UPDATE Payments
                SET RetryCount = RetryCount + 1,
                    NextRetryAt = @NextRetryAt,
                    LastUpdatedAt = @UpdatedAt
                WHERE Id = @PaymentId";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@NextRetryAt",
                DateTime.UtcNow.AddMinutes(5));
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@PaymentId", paymentId);

            await cmd.ExecuteNonQueryAsync();
        }

<<<<<<< HEAD
        public async Task<(string? OperatorPaymentId, decimal Amount,
=======
        public async Task<(string? DLocalPaymentId, decimal Amount,
>>>>>>> origin/main
            string Currency, string ClientId)?> GetPaymentInfoAsync(Guid paymentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
<<<<<<< HEAD
                SELECT p.OperatorPaymentId, pd.Amount, p.Currency, p.ClientId
=======
                SELECT p.DLocalPaymentId, pd.Amount, p.Currency, p.ClientId
>>>>>>> origin/main
                FROM Payments p
                INNER JOIN PaymentDetails pd ON pd.PaymentId = p.Id
                WHERE p.Id = @PaymentId AND pd.Type = 0"; // 0 = ServiceAmount

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@PaymentId", paymentId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    reader.IsDBNull(0) ? null : reader.GetString(0),
                    reader.GetDecimal(1),
                    reader.GetString(2),
                    reader.GetString(3)
                );
            }

            return null;
        }
    }
}
