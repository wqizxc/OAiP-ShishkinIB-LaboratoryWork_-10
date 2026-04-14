using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace laba_10.Helpers
{
    public static class PasswordHelper
    {
        private const int Iterations = 3;
        private const int MemorySizeKb = 65536;
        private const int Parallelism = 1;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль пустой!");

            var salt = RandomNumberGenerator.GetBytes(16);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = Parallelism,
                Iterations = Iterations,
                MemorySize = MemorySizeKb
            };

            var hash = argon2.GetBytes(32);
            var combined = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

            return Convert.ToBase64String(combined);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrEmpty(storedHash))
                return false;

            var combined = Convert.FromBase64String(storedHash);
            if (combined.Length < 16 + 32)
                return false;

            var salt = combined.Take(16).ToArray();
            var originalHash = combined.Skip(16).Take(32).ToArray();

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = Parallelism,
                Iterations = Iterations,
                MemorySize = MemorySizeKb
            };

            var newHash = argon2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
        }
    }
}