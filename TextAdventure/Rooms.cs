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
        public Rooms(Room startRoom)
        {
            CurrentRoom = startRoom;
        }

        public List<Room> AllRooms { get; } = new List<Room>();

        public Room GetRoomByName(string name) => AllRooms.FirstOrDefault(r => r.Name == name);

        public string Fight(Inventory inventory)
        {
            if(!CurrentRoom.HasMonster)
            {
                return "There's no monster to fight here...";
            }
            if(!inventory.HasItem("sword"))
            {
                return "You came unprepared! The monster kills you. Game Over.";
            }

            CurrentRoom.HasMonster = false;
            return "You sliced up the monster with your sword!";
        }

        public string Go(string direction, Inventory inventory)
        {
            var room = CurrentRoom;

            if (!room.Exits.ContainsKey(direction))
                return "You can't go that way.";

            var nextRoom = room.Exits[direction];

            if (nextRoom.IsDeadly)
            {
                CurrentRoom = nextRoom;
                return $"{nextRoom.Description}\nYou died! Game Over.";
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

            if (room.HasMonster)
            {
                CurrentRoom = nextRoom;
                return "You try to leave while the monster is alive...\n💀 The monster kills you. Game Over.";
            }

            CurrentRoom = nextRoom;
            return $"You move to {nextRoom.Name}.\n{nextRoom.Description}";
        }

        public Item TakeItem(string id, Inventory inventory)
        {
            var room = CurrentRoom;
            var item = room.Items.FirstOrDefault(i => i.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                room.Items.Remove(item);
                inventory.AddItem(item);
            }
            return item;
        }

    }
}
