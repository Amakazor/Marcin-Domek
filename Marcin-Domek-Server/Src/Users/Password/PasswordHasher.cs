using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Marcin_Domek_Server.Src.Users.Password
{
    internal static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;

        public static string Hash(string password, int iterations)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            string base64Hash = Convert.ToBase64String(hashBytes);

            return string.Format("$DOMEK$V1${0}${1}", iterations, base64Hash);
        }

        public static string Hash(string password)
        {
            return Hash(password, 100000);
        }

        public static bool IsHashSupported(string hashString)
        {
            return hashString.Contains("$DOMEK$V1$");
        }

        public static bool Verify(string password, string hashedPassword)
        {
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hashtype is not supported");
            }

            string[] splittedHashString = hashedPassword.Replace("$DOMEK$V1$", "").Split('$');
            int iterations = int.Parse(splittedHashString[0]);
            string base64Hash = splittedHashString[1];

            byte[] hashBytes = Convert.FromBase64String(base64Hash);

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
