using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LidgrenServer.Models
{
    [Table("user_characters")]
    public class UserCharacterModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("character_id")]
        public int CharacterId { get; set; }

        [Required]
        [Column("level")]
        public int Level { get; set; } = 1;

        [Required]
        [Column("experience")]
        public int Experience { get; set; } = 0;

        [Required]
        [Column("is_selected")]
        public bool IsSelected { get; set; } = false;

        [Required]
        [Column("hp_point")]
        public int HpPoint { get; set; } = 0;

        [Required]
        [Column("damage_point")]
        public int DamagePoint { get; set; } = 0;

        [Required]
        [Column("armor_point")]
        public int ArmorPoint { get; set; } = 0;

        [Required]
        [Column("luck_point")]
        public int LuckPoint { get; set; } = 0;


        // Điều hướng tới các bảng User và Character
        public UserModel User { get; set; }
        public CharacterModel Character { get; set; }
    }
}
