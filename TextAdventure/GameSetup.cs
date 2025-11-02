using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    public class GameSetup
    {
        // Methode om game te initialiseren
        public static Rooms CreateGame()
        {
            // Alle rooms aanmaken
            var startRoom = new Room("Start-Room", "You are at the very beginning of your adventure!");
            var keyRoom = new Room("Key-Room", "The room with a secret key!");
            var deadlyRoom = new Room("Deadly-Room", "You fell into a deadly trap :c") { IsDeadly = true };
            var doorRoom = new Room("Door-Room", "A room that needs a key (╭ರ_•́)") { RequiresKey = true };
            var swordRoom = new Room("Sword-Room", "A room that has a sharp sword laying around!");
            var monsterRoom = new Room("Monster-Room", "The room that is haunted by a devious monster!") { HasMonster = true };

            // Rooms met items => items geven
            var key = new Item("key", "A shiny golden key");
            var sword = new Item("sword", "A sharp sword ready for action");

            keyRoom.Items.Add(key);
            swordRoom.Items.Add(sword);

            // Exits instellen van start room
            startRoom.Exits.Add("left", deadlyRoom);
            startRoom.Exits.Add("right", keyRoom);
            startRoom.Exits.Add("up", doorRoom);
            startRoom.Exits.Add("down", swordRoom);
            // Exits instellen voor andere rooms
            keyRoom.Exits.Add("left", startRoom);
            doorRoom.Exits.Add("down", startRoom);
            swordRoom.Exits.Add("up", startRoom);
            swordRoom.Exits.Add("down", monsterRoom);
            monsterRoom.Exits.Add("up", swordRoom);
            // we zetten currentroom als startRoom
            return new Rooms(startRoom);
        }
    }
}
