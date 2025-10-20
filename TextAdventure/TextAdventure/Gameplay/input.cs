using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Gameplay;

public static class Input
{
    public static (string cmd, string arg) Parse(string raw)
    {
        raw = raw.Trim();
        if (string.IsNullOrEmpty(raw)) return (string.Empty, string.Empty);
        var firstSpace = raw.IndexOf(' ');
        if (firstSpace < 0) return (raw.ToLowerInvariant(), string.Empty);
        var cmd = raw[..firstSpace].ToLowerInvariant();
        var arg = raw[(firstSpace + 1)..].Trim();
        return (cmd, arg);
    }
}

