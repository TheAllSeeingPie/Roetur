using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services;
using Roetur.Core;

namespace TestService
{
    public class TestService : StatelessService
    {
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            Router.Add("/", context => context.OkJson(() => HelloWorld.SayHello()));
            Router.Add("/exception", context => { throw new Exception(); });
            Router.Add("/:id", context => context.OkJson(()=> context.Param<int>(":id")));

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