using Microsoft.EntityFrameworkCore;

namespace PondWebApp
{
	public class PondDBContext(DbContextOptions<PondDBContext> options) : DbContext(options)
	{
		public DbSet<Member> Members { get; set; }
		public DbSet<Admin> Admins { get; set; }
	}
}
