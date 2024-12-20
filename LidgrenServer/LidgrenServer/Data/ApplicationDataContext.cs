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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Database=Fruits_Battle_Game;User=root;Password=11112003");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Inventories
            modelBuilder.Entity<UserInventoryModel>(entity =>
            {
                entity.HasKey(ui => ui.Id);

                entity.HasOne(ui => ui.User)
                      .WithMany(u => u.UserInventories)
                      .HasForeignKey(ui => ui.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ui => ui.Product)
                      .WithMany()
                      .HasForeignKey(ui => ui.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ui => ui.Quantity)
                      .IsRequired();
            });

            // Other models...

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();

                entity.HasMany(u => u.UserInventories)
                      .WithOne(ui => ui.User)
                      .HasForeignKey(ui => ui.UserId);

                entity.HasMany(u => u.UserRanks)
                      .WithOne(ur => ur.User)
                      .HasForeignKey(ur => ur.UserId);

                entity.HasMany(u => u.Relationships)
                      .WithOne(r => r.UserFirst)
                      .HasForeignKey(r => r.UserFirstId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Type).HasMaxLength(10);
            });

            modelBuilder.Entity<ShopModel>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.Product)
                      .WithMany()
                      .HasForeignKey(s => s.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Add configurations for other models as needed...
        }
    }
}
