using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace WarehouseITSystem.Utils
{
    internal class PasswordHash
    {
        public static string GetHash(char[] password)
        {
            using SHA256 crypt = SHA256.Create();
            var hash = new StringBuilder();

            var iter = int.Parse(ConfigurationManager.AppSettings["hashIterations"]!);
            var saltArr = ConfigurationManager.AppSettings["passwordSalt"]!.ToCharArray();
            var newCharArr = new char[password.Length + saltArr.Length];
            Array.Copy(password, newCharArr, password.Length);
            Array.Copy(saltArr, 0, newCharArr, password.Length, saltArr.Length);
            byte[] crypto = Encoding.UTF8.GetBytes(newCharArr);
            for (int i = 0; i < iter; i++)
            {
                crypto = crypt.ComputeHash(crypto);
            }

            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}
