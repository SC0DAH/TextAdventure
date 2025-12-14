    using System.Security.Cryptography;
    using System.Text;

    namespace AdventureAPI
    {
        public static class PasswordHasher
        {
            // random salt genereren
            public static string GenerateSalt(int size = 16)
            {
                var bytes = new byte[size];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }

            // hash paswoord + salt
            public static string HashWithSalt(string password, string salt)
            {
                if (password == null) password = "";
                var combined = $"{salt}:{password}";
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(combined);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }

            public static bool SlowEquals(string a, string b)
            {
                if (a == null || b == null) return false;
                var ba = Encoding.UTF8.GetBytes(a);
                var bb = Encoding.UTF8.GetBytes(b);
                return CryptographicOperations.FixedTimeEquals(ba, bb); // checkt beide en voorkomt timing attacks
            }
        }
    }
