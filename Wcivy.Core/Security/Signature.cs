using System.Text;

namespace Wcivy.Core.Security
{
    public class Signature
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="str">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        public static string Sign(string str, string key)
        {
            var result = new StringBuilder();
            //签名数据
            var signStr = str + key;
            // 进行MD5编码　
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(signStr));
            foreach (var b in bytes)
            {
                result.Append(b.ToString("X2"));
            }
            return result.ToString();
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="str">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="sign">签名结果</param>
        /// <returns>验证结果</returns>
        public static bool Verify(string str, string key, string sign)
        {
            var mysign = Sign(str, key);
            return mysign.Equals(sign, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
