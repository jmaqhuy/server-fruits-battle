using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Security.Cryptography;

namespace LidgrenServer.model
{
    [Table("Users")]
    public class UserModel
    {
        [Key]
        [Required]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Username")]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Column("display_name")]
        [StringLength(100)]
        public string? display_name { get; set; }

        [Column("coin")]
        public int? coin { get; set; }

        [Required]
        [Column("Password")]
        public string Password { get; set; } = null!;

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
