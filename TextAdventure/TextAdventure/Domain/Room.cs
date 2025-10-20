using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Domain;

public sealed class Room
{
    public string Name { get; }
    public string Description { get; }
    public bool IsDeadly { get; init; }
    public bool RequiresKey { get; init; }
    public bool HasMonster { get; init; }
    public bool MonsterAlive { get; set; }

    private readonly List<Item> _items = new();
    private readonly Dictionary<Direction, Room> _exits = new();

    public Room(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Kamernaam mag niet leeg zijn.");
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
    }

    public void AddItem(Item i) => _items.Add(i);
    public bool HasItem(string id) => _items.Any(i => i.Id == id.ToLowerInvariant());

    public Item? Take(string id)
    {
        var it = _items.FirstOrDefault(i => i.Id == id.ToLowerInvariant());
        if (it != null) _items.Remove(it);
        return it;
    }

    public void Connect(Direction dir, Room target) => _exits[dir] = target;
    public bool TryGetExit(Direction dir, out Room? target) => _exits.TryGetValue(dir, out target);

    public string Describe(bool withExits, bool withItems, bool withMonster)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{Name}]");
        sb.AppendLine(Description);

        if (withMonster && HasMonster)
            sb.AppendLine(MonsterAlive ? "Je hoort gegrom… Er is hier een levend monster." : "Het dode monster ligt hier roerloos.");

        if (withItems)
        {
            if (_items.Count == 0) sb.AppendLine("Geen items hier.");
            else
            {
                sb.AppendLine("Items:");
                foreach (var i in _items) sb.AppendLine($"- {i}");
            }
        }

        if (withExits)
        {
            var ex = _exits.Keys.Select(k => k.ToString().ToLower());
            sb.AppendLine("Uitgangen: " + string.Join(", ", ex));
        }

        return sb.ToString();
    }
}