using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventureGame
{
    public class Room
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsDeadly { get; set; }
        public bool RequiresKey { get; set; }
        public bool HasMonster { get; set; }

        public List<Item> Items { get; } = new();
        public Dictionary<Direction, Room> Exits { get; } = new();

        public Room(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void Connect(Direction dir, Room room)
        {
            Exits[dir] = room;
        }

        public Room GetExit(Direction dir)
        {
            return Exits.ContainsKey(dir) ? Exits[dir] : null;
        }

        public Item TakeItem(string id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                Items.Remove(item);
            }
            return item;
        }

        public string Describe()
        {
            var exits = string.Join(", ", Exits.Keys);
            var items = Items.Count == 0 ? "No items here." : "Items: " + string.Join(", ", Items.Select(i => i.Id));
            return $"{Name}\n{Description}\n{items}\nExits: {exits}";
        }
    }
}
