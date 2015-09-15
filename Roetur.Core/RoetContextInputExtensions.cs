using System;
using System.ComponentModel;
using System.Globalization;

namespace Roetur.Core
{
    public static class RoetContextInputExtensions
    {
        public static T Param<T>(this RoeturContext context, string identifier)
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(context.Params[identifier]);
        }
    }
}