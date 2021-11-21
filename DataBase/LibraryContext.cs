using Microsoft.EntityFrameworkCore;

namespace DataBase
{
    public partial class Item
    {
        public int ItemId { get; set; }

        public float X1 { get; set; }
        public float X2 { get; set; }
        public float Y1 { get; set; }
        public float Y2 { get; set; }
        public byte[] Image { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
    }

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
