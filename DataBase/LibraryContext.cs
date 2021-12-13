using Contract;
using Microsoft.EntityFrameworkCore;

namespace DataBase
{
    class LibraryContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        public LibraryContext()
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Item>().HasKey(x => x.ItemId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder b) => b.UseSqlite(@"Data Source=D:\And\Prog\cs\402_panfilov\DataBase\library.db");
    }
}
