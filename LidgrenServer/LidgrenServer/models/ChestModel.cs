using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("chests")]
    public class ChestModel
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("value")]
        public int Value { get; set; }
    }
}
