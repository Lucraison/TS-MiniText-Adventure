using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventureGame
{
    public class Inventory
    {
        private List<Item> _items = new();

        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        public bool HasItem(string id)
        {
            return _items.Any(i => i.Id == id);
        }

        public void RemoveItem(string id)
        {
            _items.RemoveAll(i => i.Id == id);
        }

        public List<Item> GetAll()
        {
            return _items.ToList();
        }

        public override string ToString()
        {
            if (_items.Count == 0)
                return "Inventory is empty.";

            return "Inventory:\n" + string.Join("\n", _items.Select(i => "- " + i));
        }
    }
}
