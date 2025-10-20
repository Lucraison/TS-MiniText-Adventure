using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Domain;

public sealed class Inventory
{
    private readonly List<Item> _items = new();

    public bool Contains(string id) => _items.Any(i => i.Id == id.ToLowerInvariant());

    public void Add(Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _items.Add(item);
    }

    public Item? Remove(string id)
    {
        var it = _items.FirstOrDefault(i => i.Id == id.ToLowerInvariant());
        if (it != null) _items.Remove(it);
        return it;
    }

    public string Describe()
    {
        if (_items.Count == 0) return "Je inventory is leeg.";
        var sb = new StringBuilder("Inventory:\n");
        foreach (var i in _items) sb.AppendLine($"- {i}");
        return sb.ToString();
    }
}
