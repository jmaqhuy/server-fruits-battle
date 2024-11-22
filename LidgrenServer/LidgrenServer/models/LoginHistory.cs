using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LidgrenServer.Models
{
    [Table("login_history")]
    public class LoginHistory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel UserModel { get; set; } = null!;

        [Required]
        [Column("login_time")]
        public DateTime LoginTime { get; set; }

        [Column("logout_time")]
        public DateTime? LogoutTime { get; set; }

        [Required]
        [Column("is_online_now")]
        public bool IsLoginNow { get; set; }

        //[Required]
        //[Column("device_id")]
        //[StringLength(255)]
        //public string DeviceId { get; set; } = null!;


    }
}
