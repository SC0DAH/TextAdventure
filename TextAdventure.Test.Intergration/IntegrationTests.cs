using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextAdventure;

namespace TextAdventure.Test.Integration
{
    /// <summary>
    /// Integratietesten: test de ECHTE game flow zoals in Program.cs
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class IntegrationTests
    {
        // TEST 1: Win scenario - pak key en ga door deur
        [TestMethod]
        public void CompleteGame_WinScenario()
        {
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // Speelt het winnende pad
            world.Go("right", inventory);              // Ga naar Key-Room
            world.TakeItem("key", inventory);          // Pak key
            Assert.IsTrue(inventory.HasItem("key"), "Should have key in inventory after taking it"); // Controleer of key in inventory zit
            world.Go("left", inventory); // Ga naar Door-Room
            var winResult = world.Go("up", inventory); // Ga naar Door-Room


            // ASSERT: Check de ECHTE output van je game
            Assert.IsTrue(winResult.Contains("Congratulations"), "Should see Congratulations message");
            Assert.IsTrue(winResult.Contains("win"), "Should see win message");
        }

        // TEST 2: Deadly room = Game Over
        [TestMethod]
        public void EnterDeadlyRoom_ShouldDie()
        {
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            var result = world.Go("left", inventory);

            // Check de echte output
            Assert.IsTrue(result.Contains("Game Over"), "Should see Game Over");
            Assert.AreEqual("Deadly-Room", world.CurrentRoom.Name, "Should be in Deadly-Room");
        }

        // TEST 3: Monster verslaan met zwaard
        [TestMethod]
        public void FightMonster_WithSword_ShouldWin()
        {
            // ARRANGE
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // ACT
            world.Go("down", inventory);            // Naar Sword-Room
            world.TakeItem("sword", inventory);     // Pak sword
            world.Go("down", inventory);            // Naar Monster-Room
            var fightResult = world.Fight(inventory);

            // ASSERT: Check wat Fight() echt returned
            Assert.IsTrue(fightResult.Contains("sliced"), "Should see sliced message");
            Assert.IsFalse(world.CurrentRoom.HasMonster, "Monster should be dead");
        }

        // TEST 4: Monster bevechten zonder zwaard
        [TestMethod]
        public void FightMonster_WithoutSword_ShouldDie()
        {
            // ARRANGE
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // ACT
            world.Go("down", inventory);  // Sword-Room
            world.Go("down", inventory);  // Monster-Room
            var result = world.Fight(inventory);

            // ASSERT: Check de ECHTE fight output
            Assert.IsTrue(result.Contains("Game Over"), "Should see Game Over");
            Assert.IsTrue(world.CurrentRoom.HasMonster, "Monster should still be alive");
        }

        // TEST 5: Deur zonder key
        [TestMethod]
        public void TryDoor_WithoutKey_ShouldBeBlocked()
        {
            // ARRANGE
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // ACT
            var result = world.Go("up", inventory);

            // Checken wat de game echt zegt
            Assert.IsTrue(result.Contains("locked"), "Should see door is locked");
            Assert.AreEqual("Start-Room", world.CurrentRoom.Name, "Should still be in Start-Room");
        }

        // TEST 6: Monster room verlaten terwijl monster leeft
        [TestMethod]
        public void LeaveMonsterRoom_WhileMonsterAlive_ShouldDie()
        {
            // ARRANGE
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // ACT
            world.Go("down", inventory);  // Sword-Room
            world.Go("down", inventory);  // Monster-Room 
            var result = world.Go("up", inventory);  // probere vluchten

            // "died" of huidige output "kills you"/"Game Over"
            Assert.IsTrue(result.Contains("died") || result.Contains("Game Over"), $"Expected death message but it gave {result}");
        }


        // TEST 7: 
        [TestMethod]
        public void DefeatMonster_ThenLeave_ShouldSucceed()
        {
            // ARRANGE
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // ACT
            world.Go("down", inventory);            // Sword-Room
            world.TakeItem("sword", inventory);     // Pak sword
            world.Go("down", inventory);            // Monster-Room
            world.Fight(inventory);                 // Versla monster
            var result = world.Go("up", inventory); // Ga terug

            // ASSERT
            Assert.IsFalse(result.Contains("died"), "Should not die because defeated monster");
            Assert.AreEqual("Sword-Room", world.CurrentRoom.Name, "Should be in Sword-Room");
        }

        // TEST 8: Item pakken verwijdert het uit kamer
        [TestMethod]
        public void TakeItem_ShouldMoveFromRoomToInventory()
        {
            // ARRANGE
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // ACT
            world.Go("right", inventory);  // Key-Room
            var keyRoomItemsBefore = world.CurrentRoom.Items.Count;
            world.TakeItem("key", inventory);

            // ASSERT
            Assert.IsTrue(inventory.HasItem("key"), "Key should be in inventory");
            Assert.AreEqual(keyRoomItemsBefore - 1, world.CurrentRoom.Items.Count, "Key should be removed from room");
        }

        // TEST 9: Ongeldige richting
        [TestMethod]
        public void Go_InvalidDirection_ShouldStayInRoom()
        {
            // ARRANGE
            var world = GameSetup.CreateGame();
            var inventory = new Inventory();

            // ACT
            var result = world.Go("north", inventory);  // Start-Room heeft geen north

            // ASSERT
            Assert.IsTrue(result.Contains("can't go"), "Should say can't go that way");
            Assert.AreEqual("Start-Room", world.CurrentRoom.Name, "Should still be in Start-Room");
        }
    }
}