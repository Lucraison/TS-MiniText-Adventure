using System;
using TextAdventure.Domain;
using TextAdventure.Gameplay;
using TextAdventure.Security;

namespace TextAdventure;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var api = new ApiClient("https://localhost:7007"); // jouw API draait op 7007 (HTTPS)

        string jwt = "";
        string role = "";
        bool noclip = false;
       


        // ========== LOGIN ==========
        while (true)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Password: ");
            var password = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username en password zijn verplicht.");
                continue;
            }

            var login = api.LoginAsync(username, password).GetAwaiter().GetResult();
            if (login == null)
            {
                Console.WriteLine("Login mislukt of API is niet bereikbaar. Probeer opnieuw.");
                continue;
            }

            jwt = login.Token;
            role = login.User.Role; // laat zo als jouw DTO dit ondersteunt

            Console.WriteLine($"Ingelogd als {username} ({role}).");
            break;
        }

        // ========== GAME SETUP ==========
        var world = GameSetup.BuildWorld();
        var inventory = new Inventory();

        string? passphrase = null;

       
        
        // ========== ENCRYPTED ROOM OPEN ==========
        string? TryOpenEncryptedRoom(string roomId)
        {
            // 1) Moet ingelogd zijn
            if (string.IsNullOrWhiteSpace(jwt)) return null;

            // 2) Passphrase moet gevonden zijn in de game
            if (string.IsNullOrWhiteSpace(passphrase)) return null;

            // 3) Keyshare ophalen via API (met JWT)
            var keyshare = api.GetKeyshareAsync(roomId, jwt).GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(keyshare)) return null;

            // 4) Derived key maken volgens opdracht: SHA256(keyshare + ":" + passphrase)
            var derived = CryptoHelpers.Sha256Hex($"{keyshare}:{passphrase}").ToUpperInvariant();


            // 5) Bestanden bepalen: room_secret1.enc/pfx en room_secret2.enc/pfx
            var encPath = Path.Combine(AppContext.BaseDirectory, "EncryptedRooms", $"room_{roomId}.enc");
            var pfxPath = Path.Combine(AppContext.BaseDirectory, "EncryptedRooms", $"room_{roomId}.pfx");

            if (!File.Exists(encPath) || !File.Exists(pfxPath)) return null;

            // 6) Decrypt proberen zonder crash
            if (CmsDecryptor.TryDecryptToString(encPath, pfxPath, derived, out var plaintext))
                return plaintext;

            return null;
        }

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
                      noclip      — (Admin) toggle noclip (autorisatie demo)
                      quit        — stop het spel
                    """);
                    break;

                case "look":
                    {
                        var desc = world.DescribeCurrent(withExits: true, withItems: true, withMonster: true);
                        Console.WriteLine(desc);

                        // Robust: bepaal secret room op basis van room name
                        string? roomId = world.Current.Name switch
                        {
                            "Secret1" => "secret1",
                            "Secret2" => "secret2",
                            _ => null
                        };

                        if (roomId != null)
                        {
                            if (string.IsNullOrWhiteSpace(passphrase))
                            {
                                Console.WriteLine("\nJe voelt dat hier iets verborgen zit, maar je mist nog een aanwijzing...");
                            }
                            else
                            {
                                var plain = TryOpenEncryptedRoom(roomId);
                                if (plain == null)
                                    Console.WriteLine("\nDe inhoud blijft onleesbaar. (Geen toegang of verkeerde sleutel)");
                                else
                                    Console.WriteLine("\n[Verborgen inhoud]\n" + plain);
                            }
                        }

                        break;
                    }

                case "inventory":
                    Console.WriteLine(inventory.Describe());
                    break;

                case "noclip":
                    if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Geen toegang.");
                        break;
                    }
                    noclip = !noclip;
                    Console.WriteLine(noclip ? "Noclip AAN." : "Noclip UIT.");
                    break;

                case "go":
                    if (string.IsNullOrWhiteSpace(arg)) { Console.WriteLine("Gebruik: go n|e|s|w"); break; }
                    if (!DirectionHelpers.TryParse(arg, out var dir))
                    { Console.WriteLine("Onbekende richting. Gebruik: n, e, s of w."); break; }

                    // (Optioneel) als je later noclip echt wil gebruiken om bv. key-requirements te omzeilen,
                    // moet dat in World.Go zelf, niet hier. Nu blijft dit enkel een autorisatie-demo toggle.

                    var move = world.Go(dir, inventory, noclip);
                    
                    Console.WriteLine(move.Message);
                    if (move.EndState != EndState.None) running = false;
                    break;

                case "take":
                    if (string.IsNullOrWhiteSpace(arg)) { Console.WriteLine("Gebruik: take <id>"); break; }

                    var takeMsg = world.TakeFromCurrent(arg, inventory);
                    Console.WriteLine(takeMsg);

                    // Passphrase niet hardcoded: uit de note-description halen
                    if (arg.Equals("note", StringComparison.OrdinalIgnoreCase))
                    {
                        var note = inventory.Get("note"); // vereist Inventory.Get(...)
                        if (note != null)
                        {
                            var text = note.Description;
                            var first = text.IndexOf('"');
                            var last = text.LastIndexOf('"');

                            passphrase = (first >= 0 && last > first)
                                ? text.Substring(first + 1, last - first - 1)
                                : text;

                            Console.WriteLine("Je hebt een passphrase gevonden.");
                        }
                    }
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
