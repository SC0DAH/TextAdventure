using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure;

namespace UnitTests
{
    [TestClass]
    public class RoomTests
    {
        [TestMethod]
        public void Room_ShouldInitializeWithCorrectValues()
        {
            var room = new Room("TestRoom", "A test room");

            Assert.AreEqual("TestRoom", room.Name);
            Assert.AreEqual("A test room", room.Description);
            Assert.IsFalse(room.IsDeadly);
            Assert.IsFalse(room.RequiresKey);
            Assert.IsFalse(room.HasMonster);
        }

        [TestMethod]
        public void Room_ShouldAddItemAndExit()
        {
            var room1 = new Room("Room1", "First room");
            var room2 = new Room("Room2", "Second room");
            var item = new Item("key", "Golden key");

            room1.Items.Add(item);
            room1.Exits.Add("north", room2);

            Assert.AreEqual(1, room1.Items.Count);
            Assert.IsTrue(room1.Exits.ContainsKey("north"));
        }
    }
}
