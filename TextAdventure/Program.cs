using System.Security.Cryptography;
using System.Text;

namespace TextAdventure
{
    internal class Program
    {
        static string? passphrase = null;

        //  ophalen API (JWT beveiligd)
        static string? keyshare = null;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the Adventure Game!");
            Console.WriteLine("Type 'help' to see available commands.");

 
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            var auth = new AuthClient("http://localhost:5064");

            bool isRunning = true;
            while (isRunning)
            {
                Console.Write("\n> ");
                string? input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input)) continue;

                string[] parts = input.Split(' ', 2);
                string command = parts[0];
                string argument = parts.Length > 1 ? parts[1] : "";

                if (!auth.IsLoggedIn &&
                    command != "login" &&
                    command != "register" &&
                    command != "help" &&
                    command != "quit")
                {
                    Console.WriteLine("You must login first! Available commands: 'help', 'login', 'register', 'quit'.");
                    continue;
                }

                switch (command)
                {
                    case "help":
                        ShowHelp();
                        break;

                    case "register":
                        Console.Write("Username: ");
                        var user = Console.ReadLine()!.Trim();

                        Console.Write("Password: ");
                        var pass = ReadPassword();

                        var success = await auth.RegisterAsync(user, pass);
                        if (success)
                            Console.WriteLine("Registered successfully! Now you can login.");
                        else
                            Console.WriteLine("Registration failed. Try a different username.");
                        break;

                    // JWT token opgeslagen in AuthClient
                    case "login":
                        Console.Write("Username: ");
                        var loguser = Console.ReadLine()!.Trim();

                        Console.Write("Password: ");
                        var logpass = ReadPassword();

                        var loginSuccess = await auth.LoginAsync(loguser, logpass);
                        if (!loginSuccess)
                        {
                            Console.WriteLine("Login failed. Check your credentials.");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Login succesfull");
                            Console.ResetColor();
                        }
                        break;

                    case "unlock":
                        Console.Write("Enter passphrase: ");
                        passphrase = ReadPassword();

                        if (string.IsNullOrWhiteSpace(passphrase))
                        {
                            Console.WriteLine("Passphrase cannot be empty.");
                            passphrase = null;
                        }
                        else
                        {
                            Console.WriteLine("Passphrase stored.");
                        }
                        break;

                    // ophalen via API (JWT nodig)
                    case "keyshare":
                        keyshare = await auth.GetKeyShareAsync();
                        if (keyshare != null)
                            Console.WriteLine("Your secret key received.");
                        break;

                    case "look":
                        DescribeRoom(world);
                        break;

                    case "inventory":
                        Console.WriteLine(inventory.ToString());
                        break;

                    case "go":
                        if (string.IsNullOrEmpty(argument))
                        {
                            Console.WriteLine("Go where?");
                        }
                        else
                        {
                            if (world.CurrentRoom.Exits.TryGetValue(argument, out var targetRoom)
                                && (targetRoom.Name == "Key-Room" || targetRoom.Name == "Start-Room"))
                            {
                                if (!TryGenerateDecryptionKey(keyshare, passphrase, out _))
                                {
                                    Console.WriteLine("The door is encrypted.");
                                    break;
                                }

                                Console.WriteLine("Decryption successful. The room is now accessible.");
                            }

                            string result = world.Go(argument, inventory);
                            Console.WriteLine(result);

                            if (result.Contains("Game Over") || result.Contains("Congratulations"))
                                isRunning = false;
                        }
                        break;


                    case "take":
                        if (string.IsNullOrEmpty(argument))
                            Console.WriteLine("Take what?");
                        else
                        {
                            var item = world.TakeItem(argument, inventory);
                            Console.WriteLine(item != null
                                ? $"You picked up {item.Id}."
                                : "That item isn't here.");
                        }
                        break;

                    case "fight":
                        Console.WriteLine(world.Fight(inventory));
                        break;

                    case "quit":
                        isRunning = false;
                        Console.WriteLine("Thanks for playing!");
                        break;

                    default:
                        Console.WriteLine("Unknown command. Type 'help' for list of commands.");
                        break;
                }
            }
        }

        // SHA256( keyshare + ":" + passphrase )
        static bool TryGenerateDecryptionKey(
            string? keyshare,
            string? passphrase,
            out byte[] derivedKey)
        {
            derivedKey = Array.Empty<byte>();

            if (string.IsNullOrWhiteSpace(keyshare) || string.IsNullOrWhiteSpace(passphrase))
                return false;

            try
            {
                var combined = $"{keyshare}:{passphrase}";
                using var sha = SHA256.Create();
                derivedKey = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return true;
            }
            catch
            {
                return false;
            }
        }


        static void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("help - show this list");
            Console.WriteLine("look - describe the current room");
            Console.WriteLine("inventory - show your items");
            Console.WriteLine("go <direction> - move to another room (left/right/up/down)");
            Console.WriteLine("take <item> - pick up an item in the room");
            Console.WriteLine("fight - fight the monster (if there is one)");
            Console.WriteLine("quit - end the game");
            Console.WriteLine("login - login to the server and get your JWT token");
            Console.WriteLine("unlock - enter your passphrase to unlock secured rooms");
            Console.WriteLine("register - create an account");
            Console.WriteLine("keyshare - request your unique key from the API (requires login)");
        }

        static void DescribeRoom(Rooms world)
        {
            var room = world.CurrentRoom;
            Console.WriteLine($"\nYou are in {room.Name}: {room.Description}");
            if (room.Items.Count > 0)
                Console.WriteLine("Items here: " + string.Join(", ", room.Items.Select(i => i.Id)));
            Console.WriteLine("Exits: " + string.Join(", ", room.Exits.Keys));
        }

        static string ReadPassword()
        {
            var pass = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass.Remove(pass.Length - 1, 1);
                    continue;
                }
                pass.Append(key.KeyChar);
            }
            return pass.ToString();
        }
    }
}
