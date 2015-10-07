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
            Router.Routes = new List<Tuple<RoutingRule, Func<RouterContext, Task>>>();
        }

        [TestMethod]
        public void Simple_routes_are_added_to_collection()
        {
            Router.Add("/", context => Task.Factory.StartNew(()=> {}));
            Assert.AreEqual(1, Router.Routes.Count());
        }

        [TestMethod]
        public void Complex_routes_are_added_to_collection()
        {
            Router.Add("/awdawdad/:id", context => Task.Factory.StartNew(id => { }, context.Param<int>(":id")));
            Assert.AreEqual(1, Router.Routes.Count());
        }
    }
}
