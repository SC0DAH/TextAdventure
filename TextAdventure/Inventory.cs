using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class Inventory
    {
        private List<Item> Items { get; set; } = new List<Item>();

        public void AddItem(Item item)
        {
            Items.Add(item);
        }
        public bool HasItem(string id)
        {
            return Items.Any(i => i.Id == id);
        }

        public override string ToString()
        {
            return Items.Count == 0 ? "Your inventory is empty." : "Your inventory contains: " + string.Join(", ", Items.Select(i => i.Id));
        }
    }
}
