using System;
using System.Collections.Generic;
using System.Linq;

namespace Snippets.Web.Common.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Returns a random element from a List
        /// </summary>
        /// <param name="list">List from which elements are selected</param>
        /// <typeparam name="T">The type of objects contained within the list</typeparam>
        /// <returns>A random element from the specefied list</returns>
        public static T Random<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }
}
