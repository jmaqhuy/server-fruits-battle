using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("inventories")]
    public class InventoryModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; } = null!;

        public ICollection<InventoryItemModel> Items { get; set; } = new List<InventoryItemModel>();
    }
}
