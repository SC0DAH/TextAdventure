using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdventure;

namespace UnitTests
{
    public class ItemsTest
    {
        [TestClass]
        public class ItemTests
        {
            [TestMethod]
            public void Item_ShouldInitializeCorrectly()
            {
                var item = new Item("key", "Golden key");

                Assert.AreEqual("key", item.Id);
                Assert.AreEqual("Golden key", item.Description);
            }
        }
    }
}
