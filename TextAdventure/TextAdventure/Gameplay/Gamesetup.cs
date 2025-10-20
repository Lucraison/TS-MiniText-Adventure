using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextAdventure.Domain;

namespace TextAdventure.Gameplay;

public static class GameSetup
{
    // Layout:
    // Start
    // West  -> dodelijk
    // East  -> sleutel
    // North -> deur (win, vereist key)
    // South -> zwaard
    // South->South -> monsterkamer
    public static World BuildWorld()
    {
        var start = new Room("Start", "Je staat in het midden van een vreemde, stille ruïne.");
        var west = new Room("Afgrond", "Een stap te ver… je tuimelt de diepte in.") { IsDeadly = true };
        var east = new Room("Berging", "Een stoffige bergruimte. Iets glinstert in de hoek.");
        var north = new Room("Deur", "Een zware deur met een ingewikkeld slot.") { RequiresKey = true };
        var south = new Room("Arsenal", "Een rek met verouderde wapens en schilden.");
        var deep = new Room("Grot", "Een vochtige grot. Het gegrom van een monster vult de ruimte.") { HasMonster = true, MonsterAlive = true };

        east.AddItem(new Item("key", "Sleutel", "Een roestige sleutel."));
        south.AddItem(new Item("sword", "Zwaard", "Een zwaar zwaard, maar nog bruikbaar."));

        start.Connect(Direction.West, west);
        start.Connect(Direction.East, east);
        start.Connect(Direction.North, north);
        start.Connect(Direction.South, south);

        south.Connect(Direction.North, start);
        south.Connect(Direction.South, deep);

        east.Connect(Direction.West, start);
        north.Connect(Direction.South, start);
        deep.Connect(Direction.North, south);

        return new World(start, north, deep);
    }
}

