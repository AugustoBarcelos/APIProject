using APIProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace APIProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Player> Players_API { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().HasData(
                new Player()
                {
                    Id = 1,
                    Name = "Mbape",
                    Country = "France",
                    Type = "Player",
                    Price = 115000
                },
                new Player()
                {
                    Id = 2,
                    Name = "Giroud",
                    Country = "France",
                    Type = "Player",
                    Price = 1100
                }, new Player()
                {
                    Id = 3,
                    Name = "Neuer",
                    Country = "Germany",
                    Type = "Player",
                    Price = 18000
                }
                );
        }
    }
}
