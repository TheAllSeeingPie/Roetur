using System.Threading.Tasks;
using Microsoft.Owin;

namespace Roetur.Core
{
    public class RoeturServer : OwinMiddleware
    {
        public RoeturServer(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            return Router.Invoke(context);
        }
    }
}