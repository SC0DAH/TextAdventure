using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure;

namespace UnitTests
{
    [TestClass]
    [TestCategory("Items")]
    public class ItemTests
    {
        [TestMethod]
        public void Item_ShouldStoreIdAndDescription()
        {
            var id = "sword";
            var description = "A sharp blade.";

            var item = new Item(id, description);

            Assert.AreEqual(id, item.Id);
            Assert.AreEqual(description, item.Description);
        }
    }
}
