using System;
using System.Threading.Tasks;

namespace Roetur.Core
{
    public static class RoeturContextExtensions
    {
        public static Task Ok<TIn>(this RoetContext context, Func<TIn> item)
        {
            return context.OwinContext.Ok(item);
        }
    }
}