using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Wcivy.Core.Logging;

namespace Wcivy.Core.Http.Filters
{
    /// <summary>
    /// 捕获action异常，返回统一请求结果
    /// </summary>
    public class ActionExceptionAttribute : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (actionExecutedContext.Exception is ApiException)
                {
                    // 未处理的自定义异常
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                        ApiResponse.Error("内部错误")
                    );
                    Logger.Instance.ErrorFormat(actionExecutedContext.Exception, "内部错误: {0}", actionExecutedContext.Exception.Message);
                }
                else
                {
                    // 未处理的内部异常
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(ApiResponse.Error("内部错误"));
                }
            });
        }
    }
}
