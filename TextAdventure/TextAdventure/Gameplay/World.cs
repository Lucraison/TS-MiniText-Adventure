using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Domain;

namespace TextAdventure.Gameplay;

public enum EndState { None, Dead, Win }
public sealed record ActionResult(EndState EndState, string Message);

public sealed class World
{
    public Room Current { get; private set; }
    private readonly Room _start;
    private readonly Room _winDoor;
    private readonly Room _monsterRoom;

    public World(Room start, Room winDoor, Room monsterRoom)
    {
        _start = start;
        _winDoor = winDoor;
        _monsterRoom = monsterRoom;
        Current = start;
    }

    public string DescribeCurrent(bool withExits = true, bool withItems = false, bool withMonster = false)
        => Current.Describe(withExits, withItems, withMonster);

    public string TakeFromCurrent(string id, Inventory inv)
    {
        var it = Current.Take(id);
        if (it is null) return $"Geen item met id '{id}' in deze kamer.";
        inv.Add(it);
        return $"Je nam: {it}.";
    }

    public ActionResult Go(Direction dir, Inventory inv)
    {
        // Regel: monsterkamer verlaten terwijl monster leeft => dood.
        if (Current == _monsterRoom && _monsterRoom.MonsterAlive)
            return new ActionResult(EndState.Dead, "Je probeert te vluchten… Het monster sleurt je terug. Je bent DOOD.");

        if (!Current.TryGetExit(dir, out var target) || target == null)
            return new ActionResult(EndState.None, "Daar is geen uitgang.");

        if (target.IsDeadly)
        {
            Current = target;
            return new ActionResult(EndState.Dead, $"Je stapt {dir.ToString().ToLower()}… {target.Name}: {target.Description}\nGAME OVER — je bent DOOD.");
        }

        if (target == _winDoor)
        {
            if (!inv.Contains("key"))
                return new ActionResult(EndState.None, "De deur zit op slot. Je hebt een sleutel nodig.");
            Current = target;
            return new ActionResult(EndState.Win, "Je opent de deur met de sleutel… Zonlicht! JE WINT! 🎉");
        }

        Current = target;
        return new ActionResult(EndState.None, DescribeCurrent(withExits: true, withItems: true, withMonster: true));
    }

    public ActionResult Fight(Inventory inv)
    {
        if (Current != _monsterRoom)
            return new ActionResult(EndState.None, "Hier is niets om tegen te vechten.");

        if (!_monsterRoom.MonsterAlive)
            return new ActionResult(EndState.None, "Het monster is al verslagen.");

        if (!inv.Contains("sword"))
            return new ActionResult(EndState.Dead, "Je valt aan met blote handen… Het monster is te sterk. Je bent DOOD.");

        _monsterRoom.MonsterAlive = false;
        return new ActionResult(EndState.None, "Met een krachtige zwaai versla je het monster. De weg is veilig.");
    }
}