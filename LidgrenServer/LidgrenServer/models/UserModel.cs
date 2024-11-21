using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Security.Cryptography;

namespace LidgrenServer.Models
{
    [Table("users")]
    public class UserModel
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; } = null!;


        [Column("display_name")]
        [StringLength(100)]
        public string? Display_name { get; set; }

        [Column("coin")]
        public int? Coin { get; set; }

        [Column("is_online")]
        public bool IsOnline { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; } = null!;

        public ICollection<LoginHistory> LoginHistory { get; set; } = new List<LoginHistory>();

        // Quan hệ nhiều-nhiều với Character thông qua bảng UserCharacter
        public ICollection<UserCharacter> UserCharacters { get; set; } = new List<UserCharacter>();
        // Hàm mã hóa mật khẩu khi người dùng nhập vào
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString(); // Lưu mật khẩu đã mã hóa vào thuộc tính Password
            }
        }

        // Kiểm tra mật khẩu có hợp lệ không
        public bool VerifyPassword(string password)
        {
            string hashedPassword = HashPassword(password);
            return this.Password == hashedPassword;
        }
    }
}
