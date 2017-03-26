using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StackExchange.Redis;
using Wcivy.Core.Common;
using Wcivy.Core.Logging;
using Wcivy.Core.Extensions;

namespace Wcivy.Core.Redis
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public sealed class RedisCache
    {
        private static readonly Lazy<RedisCache> _lazy = new Lazy<RedisCache>(() => new RedisCache());
        private static readonly Lazy<ConnectionMultiplexer> _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return GetConnection();
        });
        private static IConnectionMultiplexer _conn { get { return _lazyConnection.Value; } }
        private static IDatabase _db;

        public static RedisCache Instance { get { return _lazy.Value; } }
        private RedisCache()
        {
            _db = _conn.GetDatabase();
        }

        public bool Add<T>(string key, T value, TimeSpan? expiry = null)
        {
            return _db.StringSet(key, JsonHelper.Serialize(value), expiry);
        }

        public Task<bool> AddAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            return _db.StringSetAsync(key, JsonHelper.Serialize(value), expiry);
        }

        public T Get<T>(string key)
        {
            return JsonHelper.Deserialize<T>(_db.StringGet(key));
        }

        public bool Exists(string key)
        {
            return _db.KeyExists(key);
        }

        public bool Remove(string key)
        {
            return _db.KeyDelete(key);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            keys.ForEach(k => Remove(k));
        }

        private static ConnectionMultiplexer GetConnection()
        {
            var config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "redis.config");

            var connect = ConnectionMultiplexer.Connect(ConfigManger.GetConfiguration(config));

            connect.ConnectionFailed += (s, e) =>
            {
                Logger.Instance.ErrorFormat("重新连接失败：Endpoint failed: {0}, {1}{2}", e.EndPoint, e.FailureType, (e.Exception == null ? "" : (", " + e.Exception.Message)));
            };
            connect.ConnectionRestored += (s, e) => { Logger.Instance.ErrorFormat("重新建立连接: {0}", e.EndPoint); };
            connect.ErrorMessage += (s, e) => { Logger.Instance.ErrorFormat("发生错误: {0}", e.Message); };
            connect.ConfigurationChanged += (s, e) => { Logger.Instance.ErrorFormat("更改配置: {0}", e.EndPoint); };
            connect.HashSlotMoved += (s, e) => { Logger.Instance.ErrorFormat("更改集群:NewEndPoint is {0}, OldEndPoint is {1}", e.NewEndPoint, e.OldEndPoint); };
            connect.InternalError += (s, e) => { Logger.Instance.ErrorFormat(e.Exception, "Redis内部错误: {0}", e.Exception.Message); };

            return connect;
        }
    }
}
