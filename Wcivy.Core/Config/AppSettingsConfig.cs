using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Wcivy.Core.Config
{
    public static class AppSettingsConfig
    {
        /// <summary>
        /// 获取AppSettings配置信息
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <param name="key">类属性的key名称</param>
        /// <param name="configKey">config中的key名称</param>
        /// <returns>int类型的值</returns>
        public static bool GetBooleanValue(bool defaultValue,
            [CallerMemberName]string key = "", string configKey = "")
        {
            return GetValue(bool.Parse, defaultValue, key, configKey);
        }

        /// <summary>
        /// 获取AppSettings配置信息
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <param name="key">类属性的key名称</param>
        /// <param name="configKey">config中的key名称</param>
        /// <returns>int类型的值</returns>
        public static int GetIntValue(int defaultValue,
            [CallerMemberName]string key = "", string configKey = "")
        {
            return GetValue(int.Parse, defaultValue, key, configKey);
        }

        /// <summary>
        /// 获取AppSettings配置信息
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <param name="key">类属性的key名称</param>
        /// <param name="configKey">config中的key名称</param>
        /// <returns>string类型的值</returns>
        public static string GetStringValue(string defaultValue,
            [CallerMemberName]string key = "", string configKey = "")
        {
            return GetValue(_ => _, defaultValue, key, configKey);
        }

        /// <summary>
        /// 获取AppSettings配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseFunc">类型转换方法</param>
        /// <param name="defaultTValue">默认值</param>
        /// <param name="key">类属性的key名称</param>
        /// <param name="configKey">config中的key名称</param>
        /// <returns></returns>
        public static T GetValue<T>(Func<string, T> parseFunc, T defaultTValue,
            [CallerMemberName]string key = "", string configKey = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(configKey))
                {
                    key = configKey;
                }

                var node = ConfigurationManager.AppSettings[key];
                return !string.IsNullOrEmpty(node) ? parseFunc(node) : defaultTValue;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
