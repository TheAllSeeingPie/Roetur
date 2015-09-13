using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Roetur.Core.Tests
{
    [TestClass]
    public class RoeturRoutingSpecification
    {
        [TestInitialize]
        public void Initialise()
        {
            Roetur.Routes = new List<Tuple<Regex, Func<RoetContext, Task>>>();
        }

        [TestMethod]
        public async Task Simple_routes_are_routed()
        {
            var success = false;
            Roetur.Add("/", c => Task.Factory.StartNew(() => { success = true; }));
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/")
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest
            };

            await Roetur.Invoke(context);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task Complex_routes_with_ints_are_routed()
        {
            var success = false;
            Roetur.Add("/:id", c => Task.Factory.StartNew(() =>
            {
                Assert.AreEqual(1, c.Param<int>(":id"));
                success = true;
            }));
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/1")
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest
            };

            await Roetur.Invoke(context);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task Complex_routes_with_ints_and_guids_are_routed()
        {
            var guid = Guid.NewGuid();
            var success = false;
            Roetur.Add("/:id/:someguid", c => Task.Factory.StartNew(() =>
            {
                Assert.AreEqual(1, c.Param<int>(":id"));
                Assert.AreEqual(guid, c.Param<Guid>(":someguid"));
                success = true;
            }));
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri($"http://localhost/1/{guid}")
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest
            };

            await Roetur.Invoke(context);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task Routes_are_executed_in_correct_order()
        {
            var success = false;
            Roetur.Add("/", c=> Task.Factory.StartNew(()=> Assert.Fail()));
            Roetur.Add("/:id", c => Task.Factory.StartNew(() =>
            {
                Assert.AreEqual(1, c.Param<int>(":id"));
                success = true;
            }));
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/1")
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest
            };

            await Roetur.Invoke(context);

            Assert.IsTrue(success);
        }
    }
}