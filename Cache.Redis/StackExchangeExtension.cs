using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace Cache.Redis
{
    public static class StackExchangeExtension
    {
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void ObjectSet(this IDatabase db, string key, object value)
        {
            db.StringSet(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="ticks">过期时间</param>
        public static void ObjectSet(this IDatabase db, string key, object value, long ticks)
        {
            db.StringSet(key, JsonConvert.SerializeObject(value), TimeSpan.FromTicks(ticks));
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="db"></param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static T ObjectGet<T>(this IDatabase db, string key)
        {
            string value = db.StringGet(key);

            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 获取Object对象
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static object ObjectGet(this IDatabase db, string key)
        {
            string value = db.StringGet(key);

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(value);
        }
    }
}
