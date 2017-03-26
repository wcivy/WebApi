using Wcivy.Core.Config;

namespace Wcivy.WebApi.Config
{
    public class AppSettings
    {
        /// <summary>
        /// 是否开启请求日志
        /// </summary>
        public static bool IsApiRequestLogEnabled
        {
            get
            {
                return AppSettingsConfig.GetBooleanValue(false, configKey: "ApiRequestLogEnabled");
            }
        }
        /// <summary>
        /// 请求过期时间
        /// </summary>
        public static int RequestExpireTime
        {
            get
            {
                return AppSettingsConfig.GetIntValue(10);
            }
        }
    }
}