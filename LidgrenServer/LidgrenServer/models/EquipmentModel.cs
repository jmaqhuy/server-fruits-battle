using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
	[Table("equipments")]
	public class EquipmentModel
	{
		[Key]
		[Required]
		[Column("id")]
		public int Id { get; set; }

		[Required]
		[StringLength(20)]
		[Column("name")]
		public string Name { get; set; } = null!;

		[Required]
		[StringLength(20)]
		[Column("type")]
		public string Type { get; set; } = null!;
	}
}
