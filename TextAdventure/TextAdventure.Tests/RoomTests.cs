using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextAdventure.Domain;
using Xunit;

namespace TextAdventure.Tests;

public class RoomTests
{
    [Fact]
    public void Take_RemovesItemFromRoom()
    {
        var r = new Room("Test", "desc");
        r.AddItem(new Item("key", "Sleutel", ""));
        Assert.True(r.HasItem("key"));

        var it = r.Take("key");

        Assert.NotNull(it);
        Assert.False(r.HasItem("key"));
    }
}

