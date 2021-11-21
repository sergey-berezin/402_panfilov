using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBase
{
    public class DataBaseManager
    {
        private LibraryContext db = new();

        public event Action DataChanged;


        public bool Add(Item item)
        {
            if (CheckItemPresence(item)) return false;

            db.Items.Add(item);
            db.SaveChanges();
            DataChanged?.Invoke();
            return true;
        }

        public async Task AddAsync(Item item)
        {
            if (CheckItemPresence(item)) return;

            await db.Items.AddAsync(item);
            await db.SaveChangesAsync();

            await Task.Delay(1);           // Без этой строки ui зависает

            DataChanged?.Invoke();
        }

        public void Clear()
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            DataChanged?.Invoke();
        }

        bool CheckItemPresence(Item item)
        {
            bool result = false;
            var query = db.Items.Where(x => (x.X1 == item.X1) && (x.X2 == item.X2) && (x.Y1 == item.Y1) && (x.Y2 == item.Y2));
            foreach (var elem in query)
            {
                if (elem.Image.SequenceEqual(item.Image))
                    result = true;
            }

            return result;
        }

        public IEnumerable<string> GetClassList()
        {
            IEnumerable<string> result;
            result = db.Items.Select(x => x.Label).Distinct().Select(x => $"[{db.Items.Where(y => y.Label == x).Count()}] {x}");
            return result;
        }

        public IEnumerable<byte[]> GetImageList(string label)
        {
            IEnumerable<byte[]> result;
            result = db.Items.Where(x => x.Label == label).Select(x => x.Image);
            return result;
        }
    }
}
