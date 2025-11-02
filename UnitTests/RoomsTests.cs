using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure;
namespace UnitTests
{
    [TestClass]
    public class RoomsTests
    {
        [TestMethod]
         public void Fight_ShouldReturnNoMonsterMessage_IfNoMonster()
         {
                var room = new Room("Test", "No monster here");
                var rooms = new Rooms(room);
                var inventory = new Inventory();

                var result = rooms.Fight(inventory);

                Assert.AreEqual("There's no monster to fight here...", result);
         }

         [TestMethod]
         public void Fight_ShouldKillPlayer_IfNoSword()
         {
                var room = new Room("Monster-Room", "Scary!") { HasMonster = true };
                var rooms = new Rooms(room);
                var inventory = new Inventory();

                var result = rooms.Fight(inventory);

                Assert.AreEqual("You came unprepared! The monster kills you. Game Over.", result);
         }

         [TestMethod]
         public void Fight_ShouldKillMonster_IfSwordInInventory()
         {
                var room = new Room("Monster-Room", "Scary!") { HasMonster = true };
                var rooms = new Rooms(room);
                var inventory = new Inventory();
                inventory.AddItem(new Item("sword", "Sharp sword"));

                var result = rooms.Fight(inventory);

                Assert.AreEqual("You sliced up the monster with your sword!", result);
                Assert.IsFalse(room.HasMonster);
         }
    }
}

