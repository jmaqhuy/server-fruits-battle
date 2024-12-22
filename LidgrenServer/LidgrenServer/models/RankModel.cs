using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("rank")]
    public class RankModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(10)]
        public string Name { get; set; } = null!;

        [Required]
        [Column("asset_name")]
        [StringLength(10)]
        public string AssetName { get; set; } = null!;

        [Required]
        [Column("max_star")]
        public int MaxStar { get; set; }
    }
}
