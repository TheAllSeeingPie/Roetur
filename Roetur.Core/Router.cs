using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Roetur.Core
{
    internal class RoutingRule
{
        public RoutingRule(Regex regex, string method)
        {
            Regex = regex;
            Method = method;
        }

        public Regex Regex { get; }
        public string Method { get; }
}

    public static class Router
    {
        internal static IEnumerable<Tuple<RoutingRule, Func<RouterContext, Task>>> Routes = new List<Tuple<RoutingRule, Func<RouterContext, Task>>>();
        internal static readonly Regex RouteValidator = new Regex(@"^/$|(/:?[\w-]+)", RegexOptions.Compiled);
        internal static readonly Regex Tokeniser = new Regex(@"(:[\w]+)", RegexOptions.Compiled);

        public static void Add(string route, Func<RouterContext, Task> action, string verb = "GET")
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

            Routes = new List<Tuple<RoutingRule, Func<RouterContext, Task>>>(Routes)
            {
                new Tuple<RoutingRule, Func<RouterContext, Task>>(new RoutingRule(regex, verb), action)
            }.OrderByDescending(t=> t.Item1.Regex.ToString()).ToArray();
        }

        public static Task Invoke(IOwinContext context)
        {
            var absolutePath = context.Request.Uri.AbsolutePath;
            var match = Routes.FirstOrDefault(t => t.Item1.Regex.IsMatch(absolutePath) && t.Item1.Method.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase));

            if (match == null ) return context.Error500("No route found");

            try
            {
                return match.Item2.Invoke(new RouterContext(context, match.Item1.Regex))
                    .ContinueWith(t => context.Ok());
            }
            catch (UnauthorizedAccessException)
            {
                return context.Error401("401 Not Authorised");
            }
            catch (Exception e)
            {
                return context.Error500(e.ToString());
            }
        }
    }
}