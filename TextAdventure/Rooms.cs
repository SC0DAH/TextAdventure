using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class Rooms
    {
        public Room CurrentRoom { get; private set; }
        public Rooms(Room startRoom)
        {
            CurrentRoom = startRoom;
        }

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
    }
}
