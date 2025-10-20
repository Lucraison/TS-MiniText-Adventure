using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextAdventure.Domain;
using TextAdventure.Gameplay;
using Xunit;

namespace TextAdventure.Tests;

public class WorldRulesTests
{
    [Fact]
    public void WestFromStart_IsDead()
    {
        var world = GameSetup.BuildWorld();
        var inv = new Inventory();

        var result = world.Go(Direction.West, inv);

        Assert.Equal(EndState.Dead, result.EndState);
        Assert.Contains("DOOD", result.Message);
    }

    [Fact]
    public void WinningPath_EastTakeKey_NorthWins()
    {
        var world = GameSetup.BuildWorld();
        var inv = new Inventory();

        var gateTry = world.Go(Direction.North, inv);
        Assert.Equal(EndState.None, gateTry.EndState);
        Assert.Contains("sleutel", gateTry.Message.ToLower());

        world.Go(Direction.East, inv);
        world.TakeFromCurrent("key", inv);
        Assert.True(inv.Contains("key"));

        world.Go(Direction.West, inv);
        var win = world.Go(Direction.North, inv);

        Assert.Equal(EndState.Win, win.EndState);
        Assert.Contains("JE WINT", win.Message);
    }

    [Fact]
    public void Monster_KillsOnEscape_AttackWithSwordIsSafe()
    {
        var world = GameSetup.BuildWorld();
        var inv = new Inventory();

        world.Go(Direction.South, inv);
        world.TakeFromCurrent("sword", inv);
        Assert.True(inv.Contains("sword"));

        world.Go(Direction.South, inv);

        var fight = world.Fight(inv);
        Assert.Equal(EndState.None, fight.EndState);
        Assert.Contains("versla", fight.Message.ToLower());

        var outMsg = world.Go(Direction.North, inv);
        Assert.Equal(EndState.None, outMsg.EndState);
    }

    [Fact]
    public void Monster_EscapeWithoutFight_Dead()
    {
        var world = GameSetup.BuildWorld();
        var inv = new Inventory();

        world.Go(Direction.South, inv);
        world.Go(Direction.South, inv);

        var flee = world.Go(Direction.North, inv);

        Assert.Equal(EndState.Dead, flee.EndState);
    }
}

