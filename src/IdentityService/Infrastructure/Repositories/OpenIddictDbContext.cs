using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace IdentityService.Infrastructure.Repositories
{
	public class OpenIddictDbContext(DbContextOptions<OpenIddictDbContext> options) : DbContext(options)
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.UseOpenIddict();
		}
	}
}
