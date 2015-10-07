using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Roetur.Routes = new List<Tuple<RoetingRule, Func<RoeturContext, Task>>>();
        }

        [TestMethod]
        public void Payload_can_be_deserialised_from_context()
        {
            byte[] buffer = Encoding.ASCII.GetBytes(@"{""Name"":""Hello world!""}");
            var stubIOwinRequest = new StubIOwinRequest
            {
                BodyGet = () => new MemoryStream(buffer)
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest
            };
            var roeturContext = new RoeturContext(context, new Regex("/"));
            var testClass = roeturContext.Payload<Testclass>();

            Assert.AreEqual("Hello world!", testClass.Name);
        }

        [TestMethod]
        public async Task Ok_should_return_200_Ok()
        {
            Roetur.Add("/", c => Task.Delay(0));
            var owinResponse = new StubIOwinResponse
            {
                InstanceBehavior = StubBehaviors.Current,
                WriteAsyncString = s => Task.Factory.StartNew(() => { })
            };
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/"),
                MethodGet = () => "GET"
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest,
                ResponseGet = () => owinResponse
            };

            await Roetur.Invoke(context);

            Assert.AreEqual(200, ((IOwinContext)context).Response.StatusCode);
        }

        [TestMethod]
        public async Task Ok_with_data_should_return_200_Ok()
        {
            Roetur.Add("/", c => c.OkJson(()=> "Hello world!"));
            var owinResponse = new StubIOwinResponse
            {
                InstanceBehavior = StubBehaviors.Current,
                WriteAsyncString = s => Task.Factory.StartNew(() => { })
            };
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/"),
                MethodGet = () => "GET"
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest,
                ResponseGet = () => owinResponse
            };

            await Roetur.Invoke(context);

            Assert.AreEqual(200, ((IOwinContext)context).Response.StatusCode);
        }

        [TestMethod]
        public async Task Ok_with_Exception_should_return_500_Error()
        {
            Roetur.Add("/", c => c.OkJson(() => {
                throw new Exception();
                return "Hello world!"; //Needed for function type inference
            }));
            var owinResponse = new StubIOwinResponse
            {
                InstanceBehavior = StubBehaviors.Current,
                WriteAsyncString = s => Task.Factory.StartNew(() => { })
            };
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/"),
                MethodGet = () => "GET"
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest,
                ResponseGet = () => owinResponse
            };

            await Roetur.Invoke(context);

            Assert.AreEqual(500, ((IOwinContext)context).Response.StatusCode);
        }

        [TestMethod]
        public async Task Ok_with_UnauthorizedAccessException_should_return_401_Error()
        {
            Roetur.Add("/", c => c.OkJson(() => {
                throw new UnauthorizedAccessException();
                return "Hello world!"; //Needed for function type inference
            }));
            var owinResponse = new StubIOwinResponse
            {
                InstanceBehavior = StubBehaviors.Current,
                WriteAsyncString = s => Task.Factory.StartNew(() => { })
            };
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/"),
                MethodGet = () => "GET"
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest,
                ResponseGet = () => owinResponse
            };

            await Roetur.Invoke(context);

            Assert.AreEqual(401, ((IOwinContext)context).Response.StatusCode);
        }
    }
}
