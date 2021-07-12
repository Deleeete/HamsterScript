using System;
using System.Security.Cryptography;
using System.Text;

namespace Hamster
{
    static class ExtensionMethod
    {
        public static long CalculateHash(this string str, MD5 md5)
        {
            byte[] bin = md5.ComputeHash(Encoding.Unicode.GetBytes(str));
            return BitConverter.ToInt64(bin);
        }
    }
}
