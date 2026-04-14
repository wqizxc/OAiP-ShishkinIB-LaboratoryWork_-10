using Microsoft.EntityFrameworkCore;
using laba_10.Models;

namespace laba_10.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Photographer> Photographers { get; set; }

        private static AppDbContext? _context;

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        public static AppDbContext GetContext()
        {
            if (_context == null)
                _context = new AppDbContext();
            return _context;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=PhotographerAuthDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;");
        }
    }
}