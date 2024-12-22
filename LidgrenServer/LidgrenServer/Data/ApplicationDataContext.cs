using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.Data
{
    public class ApplicationDataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<UserInventoryModel> UserInventories { get; set; } = null!;
        public DbSet<ProductModel> Products { get; set; } = null!;
        public DbSet<ItemModel> Items { get; set; } = null!;
        public DbSet<ShopModel> Shops { get; set; } = null!;
        public DbSet<UserRelationship> UserRelationships { get; set; } = null!;
        public DbSet<RankModel> Ranks { get; set; } = null!;
        public DbSet<UserRankModel> UserRanks { get; set; } = null!;
        public DbSet<SeasonModel> Seasons { get; set; } = null!;
        public DbSet<CharacterModel> Characters { get; set; } = null!;
        public DbSet<UserCharacterModel> UserCharacters { get; set; } = null!;
        public DbSet<LoginHistoryModel> LoginHistories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Database=Fruits_Battle_Game2;User=root;Password=068958");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Inventories
            modelBuilder.Entity<UserInventoryModel>(entity =>
            {
                entity.HasKey(ui => ui.Id);

                entity.HasOne(ui => ui.User)
                      .WithOne(u => u.Inventory)
                      .HasForeignKey<UserInventoryModel>(ui => ui.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ui => ui.Quantity)
                      .IsRequired();
            });


            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();

                entity.HasMany(u => u.Ranks)
                      .WithOne(ur => ur.User)
                      .HasForeignKey(ur => ur.UserId);

                entity.HasMany(u => u.Relationships)
                      .WithOne(r => r.UserFirst)
                      .HasForeignKey(r => r.UserFirstId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LoginHistoryModel>(entity =>
            {
                entity.HasKey(lh => lh.Id);
                entity.HasIndex(lh => new { lh.UserId, lh.IsLoginNow })
                      .HasDatabaseName("IX_User_LoginHistory_IsLoginNow");
                entity.HasOne(lh => lh.UserModel)
                      .WithMany(u => u.LoginHistory)
                      .HasForeignKey(u => u.UserId);
            });

            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Type).HasMaxLength(10);


            });


            modelBuilder.Entity<UserRelationship>(entity =>
            {
                entity.HasKey(ur => new { ur.UserFirstId, ur.UserSecondId });
                entity.Property(ur => ur.Type)
                      .HasConversion<string>()
                      .HasMaxLength(50)
                      .IsRequired();
            });
            modelBuilder.Entity<UserCharacterModel>(entity =>
            {
                entity.HasKey(uc => uc.Id);
                entity.HasIndex(uc => new { uc.UserId, uc.CharacterId })
                      .HasDatabaseName("IX_UserCharacter_UserId_CharacterId");
                entity.HasIndex(uc => new { uc.UserId, uc.IsSelected })
                      .HasDatabaseName("IX_UserCharacter_UserId_IsSelected");

                entity.HasOne(uc => uc.User)
                      .WithMany(u => u.UserCharacters)
                      .HasForeignKey(uc => uc.UserId);
                entity.HasOne(uc => uc.Character)
                      .WithMany(c => c.UserCharacters)
                      .HasForeignKey(uc => uc.CharacterId);
            });

        }
    }
}
