using System;
using System.Collections.Generic;
using System.Linq;

namespace Snippets.Web.Common.Extensions
{
    public static class ListExtensions
    {
        public static T Random<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }
}
