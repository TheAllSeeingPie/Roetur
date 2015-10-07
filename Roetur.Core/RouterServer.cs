using System.Threading.Tasks;
using Microsoft.Owin;

namespace Roetur.Core
{
    public class RouterServer : OwinMiddleware
    {
        public RouterServer(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            return Router.Invoke(context);
        }
    }
}