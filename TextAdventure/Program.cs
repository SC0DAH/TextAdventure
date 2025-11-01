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
                        if (string.IsNullOrEmpty(argument)) Console.WriteLine("Go where?");
                        else Move(argument, world, inventory, ref isRunning);
                        break;

                    case "take":
                        if (string.IsNullOrEmpty(argument)) Console.WriteLine("Take what?");
                        else TakeItem(argument, world, inventory);
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

        static void TakeItem(string id, Rooms world, Inventory inventory)
        {
            var room = world.CurrentRoom;
            var item = room.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                Console.WriteLine("That item isn't here.");
                return;
            }

            room.Items.Remove(item);
            inventory.AddItem(item);
        }

        static void Move(string dir, Rooms world, Inventory inventory, ref bool running)
        {
            var room = world.CurrentRoom;
            if (!room.Exits.ContainsKey(dir))
            {
                Console.WriteLine("You can't go that way.");
                return;
            }

            var nextRoom = room.Exits[dir];

            // Checking if room is deadly
            if (nextRoom.IsDeadly)
            {
                Console.WriteLine(nextRoom.Description);
                Console.WriteLine("💀 You died! Game Over.");
                running = false;
                return;
            }

            // checking if room requires a key and if the player has key
            if (nextRoom.RequiresKey)
            {
                if (inventory.HasItem("key"))
                {
                    Console.WriteLine("You unlock the door with your key and step through...");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Congratulations, you win!");
                    Console.ResetColor();
                    running = false;
                }
                else
                {
                    Console.WriteLine("The door is locked. You need a key!");
                }
                return;
            }

            // checking if room has monster
            if (nextRoom.HasMonster)
            {
                Console.WriteLine("A monster jumps at you!");
                Console.WriteLine("You can 'fight' or try to go back...");
            }

            // if player leaves without killing monster he dies
            if (room.HasMonster)
            {
                Console.WriteLine("You try to leave while the monster is alive...");
                Console.WriteLine("💀 The monster kills you. Game Over.");
                running = false;
                return;
            }

            world.GetType().GetProperty("CurrentRoom")!
                .SetValue(world, nextRoom);

            Console.WriteLine($"You move to {nextRoom.Name}.");
            Console.WriteLine(nextRoom.Description);
        }
    }
}
