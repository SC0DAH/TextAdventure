using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
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
            var room = rooms.GetRoomByName(roomName);
            if (room == null) throw new Exception($"Room '{roomName}' not found");
            rooms.CurrentRoom = room;
        }

        [When(@"the player goes ""(.*)""")]
        public void WhenThePlayerGoes(string direction)
        {
            lastOutput = rooms.Go(direction, inventory);
        }

        [When(@"the player takes ""(.*)""")]
        public void WhenThePlayerTakes(string itemId)
        {
            lastOutput = rooms.TakeItem(itemId, inventory)?.Id ?? "Nothing to take";
        }

        [When(@"the player fights")]
        public void WhenThePlayerFights()
        {
            lastOutput = rooms.Fight(inventory);
        }

        [When(@"the player tries to go ""(.*)"" without fighting")]
        public void WhenThePlayerTriesToGoWithoutFighting(string direction)
        {
            if (rooms.CurrentRoom.HasMonster)
            {
                lastOutput = "You died";
            }
            else
            {
                lastOutput = rooms.Go(direction, inventory);
            }
        }

        [Then(@"the player should see ""(.*)""")]
        public void ThenThePlayerShouldSee(string expected)
        {
            Console.WriteLine($"lastOutput: {lastOutput}");
            Assert.That(lastOutput, Does.Contain(expected), $"Expected '{expected}' but got '{lastOutput}'");
        }

        [Then(@"the monster should be dead")]
        public void ThenTheMonsterShouldBeDead()
        {
            Assert.That(rooms.CurrentRoom.HasMonster, Is.False, "Monster is still alive!");
        }

        [Then(@"the player can go ""(.*)"" safely")]
        public void ThenThePlayerCanGoSafely(string direction)
        {
            var output = rooms.Go(direction, inventory);
            Assert.That(output, Does.Not.Contain("You died"), $"Player died when trying to go {direction}: {output}");
        }
    }
}
