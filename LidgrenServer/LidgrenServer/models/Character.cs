using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LidgrenServer.Models
{
    [Table("character")]
    public class Character
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("level")]
        public int level { get; set; } = 1;

        [Required]
        [Column("exp")]
        public int exp {  get; set; }

        [Required]
        [Column("is_selected_character")]
        public bool IsSelectedCharacter { get; set; }

        public ICollection<UserCharacter> UserCharacters { get; set; } = new List<UserCharacter>();
    }
}
