using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.Data
{
    public class ApplicationDataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<LoginHistory> LoginHistories { get; set; } = null!;
        public DbSet<Character> Characters { get; set; }
        public DbSet<UserCharacter> UserCharacters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Database=Fruits_Battle_Game;User=root;Password=Huy@123.sc");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username);
                entity.HasMany(u => u.LoginHistory)
                      .WithOne(h => h.UserModel)
                      .HasForeignKey(h => h.UserId);
            });

            modelBuilder.Entity<LoginHistory>(entity =>
            {
                entity.HasKey(ld => ld.Id);
                modelBuilder.Entity<LoginHistory>()
                    .HasIndex(lh => new { lh.UserId, lh.IsLoginNow })
                    .HasDatabaseName("IX_User_Device_IsLoginNow");

                modelBuilder.Entity<LoginHistory>()
                    .HasOne(lh => lh.UserModel)
                    .WithMany(u => u.LoginHistory)
                    .HasForeignKey(lh => lh.UserId);
            });
            modelBuilder.Entity<UserCharacter>()
                .HasKey(uc => new { uc.UserId, uc.CharacterId });

            modelBuilder.Entity<UserCharacter>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCharacters)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCharacter>()
                .HasOne(uc => uc.Character)
                .WithMany(c => c.UserCharacters)
                .HasForeignKey(uc => uc.CharacterId);
        }

    }
}
