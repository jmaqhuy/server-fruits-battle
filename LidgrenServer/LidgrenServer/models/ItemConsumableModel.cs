using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("items")]
    public class ItemConsumableModel
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Column("image_name")]
        [StringLength(200)]
        public string ImageName { get; set; } = null!;
    }
}
