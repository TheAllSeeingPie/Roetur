using Owin;
using Roetur.Core;

namespace TestService
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Use<RoeturServer>();
        }
    }
}