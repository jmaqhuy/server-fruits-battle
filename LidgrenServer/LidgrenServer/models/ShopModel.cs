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
    }
}
