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
        public DbSet<InventoryModel> Inventories { get; set; } = null!;
        public DbSet<InventoryItemModel> InventoryItems { get; set; } = null!;
        public DbSet<ItemConsumableModel> ItemConsumable { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Database=Fruits_Battle_Game;User=root;Password=nam171103");
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
                entity.HasOne(u => u.Inventory)
                      .WithOne(i => i.User)
                      .HasForeignKey<InventoryModel>(i => i.UserId);
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
                entity.HasIndex(lh => new { lh.UserId, lh.IsLoginNow })
                      .HasDatabaseName("IX_User_Device_IsLoginNow");
                entity.HasOne(lh => lh.UserModel)
                      .WithMany(u => u.LoginHistory)
                      .HasForeignKey(lh => lh.UserId);   
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

            modelBuilder.Entity<InventoryModel>(entity =>
            {
                entity.HasKey(inv => inv.Id);

                entity.HasOne(inv => inv.User)
                      .WithOne(u => u.Inventory)
                      .HasForeignKey<InventoryModel>(inv => inv.UserId);
            });

            modelBuilder.Entity<InventoryItemModel>(entity =>
            {
                entity.HasKey(ii => ii.Id);

                entity.HasOne(ii => ii.Inventory)
                      .WithMany(inv => inv.Items)
                      .HasForeignKey(ii => ii.InventoryId);

                entity.HasOne(ii => ii.Item)
                      .WithMany()
                      .HasForeignKey(ii => ii.ItemId);
            });

            modelBuilder.Entity<ItemConsumableModel>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Name).IsRequired().HasMaxLength(100);
                entity.Property(i => i.Description).HasMaxLength(500);
                entity.Property(i => i.ImageName).HasMaxLength(200);
            });
        }

    }
}
