using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using ServiceStack.Text;

namespace Roetur.Core
{
    public static class IOwinContextOutputExtensions
    {
        public static Task Ok(this IOwinContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 200;
            });
        }

        public static Task Ok<TIn>(this IOwinContext context, Func<TIn> item)
        {
            string s;
            try
            {
                s = JsonSerializer.SerializeToString(item());
            }
            catch (Exception e)
            {
                return context.Error500(e.ToString());
            }
            return context.Write(s, 200);
        }

        public static Task Error500(this IOwinContext context, string message)
        {
            return context.Write(JsonSerializer.SerializeToString(message), 500);
        }

        private static Task Write(this IOwinContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(message);
        }
    }
}