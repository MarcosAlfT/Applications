using Microsoft.EntityFrameworkCore;
using Pagarte.Worker.Domain;

namespace Pagarte.Worker.Infrastructure
{
    public class WorkerDbContext : DbContext
    {
        public WorkerDbContext(DbContextOptions<WorkerDbContext> options) : base(options) { }

        public DbSet<ProcessingLog> ProcessingLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProcessingLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MessagePayload).IsRequired();
                entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
                entity.HasIndex(e => e.PaymentId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.NextRetryAt);
            });
        }
    }
}
