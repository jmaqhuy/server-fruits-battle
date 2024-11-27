using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LidgrenServer.Models
{
    [Table("user_relationship")]
    public class UserRelationship
    {
        [Key, Column("user_first_id")]
        public int UserFirstId { get; set; }

        [Key, Column("user_second_id")]
        public int UserSecondId { get; set; }

        [Required]
        [Column("type")]
        public RelationshipType Type { get; set; }  // Sử dụng enum thay vì string

        // Navigation properties
        [ForeignKey(nameof(UserFirstId))]
        public UserModel UserFirst { get; set; } = null!;

        [ForeignKey(nameof(UserSecondId))]
        public UserModel UserSecond { get; set; } = null!;
    }

    public enum RelationshipType
    {
        PENDING_FIRST_SECOND = 1,
        PENDING_SECOND_FIRST = 2,
        FRIENDS = 3,
        BLOCK_FIRST_SECOND = 4,
        BLOCK_SECOND_FIRST = 5
    }
}
