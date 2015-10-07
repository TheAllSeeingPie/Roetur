using System;
using System.Threading.Tasks;
using Jil;
using Microsoft.Owin;

namespace Roetur.Core
{
    public static class IOwinContextOutputExtensions
    {
        public static Task Ok(this IOwinContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                context.Response.StatusCode = 200;
            });
        }

        public static Task OkJson<TIn>(this IOwinContext context, Func<TIn> item)
        {
            return context.Write(JSON.Serialize(item()), 200);
        }

        public static Task Error401(this IOwinContext context, string message)
        {
            return context.Write(JSON.Serialize(message), 401, "text/plain");
        }

        public static Task Error500(this IOwinContext context, string message)
        {
            return context.Write(JSON.Serialize(message), 500, "text/plain");
        }

        private static Task Write(this IOwinContext context, string message, int statusCode, string contentType = "application/json")
        {
            context.Response.ContentType = contentType;
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(message);
        }
    }
}