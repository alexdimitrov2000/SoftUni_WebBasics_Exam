using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PandaWebApp.Models;

namespace PandaWebApp.Data
{
    public class PandaDbContext : DbContext
    {
        public PandaDbContext(DbContextOptions options) : base(options)
        {
        }

        public PandaDbContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                   .UseLazyLoadingProxies()
                   .UseSqlServer(@"Server=DESKTOP-TI6GEI6\SQLEXPRESS;Database=Panda;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.Receipts)
            //    .WithOne(r => r.Recipient)
            //    .OnDelete(DeleteBehavior.ClientSetNull);

            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.Packages)
            //    .WithOne(p => p.Recipient);

            modelBuilder.Entity<Package>()
            .HasOne(p => p.Receipt)
            .WithOne(r => r.Package)
            .HasForeignKey<Receipt>(r => r.PackageId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
