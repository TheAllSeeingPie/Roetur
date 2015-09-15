using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Owin.Fakes;
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

        [TestMethod]
        public void Payload_can_be_deserialised_from_context()
        {
            byte[] buffer = Encoding.ASCII.GetBytes(@"{""Name"":""Hello world!""");
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
    }
}
