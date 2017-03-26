using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Wcivy.Sample.Client
{
    /// <summary>
    /// 签名认证请求方法
    /// </summary>
    public class SignAuthClient
    {
        private string _appid;
        private string _key;
        private Uri _baseUri;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appid">应用代码</param>
        /// <param name="key">密钥</param>
        /// <param name="baseUri">接口服务地址</param>
        public SignAuthClient(string appid, string key, string baseUri)
        {
            _appid = appid;
            _key = key;
            _baseUri = new Uri(baseUri);
        }

        #region 同步方法
        /// <summary>
        /// GET或POST请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpMethod">请求方法。GET或POST</param>
        /// <param name="url">接口地址</param>
        /// <param name="param">参数</param>
        /// <param name="isEncrypt">是否加密：默认true</param>
        /// <returns></returns>
        public ApiResponse Send<T>(HttpMethod httpMethod, string url, T param = default(T), bool isEncrypt = true) where T : class
        {
            if (httpMethod == HttpMethod.Get)
            {
                if (typeof(T) == typeof(string))
                {
                    return Get(url, param.ToString(), isEncrypt);
                }
                else if (typeof(T) == typeof(NameValueCollection))
                {
                    return Get(url, param as NameValueCollection, isEncrypt);
                }
            }
            else if (httpMethod == HttpMethod.Post)
            {
                return Post(url, param, isEncrypt);
            }
            return null;
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="query">url参数。例：a=1&b=2</param>
        /// <param name="isEncrypt">是否加密：默认true</param>
        /// <returns></returns>
        public ApiResponse Get(string url, string query = "", bool isEncrypt = true)
        {
            var singQuery = string.Empty;
            var uri = GetApiUrl(url);
            if (!string.IsNullOrWhiteSpace(query))
            {
                uri += "?" + query;
                singQuery = GetSignQueryString(query);
            }
            var request = (HttpWebRequest)WebRequest.Create(uri);
            //加入头信息
            if (isEncrypt) AddWebRequestSignHeader(request, singQuery);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Set("Pragma", "no-cache");

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReceive = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(streamReceive, Encoding.UTF8))
                        {
                            var result = streamReader.ReadToEnd();
                            return JsonConvert.DeserializeObject<ApiResponse>(result);
                        }
                    }
                }
                else
                {
                    return new ApiResponse
                    {
                        Code = (int)response.StatusCode,
                        IsSuccess = false,
                        Message = "请求失败"
                    };
                }
            }
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="nvc">参数</param>
        /// <param name="isEncrypt">是否加密：默认true</param>
        /// <returns></returns>
        public ApiResponse Get(string url, NameValueCollection nvc, bool isEncrypt = true)
        {
            var query = string.Join("&", nvc.AllKeys.Select(k => k + "=" + nvc[k]));
            return Get(url, query, isEncrypt);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <param name="isEncrypt">是否加密：默认true</param>
        /// <returns></returns>
        public ApiResponse Post<T>(string url, T value, bool isEncrypt = true) where T : class
        {
            var data = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(data);
            var request = (HttpWebRequest)WebRequest.Create(GetApiUrl(url));
            //加入头信息
            if (isEncrypt) AddWebRequestSignHeader(request, data);

            request.Method = "POST";
            request.ContentLength = bytes.Length;
            request.ContentType = "application/json";
            request.Headers.Set("Pragma", "no-cache");

            //写数据
            using (var reqstream = request.GetRequestStream())
            {
                reqstream.Write(bytes, 0, bytes.Length);
            }

            //读数据
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReceive = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(streamReceive, Encoding.UTF8))
                        {
                            string result = streamReader.ReadToEnd();
                            return JsonConvert.DeserializeObject<ApiResponse>(result);
                        }
                    }
                }
                else
                {
                    return new ApiResponse
                    {
                        Code = (int)response.StatusCode,
                        IsSuccess = false,
                        Message = "请求失败"
                    };
                }
            }
        }
        #endregion

        #region 异步方法
        /// <summary>
        /// 异步GET或POST请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpMethod">请求方法。GET或POST</param>
        /// <param name="url">接口地址</param>
        /// <param name="param">参数</param>
        /// <param name="isEncrypt">是否加密：默认true</param>
        /// <returns></returns>
        public async Task<ApiResponse> SendAsync<T>(HttpMethod httpMethod, string url, T param = default(T), bool isEncrypt = true) where T : class
        {
            if (httpMethod == HttpMethod.Get)
            {
                if (typeof(T) == typeof(string))
                {
                    return await GetAsync(url, param.ToString(), isEncrypt);
                }
                else if (typeof(T) == typeof(NameValueCollection))
                {
                    return await GetAsync(url, param as NameValueCollection, isEncrypt);
                }
            }
            else if (httpMethod == HttpMethod.Post)
            {
                return await PostAsync(url, param, isEncrypt);
            }
            return null;
        }
        /// <summary>
        /// 异步Get请求
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="query">url参数。例：a=1&b=2</param>
        /// <param name="isEncrypt">是否加密：默认true</param>
        /// <returns></returns>
        public async Task<ApiResponse> GetAsync(string url, string query = "", bool isEncrypt = true)
        {
            HttpClient client = new HttpClient();
            var singQuery = string.Empty;
            var uri = GetApiUrl(url);
            if (!string.IsNullOrWhiteSpace(query))
            {
                uri += "?" + query;
                singQuery = GetSignQueryString(query);
            }
            //加入头信息
            if (isEncrypt) AddHttpClientSignHeader(client, singQuery);

            var content = new ApiResponse();
            var response = await client.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                content = await response.Content.ReadAsAsync<ApiResponse>();
            }
            else
            {
                content = new ApiResponse
                {
                    Code = (int)response.StatusCode,
                    IsSuccess = false,
                    Message = "请求失败"
                };
            }
            return content;
        }

        public async Task<ApiResponse> GetAsync(string url, NameValueCollection nvc, bool isEncrypt = true)
        {
            var query = string.Join("&", nvc.AllKeys.Select(k => k + "=" + nvc[k]));
            return await GetAsync(url, query, isEncrypt);
        }

        /// <summary>
        /// 异步Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">接口地址</param>
        /// <param name="value">参数</param>
        /// <param name="isEncrypt">是否加密：默认true</param>
        /// <returns></returns>
        public async Task<ApiResponse> PostAsync<T>(string url, T value, bool isEncrypt = true) where T : class
        {
            HttpClient client = new HttpClient();
            var data = JsonConvert.SerializeObject(value);

            //加入头信息
            if (isEncrypt) AddHttpClientSignHeader(client, data);

            var content = new ApiResponse();
            var response = await client.PostAsJsonAsync(GetApiUrl(url), value);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                content = await response.Content.ReadAsAsync<ApiResponse>();
            }
            else
            {
                content = new ApiResponse
                {
                    Code = (int)response.StatusCode,
                    IsSuccess = false,
                    Message = "请求失败"
                };
            }
            return content;
        }
        #endregion

        #region 私有方法
        private string GetApiUrl(string uri)
        {
            return (_baseUri != null) ? new Uri(_baseUri, uri).ToString() : uri;
        }
        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <param name="appid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetSignature(string data, string appid, string timeStamp, string nonce)
        {
            var secret = _key;
            var result = new StringBuilder();

            var hash = System.Security.Cryptography.MD5.Create();
            //拼接签名数据（顺序：请求参数 -> 应用id -> 时间戳 -> 随机数 -> 密钥）
            var signStr = data + appid + timeStamp + nonce + secret;
            var md5 = System.Security.Cryptography.MD5.Create();
            // 编码UTF8/Unicode　
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(signStr));
            // 转换成字符串
            foreach (var b in bytes)
            {
                result.Append(b.ToString("X2"));
            }
            return result.ToString();
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        private string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary>  
        /// 获取随机数
        /// </summary>  
        /// <returns></returns>  
        private string GetRandom()
        {
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            int i = rd.Next(0, int.MaxValue);
            return i.ToString();
        }

        /// <summary>
        /// 拼接加密用的请求参数
        /// </summary>
        /// <param name="query">url参数。例：a=1&b=2</param>
        /// <returns></returns>
        private string GetSignQueryString(string query)
        {
            // 按Key的字母顺序排序
            var sortedParams = GetSortedDictionary(HttpUtility.ParseQueryString(query));
            var sb = new StringBuilder();
            // 把所有参数名和参数值串在一起
            foreach (var item in sortedParams)
            {
                sb.Append(item.Key).Append(item.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取排序的键值对
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private SortedDictionary<string, string> GetSortedDictionary(NameValueCollection nvc, Func<string, bool> filter = null)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            if (nvc != null && nvc.Count > 0)
            {
                foreach (var k in nvc.AllKeys)
                {
                    if (filter == null || !filter(k))
                    {//如果没设置过滤条件或者无需过滤  
                        dic.Add(k, nvc[k]);
                    }
                }
            }
            return dic;
        }

        #region 添加签名
        private void AddWebRequestSignHeader(HttpWebRequest request, string data)
        {
            var timeStamp = GetTimeStamp();
            var nonce = GetRandom();
            request.Headers.Add("appid", _appid);
            request.Headers.Add("timestamp", timeStamp);
            request.Headers.Add("nonce", nonce);
            request.Headers.Add("signature", GetSignature(data, _appid, timeStamp, nonce));
        }

        /// <summary>
        /// 添加请求头信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="appid"></param>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <param name="data"></param>
        private void AddHttpClientSignHeader(HttpClient client, string data)
        {
            var timeStamp = GetTimeStamp();
            var nonce = GetRandom();
            //当前请求的appid
            client.DefaultRequestHeaders.Add("appid", _appid);
            //发起请求时的时间戳（单位：毫秒）
            client.DefaultRequestHeaders.Add("timestamp", timeStamp);
            //随机数，增加破解复杂度
            client.DefaultRequestHeaders.Add("nonce", nonce);
            //当前请求内容的数字签名
            client.DefaultRequestHeaders.Add("signature", GetSignature(data, _appid, timeStamp, nonce));
        }
        #endregion
    }
    #endregion
}
