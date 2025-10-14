using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventureGame
{
    public class GameSetup
    {
        public static Room CreateWorld()
        {
            var start = new Room("Start Room", "You are in the center of the dungeon.");
            var left = new Room("Trap Room", "There's nothing but silence... deadly silence.") { IsDeadly = true };
            var right = new Room("Storage Room", "An old storage room.");
            var up = new Room("Exit Room", "A locked door leads out.") { RequiresKey = true };
            var down = new Room("Armory", "You see a sword resting on a stand.");
            var deep = new Room("Monster Lair", "A growl rumbles from the shadows.") { HasMonster = true };

            right.Items.Add(new Item("key", "A small rusty key."));
            down.Items.Add(new Item("sword", "A sharp steel sword."));

            start.Connect(Direction.West, left);
            start.Connect(Direction.East, right);
            start.Connect(Direction.North, up);
            start.Connect(Direction.South, down);

            down.Connect(Direction.South, deep);
            deep.Connect(Direction.North, down);

            return start;
        }
    }
}
