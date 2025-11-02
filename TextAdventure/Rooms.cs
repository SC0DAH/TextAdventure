using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    public class Rooms
    {
        public Room CurrentRoom { get; set; }
        public List<Room> AllRooms { get; } = new List<Room>();

        public Rooms(Room startRoom)
        {
            CurrentRoom = startRoom;
            AllRooms.Add(startRoom);
        }

        public Room GetRoomByName(string name) => AllRooms.FirstOrDefault(r => r.Name == name);

        // Бой с монстром
        public string Fight(Inventory inventory)
        {
            if (!CurrentRoom.HasMonster)
                return "There's no monster to fight here...";

            if (!inventory.HasItem("sword"))
                return "You came unprepared! The monster kills you. Game Over.";

            CurrentRoom.HasMonster = false;
            return "You sliced up the monster with your sword!";
        }

        public string Go(string direction, Inventory inventory)
        {
            if (!CurrentRoom.Exits.ContainsKey(direction))
                return "You can't go that way.";

            if (CurrentRoom.HasMonster)
                return "You died";

            var nextRoom = CurrentRoom.Exits[direction];
            if (nextRoom.IsDeadly)
            {
                CurrentRoom = nextRoom;
                return $"{nextRoom.Description}\nGame Over";
            }

            if (nextRoom.RequiresKey)
            {
                if (inventory.HasItem("key"))
                {
                    CurrentRoom = nextRoom;
                    return "You unlock the door with your key and step through...\nCongratulations, you win!";
                }
                else
                {
                    return "The door is locked. You need a key!";
                }
            }

            if (nextRoom.HasMonster)
            {
                CurrentRoom = nextRoom;
                return "A monster jumps at you! You can 'fight' or try to go back...";
            }

            CurrentRoom = nextRoom;
            return $"You move to {nextRoom.Name}.\n{nextRoom.Description}";
        }
        public Item TakeItem(string id, Inventory inventory)
        {
            var item = CurrentRoom.Items.FirstOrDefault(i => i.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                CurrentRoom.Items.Remove(item);
                inventory.AddItem(item);
            }
            return item;
        }
    }
}
