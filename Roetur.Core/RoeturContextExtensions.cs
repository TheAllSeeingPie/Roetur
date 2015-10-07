using System;
using System.IO;
using System.Threading.Tasks;
using Jil;

namespace Roetur.Core
{
    public static class RoeturContextExtensions
    {
        public static Task Ok(this RoeturContext context)
        {
            return context.OwinContext.Ok();
        }

        public static Task OkJson<TIn>(this RoeturContext context, Func<TIn> item)
        {
            return context.OwinContext.OkJson(item);
        }

        public static TOut Payload<TOut>(this RoeturContext context)
        {
            return JSON.Deserialize<TOut>(new StreamReader(context.OwinContext.Request.Body).ReadToEnd());
        }
    }
}