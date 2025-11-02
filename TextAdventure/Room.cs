using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    public class Room
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsDeadly { get; set; }
        public bool RequiresKey { get; set; }
        public bool HasMonster { get; set; }
        public List<Item> Items { get; } = new();

        public Dictionary<string, Room> Exits { get; } = new();

        public Room(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
