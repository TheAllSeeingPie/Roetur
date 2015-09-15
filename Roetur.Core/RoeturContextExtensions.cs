using System;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace Roetur.Core
{
    public static class RoeturContextExtensions
    {
        public static Task Ok(this RoeturContext context)
        {
            return context.OwinContext.Ok();
        }

        public static Task Ok<TIn>(this RoeturContext context, Func<TIn> item)
        {
            return context.OwinContext.Ok(item);
        }

        public static TOut Payload<TOut>(this RoeturContext context)
        {
            return JsonSerializer.DeserializeFromStream<TOut>(context.OwinContext.Request.Body);
        }
    }
}