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
        public string Username { get; set; } = null!;

        [NotMapped]
        private string _passwordHash = null!;


        [Required]
        [Column("Password")]
        public string Password 
        {
            get { return _passwordHash; }
            set { _passwordHash = HashPassword(value); }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
