namespace Wcivy.Core.Http
{
    /// <summary>
    /// 统一返回请求结果
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code { set; get; }

        /// <summary>
        /// 结果
        /// </summary>
        public object Result { set; get; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { set; get; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { set; get; }

        public static ApiResponse Success(object data)
        {
            return new ApiResponse
            {
                Code = 200,
                IsSuccess = true,
                Result = data
            };
        }

        public static ApiResponse Error(string message, int code = 500)
        {
            return new ApiResponse
            {
                Code = code,
                IsSuccess = false,
                Message = message
            };
        }
    }
}
