using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services;
using Roetur.Core;

namespace TestService
{
    public class TestService : StatelessService
    {
        protected override ICommunicationListener CreateCommunicationListener()
        {
            // TODO: Replace this with an ICommunicationListener implementation if your service needs to handle user requests.
            return base.CreateCommunicationListener();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            Route.Add("/", context => context.Ok(() => HelloWorld.SayHello()));
            Route.Add("/exception", context => { throw new Exception(); });

            int iterations = 0;
            using (WebApp.Start<Startup>("http://localhost:8002"))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ServiceEventSource.Current.ServiceMessage(this, "Working-{0}", iterations++);
                    await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
                }
            }
        }
    }
}