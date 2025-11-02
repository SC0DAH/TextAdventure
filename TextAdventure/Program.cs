namespace TextAdventure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Adventure Game!");
            Console.WriteLine("Type 'help' to see available commands.");

            // Setting up the game
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            bool isRunning = true;
            while (isRunning)
            {
                Console.Write("\n> ");
                string? input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input)) continue;

                string[] parts = input.Split(' ', 2);
                string command = parts[0];
                string argument = parts.Length > 1 ? parts[1] : "";

                switch (command)
                {
                    case "help":
                        ShowHelp();
                        break;

                    case "look":
                        DescribeRoom(world);
                        break;

                    case "inventory":
                        Console.WriteLine(inventory.ToString());
                        break;

                    case "go":
                        if (string.IsNullOrEmpty(argument))
                            Console.WriteLine("Go where?");
                        else
                        {
                            string result = world.Go(argument, inventory);
                            Console.WriteLine(result);
                            if (result.Contains("Game Over") || result.Contains("Congratulations")) isRunning = false;
                        }
                        break;

                    case "take":
                        if (string.IsNullOrEmpty(argument))
                            Console.WriteLine("Take what?");
                        else
                        {
                            var item = world.TakeItem(argument, inventory);
                            Console.WriteLine(item != null ? $"You picked up {item.Id}." : "That item isn't here.");
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
        }

        static void DescribeRoom(Rooms world)
        {
            var room = world.CurrentRoom;
            Console.WriteLine($"\nYou are in {room.Name}: {room.Description}");
            if (room.Items.Count > 0)
                Console.WriteLine("Items here: " + string.Join(", ", room.Items.Select(i => i.Id)));
            Console.WriteLine("Exits: " + string.Join(", ", room.Exits.Keys));
        }
    }

}
