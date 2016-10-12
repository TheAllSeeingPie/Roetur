using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Roetur.Core.Tests
{
    [TestClass]
    public class IOwinContextOutputExtensions
    {
        [TestMethod]
        public async Task Ok_should_return_200_Ok()
        {
            var request = new Mock<IOwinRequest>();
            request.Setup(r => r.Uri).Returns(new Uri("http://localhost/"));
            request.Setup(r => r.Method).Returns("GET");

            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Request).Returns(request.Object);
        }
    }
}