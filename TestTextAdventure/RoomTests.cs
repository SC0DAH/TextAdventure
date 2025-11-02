using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextAdventure;

namespace TestTextAdventure
{
    //samengevat
    /// Unit tests voor de Room class
    /// Hier test ik of Room correct werkt

    [TestClass]
    [TestCategory("Room")]

    public class RoomTests
    {
        // TEST 1: Checkt of de constructor (new Room(...)) werkt
        [TestMethod]  
        public void Room_Constructor_ShouldSetNameAndDescription()
        {
            var name = "Start Room";
            var description = "You are at the very beginning of your adventure!";

            var room = new Room(name, description);

            Assert.AreEqual("Start Room", room.Name);
            Assert.AreEqual("You are at the very beginning of your adventure!", room.Description);
        }

        // TEST 2: Checkt of Items lijst leeg start
        [TestMethod]
        public void Room_Items_ShouldBeEmptyAtStart()
        {
            var room = new Room("Test Room", "A test room");

            // Check dat Items lijst bestaat EN leeg is
            Assert.IsNotNull(room.Items);  // Lijst bestaat
            Assert.AreEqual(0, room.Items.Count);  // Lijst heeft 0 items
        }

        // TEST 3: Checkt of je items kan toevoegen en verwijderen
        [TestMethod]
        public void Room_Items_CanAddAndRemoveItems()
        {
            // ARRANGE
            var room = new Room("Treasure Room", "Full of treasure");
            var item = new Item("key", "A sharp sword ready for action");

            // ACT - Voeg item toe
            room.Items.Add(item);

            // Chec k dat item is togevoegd
            Assert.AreEqual(1, room.Items.Count);
            Assert.AreEqual("key", room.Items[0].Id);

            room.Items.Remove(item);

            // Check dat item is verwijderd
            Assert.AreEqual(0, room.Items.Count);
        }

        // TEST 4: Checkt of kamers verbonden kunnen worden met exits
        [TestMethod]
        public void Room_Exits_CanConnectRooms()
        {
            // ARRANGE
            var room1 = new Room("Room 1", "First room");
            var room2 = new Room("Room 2", "Second room");

            // Verbind room1 naar room2 via "north"
            room1.Exits.Add("north", room2);

            Assert.AreEqual(1, room1.Exits.Count);
            Assert.IsTrue(room1.Exits.ContainsKey("north"));
            Assert.AreSame(room2, room1.Exits["north"]);  
            // AreSame betekent exact hetzelfde object
        }

        // TEST 5: Checkt of IsDeadly standaard false is
        [TestMethod]
        public void Room_IsDeadly_DefaultsToFalse()
        {
            var room = new Room("Safe Room", "A safe place");

            // IsDeadly moet false zijn als ge het niet instelt
            Assert.IsFalse(room.IsDeadly);
        }

        // TEST 6: Checkt of je IsDeadly op true kan zetten
        [TestMethod]
        public void Room_IsDeadly_CanBeSetToTrue()
        {
            var room = new Room("Trap Room", "Deadly trap!")
            {
                IsDeadly = true
            };

            Assert.IsTrue(room.IsDeadly);
        }

        // TEST 7: Checkt of RequiresKey standaard false is
        [TestMethod]
        public void Room_RequiresKey_DefaultsToFalse()
        {
            var room = new Room("Normal Room", "Just a room");

            Assert.IsFalse(room.RequiresKey);
        }

        // TEST 8: Checkt of je RequiresKey op true kan zetten
        [TestMethod]
        public void Room_RequiresKey_CanBeSetToTrue()
        {
            var room = new Room("Locked Room", "Needs a key")
            {
                RequiresKey = true
            };

            Assert.IsTrue(room.RequiresKey);
        }

        // TEST 9: Checkt of HasMonster standaard false is
        [TestMethod]
        public void Room_HasMonster_DefaultsToFalse()
        {
            var room = new Room("Empty Room", "No monsters here");

            Assert.IsFalse(room.HasMonster);
        }

        [TestMethod]
        public void Room_HasMonster_CanBeSetToTrue()
        {
            var room = new Room("Monster Room", "Scary monster!")
            {
                HasMonster = true
            };

            Assert.IsTrue(room.HasMonster);
        }
    }
}