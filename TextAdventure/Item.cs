using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    public class Item
    {
        public string Id { get; private set; }
        public string Description { get; private set; }

        public Item(string id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
