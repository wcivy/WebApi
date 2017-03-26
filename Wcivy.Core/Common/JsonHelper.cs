using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Wcivy.Core.Common
{
    public class JsonHelper
    {
        public static string Serialize(object jsonEntiy,
            DefaultContractResolver contractResolver = null,
            string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var t = JsonConvert.SerializeObject(jsonEntiy, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new JsonConverter[] { new IsoDateTimeConverter { DateTimeFormat = dateFormat },
                    new StringEnumConverter() { } },
                //NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            });
            return t;
        }

        public static string Serialize(object jsonEntiy,
            List<JsonConverter> converters,
            DefaultContractResolver contractResolver = null,
            string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            List<JsonConverter> jsonConverter = new List<JsonConverter>
            {
                new IsoDateTimeConverter {DateTimeFormat =dateFormat}
            };
            if (converters != null && converters.Any())
                jsonConverter.AddRange(converters);

            var t = JsonConvert.SerializeObject(jsonEntiy, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Converters = jsonConverter,
                    //NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = contractResolver
                });
            return t;
        }

        /// <summary>
        /// 把json反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 解析json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject Parse(string json)
        {
            return JObject.Parse(json);
        }

        /// <summary>
        /// json转Dictionary
        /// </summary>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(string json)
        {
            var result = Deserialize<Dictionary<string, object>>(json);
            return result;
        }
    }
}
