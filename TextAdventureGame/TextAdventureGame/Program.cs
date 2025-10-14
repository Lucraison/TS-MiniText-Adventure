namespace TextAdventureGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            var startRoom = GameSetup.CreateWorld();
            var game = new Rooms(startRoom);

            Console.WriteLine("Welkom bij het Text Adventure Game!");
            Console.WriteLine("Typ 'help' voor commando's.\n");

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToLower();

                if (input == "quit")
                {
                    Console.WriteLine("Bedankt voor het spelen!");
                    break;
                }

                switch (input)
                {
                    case "help":
                        Console.WriteLine("Commando's: look, inventory, go n/e/s/w, take <id>, fight, quit");
                        break;

                    case "look":
                        Console.WriteLine(game.CurrentRoom.Describe());
                        break;

                    case "inventory":
                        Console.WriteLine(game.PlayerInventory);
                        break;

                    case string s when s.StartsWith("go "):
                        if (TryParseDirection(s.Split(' ')[1], out var dir))
                            Console.WriteLine(game.Go(dir));
                        else
                            Console.WriteLine("Ongeldige richting.");
                        break;

                    case string s when s.StartsWith("take "):
                        var id = s.Substring(5);
                        Console.WriteLine(game.Take(id));
                        break;

                    case "fight":
                        Console.WriteLine(game.Fight());
                        break;

                    default:
                        Console.WriteLine("Onbekend commando. Typ 'help' voor opties.");
                        break;
                }

            }
        }
        private static bool TryParseDirection(string input, out Direction dir)
        {
            dir = input switch
            {
                "n" or "north" => Direction.North,
                "e" or "east" => Direction.East,
                "s" or "south" => Direction.South,
                "w" or "west" => Direction.West,
                _ => default
            };
            return input is "n" or "north" or "e" or "east" or "s" or "south" or "w" or "west";
        }
    }
}
