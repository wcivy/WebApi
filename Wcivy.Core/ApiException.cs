using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcivy.Core
{
    public class ApiException : ApplicationException
    {
        public ApiException()
        {

        }
        /// <summary>
        /// 构造审批错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="inner"></param>
        public ApiException(string message, Exception inner)
            : base(DateTime.Now.ToString() + ">[Wcivy WebApi]" + message, inner)
        {

        }

        /// <summary>
        /// 构造审批错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public ApiException(string message)
            : base(DateTime.Now.ToString() + ">[Wcivy WebApi]" + message)
        {

        }
    }
}
