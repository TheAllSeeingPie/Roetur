using System;
using System.IO;
using System.Threading.Tasks;
using Jil;

namespace Roetur.Core
{
    public static class RouterContextExtensions
    {
        public static Task Ok(this RouterContext context)
        {
            return context.OwinContext.Ok();
        }

        public static Task OkJson<TIn>(this RouterContext context, Func<TIn> item)
        {
            return context.OwinContext.OkJson(item);
        }

        public static TOut Payload<TOut>(this RouterContext context)
        {
            return JSON.Deserialize<TOut>(new StreamReader(context.OwinContext.Request.Body).ReadToEnd());
        }

        public static dynamic Payload(this RouterContext context)
        {
            return JSON.DeserializeDynamic(new StreamReader(context.OwinContext.Request.Body).ReadToEnd());
        }
    }
}