using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using StackExchange.Redis;
using Wcivy.Core.Logging;

namespace Wcivy.Core.Redis
{
    internal static class ConfigManger
    {
        /// <summary>
        /// 获取Redis配置信息
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static ConfigurationOptions GetConfiguration(string configFile)
        {
            var option = new ConfigurationOptions();
            try
            {
                // LinqToXML方式读取配置信息，可以忽略xml大小写。单例模式的情况下只读取一次，可以忽略性能影响
                XDocument document = XDocument.Load(configFile);
                //var redisClient = document.Descendants("redisClient").FirstOrDefault();
                var redisClient = document.Descendants()
                    .Where(p => p.Name.LocalName.Equals("redisClient", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (redisClient.HasAttributes && redisClient.NodeType == XmlNodeType.Element)
                {
                    var attrs = redisClient.Attributes();
                    foreach (var attr in attrs)
                    {
                        switch (attr.Name.LocalName.ToLower())
                        {
                            case "allowadmin":
                                option.AllowAdmin = Common.ConvertHelper.StringToBool(attr.Value, false);
                                break;
                            case "password":
                                option.Password = attr.Value;
                                break;
                            case "connecttimeout":
                                option.ConnectTimeout = Common.ConvertHelper.StringToInt(attr.Value, 5000);
                                break;
                            case "connectretry":
                                option.ConnectRetry = Common.ConvertHelper.StringToInt(attr.Value, 3);
                                break;
                            case "channelprefix":
                                option.ChannelPrefix = attr.Value;
                                break;
                            case "dbnum":
                                option.DefaultDatabase = Common.ConvertHelper.StringToInt(attr.Value, 0);
                                break;
                            case "ssl":
                                option.Ssl = Common.ConvertHelper.StringToBool(attr.Value, false);
                                break;
                            case "sslHost":
                                option.SslHost = attr.Value;
                                break;
                            case "syncTimeout":
                                option.SyncTimeout = Common.ConvertHelper.StringToInt(attr.Value, 1000);
                                break;
                            case "writebuffer":
                                option.WriteBuffer = Common.ConvertHelper.StringToInt(attr.Value, 4096);
                                break;
                        }
                    }
                }

                var hosts = redisClient.Descendants()
                    .Where(p => p.Name.LocalName.Equals("hosts", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault().Elements();
                foreach (var host in hosts)
                {
                    if (host.Name.LocalName.Equals("add", StringComparison.OrdinalIgnoreCase) && host.HasAttributes)
                    {
                        option.EndPoints.Add(
                            host.Attributes().FirstOrDefault(a => a.Name.LocalName.ToLower() == "host").Value,
                            Common.ConvertHelper.StringToInt(host.Attributes()
                                         .FirstOrDefault(a => a.Name.LocalName.ToLower() == "port").Value, 6379)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorFormat(ex, "读取Redis配置文件[{0}]时发生错误: {1}" + configFile, ex.Message);
            }
            return option;
        }
    }
}
