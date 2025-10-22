using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class GameSetup
    {
        // Methode om game te initialiseren
        public static Rooms CreateGame()
        {
            // Alle rooms aanmaken
            var startRoom = new Room("Start-Room", "You are at the very beginning of your adventure!");
            var keyRoom = new Room("Key-Room", "The room with a secret key!");
            var deadlyRoom = new Room("Deadly-Room", "You fell into a deadly trap :c");
            var doorRoom = new Room("Door-Room", "A room that needs a key (╭ರ_•́)");
            var swordRoom = new Room("Sword-Room", "A room that has a sharp sword laying around!");
            var monsterRoom = new Room("Monster-Room", "The room that is haunted by a devious monster!");

            // Rooms met items => items geven
            var key = new Item("key", "A shiny golden key");
            var sword = new Item("sword", "A sharp sword ready for action");

            keyRoom.Items.Add(key);
            swordRoom.Items.Add(sword);
            // we zetten currentroom als start room
            return new Rooms(startRoom);
        }
    }
}
