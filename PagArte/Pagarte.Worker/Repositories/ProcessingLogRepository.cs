using Microsoft.EntityFrameworkCore;
using Pagarte.Worker.Domain;
using Pagarte.Worker.Infrastructure;

namespace Pagarte.Worker.Repositories
{
    public class ProcessingLogRepository(WorkerDbContext context) : IProcessingLogRepository
    {
        private readonly WorkerDbContext _context = context;

        public async Task<ProcessingLog> CreateAsync(ProcessingLog log)
        {
            _context.ProcessingLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task UpdateAsync(ProcessingLog log)
        {
            _context.ProcessingLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProcessingLog>> GetPendingRetriesAsync()
        {
            return await _context.ProcessingLogs
                .Where(l => l.Status == MessageStatus.Failed
                    && l.RetryCount < 3
                    && l.NextRetryAt <= DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
