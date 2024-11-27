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

        // Điều hướng tới các bảng User và Character
        public UserModel User { get; set; }
        public CharacterModel Character { get; set; }
    }
}
