namespace PondWebApp.Models
{
	public class AdminViewModel
	{
		public string Username { get; set; } = null!;
		public List<Member> Members { get; set; } = null!;
		public int MemberCount { get; set; }
	}
}
