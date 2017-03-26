using System.Linq;
using System.Net.Http.Headers;

namespace Wcivy.Core.Security
{
    /// <summary>
    /// 请求签名头
    /// </summary>
    public class SignatureHeader
    {
        public SignatureHeader(HttpRequestHeaders Headers)
        {
            if (Headers.Contains("appid"))
            {
                AppId = Headers.GetValues("appid").FirstOrDefault();
            }
            if (Headers.Contains("timestamp"))
            {
                Timestamp = Headers.GetValues("timestamp").FirstOrDefault();
            }
            if (Headers.Contains("nonce"))
            {
                Nonce = Headers.GetValues("nonce").FirstOrDefault();
            }
            if (Headers.Contains("signature"))
            {
                Signature = Headers.GetValues("signature").FirstOrDefault();
            }
        }
        /// <summary>
        /// 应用系统代码
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 检验参数
        /// </summary>
        /// <returns></returns>
        public bool Verify()
        {
            if (string.IsNullOrEmpty(AppId)
                || string.IsNullOrEmpty(Timestamp)
                || string.IsNullOrEmpty(Nonce)
                || string.IsNullOrEmpty(Signature))
                return false;
            return true;
        }

        public override string ToString()
        {
            return AppId + Timestamp + Nonce;
        }
    }
}
