using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
