using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Wcivy.Core.Common;
using Wcivy.Core.Logging;
using Wcivy.WebApi.Config;

namespace Wcivy.Core.Http
{
    /// <summary>
    /// 统一接口数据格式，记录请求以及异常的日志
    /// </summary>
    public class ApiDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 请求日志
            if (AppSettings.IsApiRequestLogEnabled)
                RequestLogAsync(request, HttpContext.Current);
            // 只接收Post和Get请求
            if (request.Method != HttpMethod.Post && request.Method != HttpMethod.Get)
            {
                return base.SendAsync(request, cancellationToken);
            }
            // 返回统一格式的数据
            return base.SendAsync(request, cancellationToken).ContinueWith(((Task<HttpResponseMessage> t) =>
            {
                var content = string.Empty;
                try
                {
                    if (t.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        HttpContent httpContent = t.Result.Content;
                        if (httpContent != null)
                        {
                            var data = httpContent.ReadAsAsync<object>();
                            // 判断是否为ApiResponse对象，如果不是则转化为该对象输出
                            if (data.Result is ApiResponse)
                            {
                                return t.Result;
                            }
                            else
                            {
                                content = JsonHelper.Serialize(ApiResponse.Success(data.Result));
                            }
                        }
                        else
                        {
                            // 错误日志
                            content = JsonHelper.Serialize(
                                ApiResponse.Error("响应错误", (int)t.Result.StatusCode)
                            );
                            Logger.Instance.ErrorFormat("响应错误: {0}" + t.Result.ReasonPhrase);
                        }
                    }
                    else
                    {
                        // 错误日志
                        content = JsonHelper.Serialize(
                            ApiResponse.Error("响应错误", (int)t.Result.StatusCode)
                        );
                        Logger.Instance.ErrorFormat("响应错误: {0}" + t.Result.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    // 异常日志
                    content = JsonHelper.Serialize(ApiResponse.Error("内部错误"));
                    Logger.Instance.ErrorFormat(ex, "内部错误: {0}", ex.Message);
                }

                return new HttpResponseMessage
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
            }));
        }

        #region Api日志操作
        /// <summary>
        /// 记录请求日志
        /// </summary>
        /// <param name="request"></param>
        private Task RequestLogAsync(HttpRequestMessage request, HttpContext httpContext)
        {
            return Task.Run(() =>
            {
                var httpMothed = request.Method.ToString();
                var uri = request.RequestUri.PathAndQuery;
                var header = JsonHelper.Serialize(request.Headers);

                httpContext.Request.InputStream.Position = 0;
                var ipStream = httpContext.Request.InputStream;
                var bytes = new byte[ipStream.Length];
                httpContext.Request.InputStream.Read(bytes, 0, bytes.Length);
                var parameters = Encoding.UTF8.GetString(bytes);

                Logger.Instance.InfoFormat("HttpMothed:{0},Action:{1},Header:{2},Params:{3}",
                    httpMothed, uri, header, parameters);
            });
        }
        #endregion
    }
}
