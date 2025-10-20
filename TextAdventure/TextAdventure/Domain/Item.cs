using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Domain;

public sealed class Item
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    public Item(string id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Item id mag niet leeg zijn.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Item naam mag niet leeg zijn.");
        Id = id.Trim().ToLowerInvariant();
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
    }

    public override string ToString() => $"{Name} ({Id})";
}

