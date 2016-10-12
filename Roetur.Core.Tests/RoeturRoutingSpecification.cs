using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Roetur.Core.Tests
{
    [TestClass]
    public class RoeturRoutingSpecification
    {
        [TestInitialize]
        public void Initialise()
        {
            Router.Routes = new List<Tuple<RoutingRule, Func<RouterContext, Task>>>();
        }

        [TestMethod]
        public async Task Simple_routes_are_routed()
        {
            var success = false;
            Router.Add("/", c => Task.Factory.StartNew(() => { success = true; }));
            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/"));
            request.Setup(r => r.Method).Returns("GET");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);
            
            await Router.Invoke(context.Object).ConfigureAwait(false);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task Thrown_exceptions_are_caught()
        {
            Router.Add("/", c => { throw new Exception(); });
            var response = new Mock<IOwinResponse>();
            response.Setup(r => r.WriteAsync(It.IsAny<string>())).Returns(Task.Factory.StartNew(() => { }));
            response.SetupProperty(r => r.StatusCode);

            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/"));
            request.Setup(r => r.Method).Returns("GET");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);
            context.Setup(c => c.Response).Returns(response.Object);

            await Router.Invoke(context.Object).ConfigureAwait(false);

            Assert.AreEqual(500, context.Object.Response.StatusCode);
        }

        [TestMethod]
        public async Task Complex_routes_with_ints_are_routed()
        {
            var success = false;
            Router.Add("/:id", c => Task.Factory.StartNew(() =>
            {
                Assert.AreEqual(1, c.Param<int>(":id"));
                success = true;
            }));
            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/1"));
            request.Setup(r => r.Method).Returns("GET");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);

            await Router.Invoke(context.Object).ConfigureAwait(false);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task Complex_routes_with_ints_and_guids_are_routed()
        {
            var guid = Guid.Empty;
            var success = false;
            Router.Add("/:id/:someguid", c => Task.Factory.StartNew(() =>
            {
                Assert.AreEqual(1, c.Param<int>(":id"));
                Assert.AreEqual(guid, c.Param<Guid>(":someguid"));
                success = true;
            }));
            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/1/{00000000-0000-0000-0000-000000000000}"));
            request.Setup(r => r.Method).Returns("GET");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);

            await Router.Invoke(context.Object).ConfigureAwait(false);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task Routes_are_executed_in_correct_order()
        {
            var success = false;
            Router.Add("/", c=> Task.Factory.StartNew(()=> Assert.Fail()));
            Router.Add("/:id", c => Task.Factory.StartNew(() =>
            {
                Assert.AreEqual(1, c.Param<int>(":id"));
                success = true;
            }));
            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/1"));
            request.Setup(r => r.Method).Returns("GET");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);

            await Router.Invoke(context.Object).ConfigureAwait(false);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task Route_verb_is_honoured()
        {
            var success = false;
            Router.Add("/", c => Task.Factory.StartNew(() => Assert.Fail()));
            Router.Add("/", c => Task.Factory.StartNew(() => { success = true; }), "POST");
            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/"));
            request.Setup(r => r.Method).Returns("POST");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);

            await Router.Invoke(context.Object).ConfigureAwait(false);

            Assert.IsTrue(success);
        }
    }
}