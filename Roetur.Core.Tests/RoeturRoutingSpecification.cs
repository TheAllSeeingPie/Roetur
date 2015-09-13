using System;
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
            Roetur.Routes.Clear();
        }

        [TestMethod]
        public async Task Simple_routes_are_routed()
        {
            var success = false;
            Roetur.AddRoet("/", c => Task.Factory.StartNew(() => { success = true; }));
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
            Roetur.AddRoet("/:id", c => Task.Factory.StartNew(() =>
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
            Roetur.AddRoet("/:id/:someguid", c => Task.Factory.StartNew(() =>
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
    }
}