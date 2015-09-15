using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Roetur.Core.Tests
{
    [TestClass]
    public class RoeturCollectionSpecification
    {
        [TestInitialize]
        public void Initialise()
        {
            Roetur.Routes = new List<Tuple<RoetingRule, Func<RoeturContext, Task>>>();
        }

        [TestMethod]
        public void Simple_routes_are_added_to_collection()
        {
            Roetur.Add("/", context => Task.Factory.StartNew(()=> {}));
            Assert.AreEqual(1, Roetur.Routes.Count());
        }

        [TestMethod]
        public void Complex_routes_are_added_to_collection()
        {
            Roetur.Add("/awdawdad/:id", context => Task.Factory.StartNew(id => { }, context.Param<int>(":id")));
            Assert.AreEqual(1, Roetur.Routes.Count());
        }
    }
}
