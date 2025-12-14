using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace AdventureAPI.Services
{
    public class KeyService
    {
        // bewaart per roomId een dictionary van username → key
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>> _roomKeys = new();
        private const int KeyLength = 32;

        public string GetOrCreateKeyForUser(string roomId, string username)
        {
            if (string.IsNullOrEmpty(roomId))
                throw new ArgumentException("roomId required");

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username required");

            // haal of maak dictionary voor room
            var userKeys = _roomKeys.GetOrAdd(roomId, _ => new ConcurrentDictionary<string, byte[]>());

            // check of user al key heeft
            if (userKeys.TryGetValue(username, out var existingKey))
                return Convert.ToBase64String(existingKey);

            // anders maak nieuwe key
            var keyBytes = new byte[KeyLength];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(keyBytes);

            userKeys[username] = keyBytes;
            return Convert.ToBase64String(keyBytes);
        }
    }
}
