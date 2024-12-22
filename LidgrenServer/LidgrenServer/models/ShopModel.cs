using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("shop")]
    public class ShopModel
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Products { get; set; } = null!;

        [Required]
        [Column("price")]
        public int Price { get; set; }

        [Required]
        [Column("stock")]
        public int Stock { get; set; }
    }
}
