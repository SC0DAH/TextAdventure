using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextAdventure;

namespace TextAdventureTests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        public void Item_ShouldBeStoringIdAndDescription()
        {
            // Arrange
            var id = "sword";
            var description = "A sharp blade.";

            // Act
            var item = new Item(id, description);

            // Assert
            Assert.AreEqual(id, item.Id);
            Assert.AreEqual(description, item.Description);
        }
    }
}
