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

        // Voeg een item toe aan de inventaris
        public void AddItem(Item item)
        {
            if (Items.Any(i => i.Id == item.Id))
            {
                Console.WriteLine($"You already have the {item.Description}.");
            }
            else
            {
                Items.Add(item);
                Console.WriteLine($"You picked up the {item.Description}.");
            }
        }
        public bool HasItem(string id)
        {
            return Items.Any(i => i.Id == id);
        }
        // Verwijder een item uit de inventaris
        public void RemoveItem(string id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                Items.Remove(item);
                Console.WriteLine($"You used the {item.Description}.");
            }
            else
            {
                Console.WriteLine("You don't have that item.");
            }
        }

        public override string ToString()
        {
            return Items.Count == 0 ? "Your inventory is empty." : "Your inventory contains: " + string.Join(", ", Items.Select(i => i.Id));
        }
    }
}
