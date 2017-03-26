using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcivy.Core.Extensions
{
    /// <summary>
    /// IEnumerable扩展类
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// enumeration类型的Foreach方法
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "source">enumeration类型</param>
        /// <param name = "action">操作</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// enumeration类型的异步Foreach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">enumeration类型</param>
        /// <param name="func">操作</param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> func)
        {
            return Task.WhenAll(
                from item in source
                select Task.Run(() => func(item)));
        }
    }
}
