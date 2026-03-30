using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Pagarte.Worker.Repositories
{
    // This repository connects to PagarteDb (same DB as Pagarte.API)
    // to update payment statuses from the worker
    public class PaymentWorkerRepository : IPaymentWorkerRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<PaymentWorkerRepository> _logger;

        public PaymentWorkerRepository(IConfiguration configuration,
            ILogger<PaymentWorkerRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("PagarteDb")
                ?? throw new InvalidOperationException("PagarteDb connection string not configured.");
            _logger = logger;
        }

        public async Task UpdateStatusAsync(Guid paymentId, string status,
            string? companyReference = null, string? errorMessage = null)
        {
            try
            {
                // Use raw SQL to avoid EF Core dependency on Pagarte.API project
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    UPDATE Payments
                    SET Status = @Status,
                        CompanyReference = COALESCE(@CompanyReference, CompanyReference),
                        ErrorMessage = COALESCE(@ErrorMessage, ErrorMessage),
                        LastUpdatedAt = @UpdatedAt,
                        ProcessedAt = CASE
                            WHEN @Status IN ('Completed','Failed','Refunded','RefundFailed')
                            THEN @UpdatedAt
                            ELSE ProcessedAt
                        END
                    WHERE Id = @PaymentId";

                using var command = new Microsoft.Data.SqlClient.SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@CompanyReference",
                    (object?)companyReference ?? DBNull.Value);
                command.Parameters.AddWithValue("@ErrorMessage",
                    (object?)errorMessage ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@PaymentId", paymentId);

                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Payment {PaymentId} status updated to {Status}",
                    paymentId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment {PaymentId} status", paymentId);
                throw;
            }
        }
    }
}
