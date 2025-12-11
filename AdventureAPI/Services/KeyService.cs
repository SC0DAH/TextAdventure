using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace AdventureAPI.Services
{
    public class KeyService
    {
        private readonly ConcurrentDictionary<string, byte[]> _userKeys = new(); // bewaart per gebruiker de keyshare in memory
        private const int KeyLength = 32; // lengte van key

        public string GetOrCreateKeyForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username required");

            if (_userKeys.TryGetValue(username, out var existingKey))
                return Convert.ToBase64String(existingKey); // als gebruiker al key heeft geven we die terug

            // maken nieuwe key
            var keyBytes = new byte[KeyLength];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(keyBytes);

            _userKeys[username] = keyBytes;
            return Convert.ToBase64String(keyBytes);
        }
    }
}
