using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.Data
{
    public class ApplicationDataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<LoginHistoryModel> LoginHistories { get; set; } = null!;
        public DbSet<CharacterModel> Characters { get; set; }
        public DbSet<UserCharacterModel> UserCharacters { get; set; }

        public DbSet<UserRelationship> UserRelationships { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Database=Fruits_Battle_Game;User=root;Password=Huy@123.sc");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasMany(u => u.LoginHistory)
                      .WithOne(h => h.UserModel)
                      .HasForeignKey(h => h.UserId);

                entity.HasMany(u => u.Relationships)
                      .WithOne(r => r.UserFirst)
                      .HasForeignKey(r => r.UserFirstId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserRelationship>(entity =>
            {
                entity.HasKey(ur => new { ur.UserFirstId, ur.UserSecondId });
                entity.Property(ur => ur.Type)
                      .HasConversion<string>()
                      .HasMaxLength(50)
                      .IsRequired();
            });

            modelBuilder.Entity<LoginHistoryModel>(entity =>
            {
                entity.HasKey(ld => ld.Id);
                modelBuilder.Entity<LoginHistoryModel>()
                    .HasIndex(lh => new { lh.UserId, lh.IsLoginNow })
                    .HasDatabaseName("IX_User_Device_IsLoginNow");

                modelBuilder.Entity<LoginHistoryModel>()
                    .HasOne(lh => lh.UserModel)
                    .WithMany(u => u.LoginHistory)
                    .HasForeignKey(lh => lh.UserId);
            });
            modelBuilder.Entity<UserCharacterModel>()
                .HasKey(uc => new { uc.UserId, uc.CharacterId });

            modelBuilder.Entity<UserCharacterModel>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCharacters)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCharacterModel>()
                .HasOne(uc => uc.Character)
                .WithMany(c => c.UserCharacters)
                .HasForeignKey(uc => uc.CharacterId);
        }

    }
}
