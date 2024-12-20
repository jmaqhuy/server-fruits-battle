using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("user_ranks")]
    public class UserRankModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; } = null!;

        [Required]
        [Column("rank_id")]
        public int RankId { get; set; }

        [ForeignKey("RankId")]
        public RankModel Rank { get; set; } = null!;

        [Required]
        [Column("season_id")]
        public int SeasonId { get; set; }

        [ForeignKey("SeasonId")]
        public SeasonModel Season { get; set; } = null!;

        [Required]
        [Column("current_star")]
        public int CurrentStar { get; set; }
    }
}
