using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LidgrenServer.Models
{
    [Table("user_characters")]
    public class UserCharacter
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
        public Character Character { get; set; }
    }
}
