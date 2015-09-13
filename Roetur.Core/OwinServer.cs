using System.Threading.Tasks;
using Microsoft.Owin;

namespace Roetur.Core
{
    public class OwinServer : OwinMiddleware
    {
        public OwinServer(OwinMiddleware next)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            return Route.Invoke(context);
        }
    }
}