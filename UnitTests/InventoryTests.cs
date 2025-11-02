using TextAdventure; 

namespace UnitTests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void AddItem_ShouldAddNewItem()
        {
            // Arrange
            var inventory = new Inventory();
            var item = new Item("key", "Golden key");

            // Act
            inventory.AddItem(item);

            // Assert
            Assert.IsTrue(inventory.HasItem("key"));
        }

        [TestMethod]
        public void AddItem_ShouldNotAddDuplicate()
        {
            // Arrange
            var inventory = new Inventory();
            var item = new Item("key", "Golden key");
            inventory.AddItem(item);

            // Act
            inventory.AddItem(item);

            // Assert
            Assert.IsTrue(inventory.HasItem("key"));
            var result = inventory.ToString();
            Assert.IsFalse(result.Contains("key, key"));
        }

        [TestMethod]
        public void RemoveItem_ShouldRemoveExistingItem()
        {
            var inventory = new Inventory();
            var item = new Item("sword", "Sharp sword");
            inventory.AddItem(item);

            // Act
            inventory.RemoveItem("sword");

            // Assert
            Assert.IsFalse(inventory.HasItem("sword"));
        }

        [TestMethod]
        public void ToString_ShouldReturnEmptyMessage_WhenInventoryIsEmpty()
        {
            // Arrange
            var inventory = new Inventory();

            // Act
            var result = inventory.ToString();

            // Assert
            Assert.AreEqual("Your inventory is empty.", result);
        }
    }
}
