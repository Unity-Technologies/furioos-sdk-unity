using System;
using System.Security.Cryptography;
using System.Text;

namespace DdpClient
{
    public class DdpUtil
    {
        public static string GetRandomId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public static byte[] GetBytesFromBase64(string base64)
        {
            return Convert.FromBase64String(base64);
        }

        public static string GetBase64FromBytes(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static DateTime MillisecondsToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTimeStamp);
        }

        public static long DateTimeToMilliseconds(DateTime date)
        {
            // Unix timestamp is seconds past epoch
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        // ReSharper disable once InconsistentNaming
        public static string GetSHA256(string text)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}