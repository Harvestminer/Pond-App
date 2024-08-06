using Microsoft.EntityFrameworkCore;

namespace PondWebApp.Services
{
	public class MemberService(PondDBContext dbContext)
	{
		private readonly PondDBContext _dbContext = dbContext;

		/// <summary>
		/// Returns all members from the database
		/// </summary>
		/// <returns></returns>
		public async Task<List<Member>> GetAllMembers()
		{
			return await this._dbContext.Members
				.AsNoTracking()
				.ToListAsync();
		}

		/// <summary>
		/// Adds a member to the database
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public async Task AddMember(Member member)
		{
			await this._dbContext.Members.AddAsync(member);
			await this._dbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Adds a range of members to the database
		/// </summary>
		/// <param name="members"></param>
		/// <returns></returns>
		public async Task AddMemberRange(List<Member> members)
		{
			await this._dbContext.Members.AddRangeAsync(members);
			await this._dbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Get a member from the database with an id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Member?> GetMemberFromID(int id)
		{
			return await this._dbContext.Members.FirstOrDefaultAsync(x => x.MemberId == id);
		}

		/// <summary>
		/// Edit a member from within a database
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public async Task EditMember(Member member)
		{
			this._dbContext.Members.Update(member);
			await this._dbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Is a member already present in the database
		/// </summary>
		/// <param name="uuid"></param>
		/// <returns></returns>
		public async Task<bool> IsMemberPresent(string uuid)
		{
			return await _dbContext.Members.AnyAsync(m => m.MinecraftUUID == uuid);
		}

		/// <summary>
		/// Remove a member using their id from within the database
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task RemoveMember(int id)
		{
			var result = await this.GetMemberFromID(id);

			if (result == null)
			{
				throw new InvalidOperationException($"No customer found with ID {id}");
			}
			else
			{
				this._dbContext.Members.Remove(result);
				await this._dbContext.SaveChangesAsync();
			}
		}
	}
}
