using LidgrenServer.model;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.Data
{
    public class ApplicationDataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Database=Fruits_Battle_Game;User=root;Password=Huy@123.sc");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Username)
                .IsUnique(); 
        }

    }
}
