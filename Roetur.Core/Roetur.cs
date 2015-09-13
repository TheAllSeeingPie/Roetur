using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Roetur.Core
{
    public static class Roetur
    {
        internal static readonly ConcurrentDictionary<Regex, Func<RoetContext, Task>> Routes = new ConcurrentDictionary<Regex, Func<RoetContext, Task>>();
        internal static readonly Regex Tokeniser = new Regex(@"(:[\w]+)", RegexOptions.Compiled); 

        public static void AddRoet(string route, Func<RoetContext, Task> action)
        {
            var matches = Tokeniser.Matches(route);
            Regex regex;
            if (matches.Count > 0)
            {
                var formattedRoute = route;
                foreach (Match match in matches)
                {
                    var tokens = match.Groups[1].Captures;
                    foreach (Capture token in tokens)
                    {
                        var formattedToken = $@"(?<{token.Value.Substring(1)}>[^/]+)";
                        formattedRoute = formattedRoute.Replace(token.Value, formattedToken);
                    }
                }
                
                regex = new Regex(formattedRoute);
            }
            else
            {
                regex = new Regex(route);
            }

            Routes.TryAdd(regex, action);
        }

        public static Task Invoke(IOwinContext context)
        {
            var absolutePath = context.Request.Uri.AbsolutePath;
            Func<RoetContext, Task> action;

            var match = Routes.Keys.FirstOrDefault(k => k.IsMatch(absolutePath));

            if (match != null && Routes.TryGetValue(match, out action))
            {
                var roetContext = new RoetContext(context, match);
                return action.Invoke(roetContext);
            }

            return context.Error500("No route found");
        }
    }
}