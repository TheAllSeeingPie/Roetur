using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Roetur.Core.Tests
{
    /// <summary>
    /// Summary description for RoeturContextExtensionSpecifications
    /// </summary>
    [TestClass]
    public class RoeturContextExtensionSpecifications
    {
        public class Testclass
        {
            public string Name { get; set; }
        }

        [TestInitialize]
        public void Initialise()
        {
            Router.Routes = new List<Tuple<RoutingRule, Func<RouterContext, Task>>>();
        }

        [TestMethod]
        public void Payload_can_be_deserialised_from_context()
        {
            byte[] buffer = Encoding.ASCII.GetBytes(@"{""Name"":""Hello world!""}");

            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(buffer));

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);

            var roeturContext = new RouterContext(context.Object, new Regex("/"));
            var testClass = roeturContext.Payload<Testclass>();

            Assert.AreEqual("Hello world!", testClass.Name);
        }

        [TestMethod]
        public void Dynamic_Payload_can_be_deserialised_from_context()
        {
            byte[] buffer = Encoding.ASCII.GetBytes(@"{""Name"":""Hello world!""}");

            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(buffer));

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);

            var roeturContext = new RouterContext(context.Object, new Regex("/"));
            var testClass = roeturContext.Payload();

            Assert.AreEqual("Hello world!", (string)testClass.Name);
        }

        [TestMethod]
        public async Task Ok_should_return_200_Ok()
        {
            Router.Add("/", c => Task.Delay(0));

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
            await Task.Delay(10);
            Assert.AreEqual(200, context.Object.Response.StatusCode);
        }

        [TestMethod]
        public async Task Ok_with_data_should_return_200_Ok()
        {
            Router.Add("/", c => c.OkJson(()=> "Hello world!"));

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

            Assert.AreEqual(200, context.Object.Response.StatusCode);
        }

        [TestMethod]
        public async Task Ok_with_Exception_should_return_500_Error()
        {
            Router.Add("/", c => c.OkJson(() => {
                throw new Exception();
                return "Hello world!"; //Needed for function type inference
            }));
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
        public async Task Ok_with_UnauthorizedAccessException_should_return_401_Error()
        {
            Router.Add("/", c => c.OkJson(() => {
                throw new UnauthorizedAccessException();
                return "Hello world!"; //Needed for function type inference
            }));
            var response = new Mock<IOwinResponse>();
            response.Setup(r => r.WriteAsync(It.IsAny<string>())).Returns(Task.Factory.StartNew(() => { }));
            response.SetupProperty(r=> r.StatusCode);

            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/"));
            request.Setup(r => r.Method).Returns("GET");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);
            context.Setup(c => c.Response).Returns(response.Object);
            

            await Router.Invoke(context.Object).ConfigureAwait(false);

            Assert.AreEqual(401, context.Object.Response.StatusCode);
        }
    }
}
