using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PondWebApp.Services
{
	public class AdminService(PondDBContext dbContext)
	{
		private readonly PondDBContext _dbContext = dbContext;
		private readonly PasswordHasher<Admin> _passwordHasher = new();

		/// <summary>
		/// Returns all members from the database
		/// </summary>
		/// <returns></returns>
		public async Task<List<Admin>> GetAllAdmins()
		{
			return await this._dbContext.Admins
				.AsNoTracking()
				.ToListAsync();
		}

		/// <summary>
		/// Adds a member to the database
		/// </summary>
		/// <param name="admin"></param>
		/// <returns></returns>
		public async void AddAdmin(Admin admin)
		{
			await this._dbContext.Admins.AddRangeAsync(admin);
			this._dbContext.SaveChanges();
		}

		/// <summary>
		/// Authenticate the users username and password against a database
		/// </summary>
		/// <param name="admin"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<bool> AuthenticateAdmin(Admin admin, string password)
		{
			Admin? _admin = await this._dbContext.Admins.FirstOrDefaultAsync(x => x.Username == admin.Username);

			if (_admin == null)
				return false;

			var result = _passwordHasher.VerifyHashedPassword(_admin, _admin.Password, password);
			return result == PasswordVerificationResult.Success;
		}
	}
}
