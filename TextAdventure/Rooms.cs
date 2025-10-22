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
    }
}
