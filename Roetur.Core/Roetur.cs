using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Roetur.Core
{
    public static class Route
    {
        internal static readonly IDictionary<string, Func<IOwinContext, Task>> Routes = new Dictionary<string, Func<IOwinContext, Task>>();

        public static void Add(string route, Func<IOwinContext, Task> action)
        {
            Routes.Add(route, action);
        }

        public static Task Invoke(IOwinContext context)
        {
            var absolutePath = context.Request.Uri.AbsolutePath;
            return Routes.ContainsKey(absolutePath) 
                ? Routes[absolutePath].Invoke(context) 
                : context.Error500("No route found");
        }
    }
}
