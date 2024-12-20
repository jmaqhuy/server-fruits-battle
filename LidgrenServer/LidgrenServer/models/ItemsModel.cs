using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("items")]
    public class ItemModel
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [Column("image_name")]
        [StringLength(50)]
        public string ImageName { get; set; } = null!;

        [Required]
        [Column("effect_type")]
        [StringLength(50)]
        public string EffectType { get; set; } = null!;

        [Required]
        [Column("value")]
        public int Value { get; set; }

        [Required]
        [Column("duration")]
        public int Duration { get; set; }

        [Required]
        [Column("target")]
        [StringLength(50)]
        public string Target { get; set; } = null!;
    }
}
