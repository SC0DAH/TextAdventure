using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure;
namespace UnitTests
{
    [TestClass]
    public class GameSetupTests
    {
        [TestMethod]
        public void CreateGame_ShouldReturnStartRoom()
        {
            // Act
            var rooms = GameSetup.CreateGame();

            // Assert
            Assert.IsNotNull(rooms);
            Assert.AreEqual("Start-Room", rooms.CurrentRoom.Name);
        }

        [TestMethod]
        public void CreateGame_ShouldHaveConnections()
        {
            var rooms = GameSetup.CreateGame();
            var start = rooms.CurrentRoom;

            Assert.IsTrue(start.Exits.ContainsKey("left"));
            Assert.IsTrue(start.Exits.ContainsKey("right"));
            Assert.IsTrue(start.Exits.ContainsKey("up"));
            Assert.IsTrue(start.Exits.ContainsKey("down"));
        }
    }
}
