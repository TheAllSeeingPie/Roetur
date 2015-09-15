using System;
using System.Threading.Tasks;
using Microsoft.Owin.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Roetur.Core.Tests
{
    [TestClass]
    public class IOwinContextOutputExtensions
    {
        [TestMethod]
        public async Task Ok_should_return_200_Ok()
        {
            var stubIOwinRequest = new StubIOwinRequest
            {
                UriGet = () => new Uri("http://localhost/"),
                MethodGet = () => "GET"
            };
            var context = new StubIOwinContext
            {
                RequestGet = () => stubIOwinRequest
            };
        }
    }
}