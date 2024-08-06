using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PondWebApp
{
	public class Admin
	{
		[Key]
		public int AdminId { get; set; }

		[Required]
		[StringLength(50)]
		[Column(TypeName = "varchar")]
		public string Username { get; set; } = null!;

		[Required]
		[StringLength(255)]
		[Column(TypeName = "varchar")]
		public string Password { get; set; } = null!;

		[Required]
		[Column(TypeName = "datetime")]
		public DateTime DateAdded { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime? DateModified { get; set; }
	}
}
