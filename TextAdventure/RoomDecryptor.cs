using System.Linq;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TextAdventure
{
    public static class RoomDecryptor
    {

        public static byte[] GenerateDecryptionKey(string keyshare, string passphrase)
        {
            if (string.IsNullOrEmpty(keyshare) || string.IsNullOrEmpty(passphrase))
                throw new ArgumentException("Keyshare and passphrase required");

            var combined = $"{keyshare}:{passphrase}";
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        }
        public static string? TryDecryptRoom(string encryptedFilePath)
        {
            try
            {
                if (!File.Exists(encryptedFilePath))
                    return null;

                byte[] encryptedData = File.ReadAllBytes(encryptedFilePath);

                EnvelopedCms cms = new EnvelopedCms();
                cms.Decode(encryptedData);

                // Probeer te decrypten met beschikbare certificaten
                cms.Decrypt();

                return Encoding.UTF8.GetString(cms.ContentInfo.Content);
            }
            catch (CryptographicException)
            {
                // Decryptie gefaald - verkeerde sleutel of certificaat niet beschikbaar
                return null;
            }
            catch (Exception)
            {
                // Andere fout
                return null;
            }
        }

        public static Room? DecryptAndParseRoom(string encryptedFilePath, string keyshare, string passphrase)
        {
            try
            {
                // Genereer key
                var key = GenerateDecryptionKey(keyshare, passphrase);

                // Decrypt
                var content = TryDecryptRoom(encryptedFilePath);
                if (content == null)
                    return null;

                // Parse content
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2)
                    return null;

                string name = lines[0].Trim();
                string description = string.Join(" ", lines.Skip(1)).Trim();

                return new Room(name, description);
            }
            catch
            {
                return null;
            }
        }
    }
}