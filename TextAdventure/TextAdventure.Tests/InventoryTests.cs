using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextAdventure.Domain;
using Xunit;

namespace TextAdventure.Tests;

public class InventoryTests
{
    [Fact]
    public void Add_ThenContains_ReturnsTrue()
    {
        var inv = new Inventory();
        inv.Add(new Item("key", "Sleutel", "test"));
        Assert.True(inv.Contains("key"));
    }

    [Fact]
    public void Remove_RemovesItem()
    {
        var inv = new Inventory();
        inv.Add(new Item("sword", "Zwaard", "test"));
        var removed = inv.Remove("sword");
        Assert.NotNull(removed);
        Assert.False(inv.Contains("sword"));
    }
}

