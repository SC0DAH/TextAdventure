using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TextAdventure;

namespace BehaviorDrivenTesting
{
    
    [Binding]
    internal class MiniTextAdventureSteps
    {
        private Rooms rooms;
        private Inventory inventory;
        private string lastOutput;

        [Given(@"the game is started")]
        public void GivenTheGameIsStarted()
        {
            rooms = GameSetup.CreateGame();
            inventory = new Inventory();
        }

        [Given(@"the player is in the (.*)")]
        public void GivenThePlayerIsInRoom(string roomName)
        {
            rooms.CurrentRoom = rooms.GetRoomByName(roomName); // voeg helper GetRoomByName toe
        }

        [When(@"the player goes ""(.*)""")]
        public void WhenThePlayerGoes(string direction)
        {
            lastOutput = rooms.Go(direction, inventory); // je Go() returnt een string met status
        }

        [When(@"the player takes ""(.*)""")]
        public void WhenThePlayerTakes(string itemId)
        {
            var item = rooms.TakeItem(itemId, inventory);
            if (item != null) inventory.AddItem(item);
        }

        [When(@"the player fights")]
        public void WhenThePlayerFights()
        {
            lastOutput = rooms.Fight(inventory);
        }

        [When(@"the player tries to go ""(.*)"" without fighting")]
        public void WhenThePlayerTriesToGoWithoutFighting(string direction)
        {
            lastOutput = rooms.Go(direction, inventory);
        }

        [Then(@"the player should see ""(.*)""")]
        public void ThenThePlayerShouldSee(string expected)
        {
            Assert.IsTrue(lastOutput.Contains(expected));
        }

        [Then(@"the monster should be dead")]
        public void ThenTheMonsterShouldBeDead()
        {
            Assert.IsFalse(rooms.CurrentRoom.HasMonster);
        }

        [Then(@"the player can go ""(.*)"" safely")]
        public void ThenThePlayerCanGoSafely(string direction)
        {
            var output = rooms.Go(direction, inventory);
            Assert.IsFalse(output.Contains("You died"));
        }
    }
}
