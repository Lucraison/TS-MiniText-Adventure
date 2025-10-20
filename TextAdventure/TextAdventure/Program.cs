using System;
using System.Xml;
using TextAdventure.Domain;
using TextAdventure.Gameplay;

namespace TextAdventure;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var world = GameSetup.BuildWorld();
        var inventory = new Inventory();

        Console.WriteLine("Mini Text Adventure — type 'help' voor commando’s.");
        Console.WriteLine(world.DescribeCurrent(withExits: true));

        bool running = true;
        while (running)
        {
            Console.Write("\n> ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(input)) continue;

            var (cmd, arg) = Input.Parse(input);

            switch (cmd)
            {
                case "help":
                    Console.WriteLine("""
                    Beschikbare commando’s:
                      help        — toon deze lijst
                      look        — toon kamer, items en uitgangen
                      inventory   — toon je inventaris
                      go n|e|s|w  — beweeg naar richting (noord/oost/zuid/west)
                      take <id>   — pak een item op (bv. 'take key' of 'take sword')
                      fight       — vecht met het monster (in de monsterkamer)
                      quit        — stop het spel
                    """);
                    break;

                case "look":
                    Console.WriteLine(world.DescribeCurrent(withExits: true, withItems: true, withMonster: true));
                    break;

                case "inventory":
                    Console.WriteLine(inventory.Describe());
                    break;

                case "go":
                    if (string.IsNullOrWhiteSpace(arg)) { Console.WriteLine("Gebruik: go n|e|s|w"); break; }
                    if (!DirectionHelpers.TryParse(arg, out var dir))
                    { Console.WriteLine("Onbekende richting. Gebruik: n, e, s of w."); break; }

                    var move = world.Go(dir, inventory);
                    Console.WriteLine(move.Message);
                    if (move.EndState != EndState.None) running = false;
                    break;

                case "take":
                    if (string.IsNullOrWhiteSpace(arg)) { Console.WriteLine("Gebruik: take <id>"); break; }
                    Console.WriteLine(world.TakeFromCurrent(arg, inventory));
                    break;

                case "fight":
                    var fight = world.Fight(inventory);
                    Console.WriteLine(fight.Message);
                    if (fight.EndState != EndState.None) running = false;
                    break;

                case "quit":
                    Console.WriteLine("Je verlaat het spel. Tot ziens!");
                    running = false;
                    break;

                default:
                    Console.WriteLine("Onbekend commando. Type 'help'.");
                    break;
            }
        }
    }
}
