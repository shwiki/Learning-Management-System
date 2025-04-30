using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace sys.Helpers
{
    public class IdentityHelper
    {
        public static string GenerateSecureTempPassword()
        {
            const int length = 12;
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var bytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = valid[bytes[i] % valid.Length];
            return new string(chars);
        }
    }
}