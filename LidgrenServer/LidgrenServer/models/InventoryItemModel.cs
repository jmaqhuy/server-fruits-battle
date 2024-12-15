using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("inventory_items")]
    public class InventoryItemModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("inventory_id")]
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public InventoryModel Inventory { get; set; } = null!;

        [Required]
        [Column("item_id")]
        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public ItemConsumableModel Item { get; set; } = null!;

        [Column("quantity")]
        public int Quantity { get; set; } = 1;
    }
}
