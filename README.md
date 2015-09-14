# Roetur

A lightwieight OWIN-based API framework. There are no sub-classes needed to process a Uri, instead there is a Router (Roetur) which you can register actions for Uri's with. Here's an example from the self-hosted TestService:

```C#
protected override async Task RunAsync(CancellationToken cancellationToken)
{
    Roetur.Core.Roetur.Add("/", context => context.Ok(() => HelloWorld.SayHello()));
    Roetur.Core.Roetur.Add("/exception", context => { throw new Exception(); });
    Roetur.Core.Roetur.Add("/:id", context => context.Ok(()=> context.Param<int>(":id")));

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
```

Every request to the OwinServer is passed through the Roetur. This will attempt to match the Uri's AbsolutePath along with the HTTP Verb of the request and if a match is found it will be processed using the relevant function provided. Using the Ok<T> extension method the response is attempted to be serialised into JSON and returned as an 200 OK and if this fails a 500 Internal Server error will be thrown and the exception serialised out. You can also use the Ok extension method to just return a 200 OK without a body.

Uri parameters can be specified and they can be retreived using the Param<T> extension method. Simply use the same name you specified for the param in the route to retreive it. These are also converted to whatever you specify so that you don't need to pass everything around as a string.

You can put anything inside of the lambda for the route, but ideally you want to at least write something out the to RoetContext.OwinContext.Response otherwise you'll not return anything!

## You've spelt Router wrong!

No I haven't. It's a bit of an "in joke" really. I've worked with a lot of people who create mis-spelled classes/methods/messages and for once I wanted to do some mis-spelling. It also drives ReSpeller (a plug-in for ReSharper) a bit mad.
