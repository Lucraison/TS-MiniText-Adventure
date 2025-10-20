using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Domain;

public enum Direction { North, East, South, West }

public static class DirectionHelpers
{
    public static bool TryParse(string token, out Direction dir)
    {
        dir = Direction.North;
        token = token.Trim().ToLowerInvariant();
        return token switch
        {
            "n" or "north" => (dir = Direction.North) is Direction,
            "e" or "east" => (dir = Direction.East) is Direction,
            "s" or "south" => (dir = Direction.South) is Direction,
            "w" or "west" => (dir = Direction.West) is Direction,
            _ => false
        };
    }
}

