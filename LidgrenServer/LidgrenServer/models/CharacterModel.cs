using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("character")]
    public class CharacterModel
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("hp")]
        public int Hp { get; set; }

        [Required]
        [Column("damage")]
        public int Damage { get; set; }

        [Required]
        [Column("armor")]
        public int Armor { get; set; }

        [Required]
        [Column("stamina")]
        public int Stamina { get; set; }

        [Required]
        [Column("luck")]
        public int Luck { get; set; }

        public ICollection<UserCharacterModel> UserCharacters { get; set; } = new List<UserCharacterModel>();
    }
}
