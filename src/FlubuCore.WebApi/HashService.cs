using System;
using System.Security.Cryptography;

namespace FlubuCore.WebApi
{
    public class HashService : IHashService
    {
        public const int SaltByteSize = 26;
        public const int HashByteSize = 30;
        public const int Pbkdf2Iterations = 1000;
        public const int IterationIndex = 0;
        public const int SaltIndex = 1;
        public const int Pbkdf2Index = 2;

        public string Hash(string valueToHash)
        {
            using (var cryptoProvider = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[SaltByteSize];
                cryptoProvider.GetBytes(salt);

                var hash = GetPbkdf2Bytes(valueToHash, salt, Pbkdf2Iterations, HashByteSize);
                return Pbkdf2Iterations + ":" +
                       Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(hash);
            }
        }

        public bool Validate(string valueToValidate, string correctHash)
        {
            char delimiter = ':';
            var split = correctHash.Split(delimiter);
            var iterations = int.Parse(split[IterationIndex]);
            var salt = Convert.FromBase64String(split[SaltIndex]);
            var hash = Convert.FromBase64String(split[Pbkdf2Index]);

            var testHash = GetPbkdf2Bytes(valueToValidate, salt, iterations, hash.Length);
            if (SlowEquals(hash, testHash))
            {
                return true;
            }

            return false;
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }

        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}
