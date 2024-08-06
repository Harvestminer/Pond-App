using System.ComponentModel.DataAnnotations.Schema;

namespace PondWebApp.Domain
{
	public class JsonMember
	{
		[NotMapped]
		public string name { get; set; } = null!;
		[NotMapped]
		public string uuid { get; set; } = null!;
	}
}
