using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcivy.Sample.Client
{
    public class ApiResponse
    {
        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code { set; get; }

        /// <summary>
        /// 结果
        /// </summary>
        public dynamic Result { set; get; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { set; get; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { set; get; }
    }
}
