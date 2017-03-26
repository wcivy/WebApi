using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using Wcivy.Core.Logging;

namespace Wcivy.Core.Http
{
    /// <summary>
    /// 捕获Unhandled返回统一请求结果
    /// </summary>
    public class UnhandledExceptionHandler : ExceptionHandler
    {
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (context.Exception is ApiException)
                {
                    // 未处理的自定义异常
                    context.Result = new ResponseMessageResult(
                    context.Request.CreateResponse(
                            ApiResponse.Error("内部错误")
                        )
                    );
                    Logger.Instance.ErrorFormat(context.Exception, "内部错误: {0}", context.Exception.Message);
                }
                else
                {
                    // 未处理的内部异常
                    context.Result = new ResponseMessageResult(
                        context.Request.CreateResponse(
                            ApiResponse.Error("内部错误")
                        )
                    );
                }
            });
        }
    }
}
