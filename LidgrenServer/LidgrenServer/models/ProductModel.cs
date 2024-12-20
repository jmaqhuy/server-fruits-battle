using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
	[Table("products")]
	public class ProductModel
	{
		[Key]
		[Required]
		[Column("id")]
		public int Id { get; set; }

		[Required]
		[StringLength(10)]
		[Column("type")]
		public string Type { get; set; } = null!;

		[Column("related_id")]
		public int? RelatedId { get; set; }
	}
}
