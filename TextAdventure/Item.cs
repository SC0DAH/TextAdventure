using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class Item
    {
        public string Id { get; }
        public string Description { get; }

        public Item(string id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
