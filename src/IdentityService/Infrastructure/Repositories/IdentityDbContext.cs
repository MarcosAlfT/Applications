using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories
{
	public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
	{
		public DbSet<User> Users { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasIndex(u => new { u.Email }).IsUnique();
				entity.HasIndex(u => new { u.Username }).IsUnique();
			});
		}
	}
}
