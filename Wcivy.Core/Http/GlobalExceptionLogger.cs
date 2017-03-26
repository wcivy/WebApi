using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Wcivy.Core.Logging;

namespace Wcivy.Core.Http
{
    /// <summary>
    /// 记录全局异常日志
    /// </summary>
    public class GlobalExceptionLogger : ExceptionLogger
    {
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Logger.Instance.ErrorFormat(context.Exception, "内部错误: {0}", context.Exception.Message);
            });
        }

        /// <summary>
        /// 记录WFException以外的异常
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool ShouldLog(ExceptionLoggerContext context)
        {
            return !(context.Exception is ApiException) && base.ShouldLog(context);
        }
    }
}
