using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure.Domain;
using TextAdventure.Gameplay;
using Xunit;
using System.IO;

namespace TextAdventure.Tests;

public class BddRunner
{
    private readonly World _world = GameSetup.BuildWorld();
    private readonly Inventory _inv = new();
    private ActionResult? _last;

    public void Given(string step)
    {
        if (!step.Equals("I am at Start", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Unknown Given: {step}");
        // start is default
    }

    public void When(string step)
    {
        var s = step.Trim();
        if (s.StartsWith("And ", StringComparison.OrdinalIgnoreCase)) s = s[4..];

        if (s.StartsWith("I go ", StringComparison.OrdinalIgnoreCase))
        {
            var token = s.Split(' ').Last();
            if (!DirectionHelpers.TryParse(token, out var dir)) throw new InvalidOperationException($"Bad dir {token}");
            _last = _world.Go(dir, _inv);
        }
        else if (s.StartsWith("I take ", StringComparison.OrdinalIgnoreCase))
        {
            var id = s.Split(' ').Last().ToLowerInvariant();
            _world.TakeFromCurrent(id, _inv);
        }
        else if (s.Equals("I fight", StringComparison.OrdinalIgnoreCase))
        {
            _last = _world.Fight(_inv);
        }
        else
        {
            throw new InvalidOperationException($"Unknown When: {step}");
        }
    }

    public void Then(string step)
    {
        switch (step.ToLowerInvariant())
        {
            case "i should win":
                Assert.NotNull(_last);
                Assert.Equal(EndState.Win, _last!.EndState);
                break;
            case "i should be dead":
                Assert.NotNull(_last);
                Assert.Equal(EndState.Dead, _last!.EndState);
                break;
            case "i should be alive":
                Assert.True(_last == null || _last.EndState == EndState.None);
                break;
            default: throw new InvalidOperationException($"Unknown Then: {step}");
        }
    }

    public static IEnumerable<string[]> LoadFeature(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Features", fileName);
        var lines = File.ReadAllLines(path)
                        .Select(l => l.Trim())
                        .Where(l => l.Length > 0 && !l.StartsWith("#", StringComparison.Ordinal))
                        .ToList();

        var scenarios = new List<List<string>>();
        List<string>? current = null;
        foreach (var l in lines)
        {
            if (l.StartsWith("Scenario:", StringComparison.OrdinalIgnoreCase))
            {
                current = new List<string>();
                scenarios.Add(current);
            }
            else if (current != null && !l.StartsWith("Feature:", StringComparison.OrdinalIgnoreCase))
            {
                current.Add(l);
            }
        }
        return scenarios.Select(s => s.ToArray());
    }
}
