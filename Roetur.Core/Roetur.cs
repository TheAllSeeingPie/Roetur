using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Roetur.Core
{
    public static class Roetur
    {
        internal static IEnumerable<Tuple<Regex, Func<RoetContext, Task>>> Routes = new List<Tuple<Regex, Func<RoetContext, Task>>>();
        internal static readonly Regex RouteValidator = new Regex(@"^/$|(/:?[\w-]+)", RegexOptions.Compiled);
        internal static readonly Regex Tokeniser = new Regex(@"(:[\w]+)", RegexOptions.Compiled); 

        public static void Add(string route, Func<RoetContext, Task> action)
        {
            if (!RouteValidator.IsMatch(route))
            {
                throw new NotSupportedException($"The specified route of {route} is not supported");
            }

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
                
                regex = new Regex(formattedRoute, RegexOptions.Compiled);
            }
            else
            {
                regex = new Regex(route, RegexOptions.Compiled);
            }

            Routes = new List<Tuple<Regex, Func<RoetContext, Task>>>(Routes)
            {
                new Tuple<Regex, Func<RoetContext, Task>>(regex, action)
            }.OrderByDescending(t=> t.Item1.ToString()).ToArray();
        }

        public static Task Invoke(IOwinContext context)
        {
            var absolutePath = context.Request.Uri.AbsolutePath;
            var match = Routes.FirstOrDefault(t => t.Item1.IsMatch(absolutePath));

            if (match == null ) return context.Error500("No route found");

            var roetContext = new RoetContext(context, match.Item1);
            return match.Item2.Invoke(roetContext);
        }
    }
}