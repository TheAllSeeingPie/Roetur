using Owin;

namespace Roetur.Core
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Use<OwinServer>();
        }
    }
}