
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

        [Required]
        [Column("email")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [Column("registered_at")]
        public DateTime RegisteredAt { get; set; } = DateTime.Now;


        [Column("display_name")]
        [StringLength(100)]
        public string? Display_name { get; set; }

        [Column("coin")]
        public int Coin { get; set; } = 100;

        //[Column("is_online")]
        //public bool IsOnline { get; set; }
        [Column("isVerifyEmail")]
        public bool isVerify { get; set; } = false;


        [Required]
        [Column("password")]
        public string Password { get; set; } = null!;

        public ICollection<LoginHistoryModel> LoginHistory { get; set; } = new List<LoginHistoryModel>();
        public ICollection<UserCharacterModel> UserCharacters { get; set; } = new List<UserCharacterModel>();
        public ICollection<UserRelationship> Relationships { get; set; } = new List<UserRelationship>();
        public InventoryModel Inventory { get; set; } = null!;

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

                return builder.ToString(); // Save into Password function
            }
        }

        // Testing the correctly password form
        public bool VerifyPassword(string password)
        {
            string hashedPassword = HashPassword(password);
            return this.Password == hashedPassword;
        }
    }
}
