using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using Wcivy.Core.Redis;
using Wcivy.Core.Security;
using Wcivy.WebApi.Config;

namespace Wcivy.Core.Http.Filters
{
    /// <summary>
    /// 签名认证
    /// </summary>
    public class SignatureAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var signatureHeader = new SignatureHeader(request.Headers);

            //判断请求头
            if (!signatureHeader.Verify())
            {
                // 请求头部不完整
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "请求头部不完整"
                };
                base.OnActionExecuting(actionContext);
                return;
            }

            // 重复提交
            if (RedisCache.Instance.Exists(signatureHeader.Signature))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "重复提交"
                };
                base.OnActionExecuting(actionContext);
                return;
            }

            //判断请求是否过期
            if (IsTimeout(signatureHeader.Timestamp))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "请求已过期"
                };
                base.OnActionExecuting(actionContext);
                return;
            }

#if DEBUG
            var secret = "test";
#else
            // 根据SignatureHeader.AppId从配置或者数据库中取出对应的密钥
            var secret = "test";
#endif

            //根据请求类型拼接参数
            var method = request.Method.Method;
            NameValueCollection form = HttpContext.Current.Request.QueryString;
            string data = string.Empty;
            switch (method)
            {
                case "POST":
                case "PUT":
                    HttpContext.Current.Request.InputStream.Position = 0;
                    var stream = HttpContext.Current.Request.InputStream;
                    var byts = new byte[stream.Length];
                    HttpContext.Current.Request.InputStream.Read(byts, 0, byts.Length);
                    data = Encoding.UTF8.GetString(byts);
                    break;
                case "GET":
                case "DELETE":
                    data = GetSignQueryString(form);
                    break;
                default:
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        ReasonPhrase = string.Format("不允许{0}请求", method)
                    };
                    base.OnActionExecuting(actionContext);
                    return;
            }
            // 签名数据（顺序：请求参数 -> 应用id -> 时间戳 -> 随机数）
            var signData = data + signatureHeader.ToString();
            if (!Signature.Verify(signData, secret, signatureHeader.Signature))
            {// 无效签名
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "无效签名"
                };
                base.OnActionExecuting(actionContext);
                return;
            }
            else
            {
                // 插入缓存，用于判断是否重复提交
                RedisCache.Instance.AddAsync(signatureHeader.Signature, AppSettings.RequestExpireTime);
                base.OnActionExecuting(actionContext);
            }
        }

        /// <summary>
        /// 是否超时
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private bool IsTimeout(string timestamp)
        {
            var ts1 = 0d;
            if (!double.TryParse(timestamp, out ts1)) return true;

            var ts2 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            var ts = ts2 - ts1;

            return ts > AppSettings.RequestExpireTime * 1000d;
        }

        /// <summary>
        /// 拼接加密用的请求参数
        /// </summary>
        /// <param name="query">url参数。</param>
        /// <returns></returns>
        private string GetSignQueryString(NameValueCollection query)
        {
            // 按Key的字母顺序排序
            var sortedParams = GetSortedDictionary(query);
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
        /// <param name="nvc"></param>
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
    }
}
