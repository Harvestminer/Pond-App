using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace PondWebApp
{
	public class Member
	{
		[Key]
		public int MemberId {  get; set; }

		[StringLength(255)]
		[Column(TypeName = "varchar")]
		public string? DiscordUsername { get; set; } = null!;

		[Required]
		[StringLength(255)]
		[Column(TypeName = "varchar")]
		public string MinecraftUsername { get; set; } = null!;

		[StringLength(255)]
		[Column(TypeName = "varchar")]
		public string? DiscordUUID { get; set; } = null!;

		[Required]
		[StringLength(255)]
		[Column(TypeName = "varchar")]
		public string MinecraftUUID { get; set; } = null!;

		[Required]
		[Column(TypeName = "datetime")]
		public DateTime DateAdded { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime? DateModified { get; set; }
	}
}
