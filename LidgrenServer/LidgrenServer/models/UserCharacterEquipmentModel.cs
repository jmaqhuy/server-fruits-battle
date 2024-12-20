using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
	[Table("user_character_equipments")]
	public class UserCharacterEquipmentModel
	{
		[Key]
		[Required]
		[Column("id")]
		public int Id { get; set; }

		[Required]
		[ForeignKey("UserCharacterModel")]
		[Column("user_character_id")]
		public int UserCharacterId { get; set; }

		[Required]
		[ForeignKey("EquipmentModel")]
		[Column("equipment_id")]
		public int EquipmentId { get; set; }

		public UserCharacterModel UserCharacter { get; set; } = null!;
		public EquipmentModel Equipment { get; set; } = null!;
	}
}
