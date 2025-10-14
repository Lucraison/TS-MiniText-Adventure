using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventureGame
{
    public class Item
    {
        public Item(string id, string description)
        {
            Id = id;
            Description = description;
        }


        public string Id { get;}
        public string Description { get;}

        public override string ToString()
        {
            return $"{Id}: {Description}";
        }
    }
}
