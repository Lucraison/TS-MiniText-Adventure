using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventureGame
{
    public class Rooms
    {
        public Room CurrentRoom { get; private set; }
        public Inventory PlayerInventory { get; } = new();

        public Rooms(Room startRoom)
        {
            CurrentRoom = startRoom;
        }

        public string Go(Direction dir)
        {
            var nextRoom = CurrentRoom.GetExit(dir);

            if (nextRoom == null)
                return "You can't go that way.";

            if (nextRoom.RequiresKey && !PlayerInventory.HasItem("key"))
                return "The door is locked. You need a key.";

            CurrentRoom = nextRoom;

            if (CurrentRoom.IsDeadly)
                return "You stepped into a trap! Game over.";

            if (CurrentRoom.HasMonster)
                return "A monster appears!";

            return $"You moved to: {CurrentRoom.Name}";
        }

        public string Take(string id)
        {
            var item = CurrentRoom.TakeItem(id);
            if (item == null)
                return "No such item found here.";

            PlayerInventory.AddItem(item);
            return $"You picked up: {id}";
        }

        public string Fight()
        {
            if (!CurrentRoom.HasMonster)
                return "Nothing to fight here.";

            if (!PlayerInventory.HasItem("sword"))
                return "You don't have a sword! The monster kills you. Game over.";

            CurrentRoom.HasMonster = false;
            return "You defeated the monster!";
        }
    }
}
