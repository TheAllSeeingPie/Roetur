using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Microsoft.Owin;

namespace Roetur.Core
{
    public class RouterContext
    {
        internal IOwinContext OwinContext { get; private set; }
        internal IReadOnlyDictionary<string, string> Params { get; private set; }
        internal string Method { get; set; }

        public RouterContext(IOwinContext owinContext, Regex regex)
        {
            OwinContext = owinContext;
            var groupNames = regex.GetGroupNames();
            if (groupNames.Length > 1)
            {
                GetParams(owinContext, regex, groupNames.Skip(1));
            }
        }

        private void GetParams(IOwinContext context, Regex regex, IEnumerable<string> groupNames)
        {
            var match = regex.Match(context.Request.Uri.AbsolutePath);
            Params = groupNames.ToDictionary(groupName => $":{groupName}", groupName => WebUtility.UrlDecode(match.Groups[groupName].Value));
        }
    }
}