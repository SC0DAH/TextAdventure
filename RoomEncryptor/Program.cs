using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

class RoomEncryptor
{
    static void Main()
    {
        Console.WriteLine("Room Encryptor");

        EncryptRoom("room_startroom.txt", "room_startroom.enc");
        EncryptRoom("room_keyroom.txt", "room_keyroom.enc");

        Console.WriteLine("Finished.");
        Console.ReadKey();
    }

    static void EncryptRoom(string inputFile, string outputFile)
    {
        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"File not found: {inputFile}");
            return;
        }

        try
        {
            byte[] data = File.ReadAllBytes(inputFile);

            using RSA rsa = RSA.Create(2048);
            var request = new CertificateRequest(
                "CN=RoomCert",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            var cert = request.CreateSelfSigned(
                DateTimeOffset.Now,
                DateTimeOffset.Now.AddYears(5)
            );

            var content = new ContentInfo(data);
            var cms = new EnvelopedCms(content);
            cms.Encrypt(new CmsRecipient(cert));

            File.WriteAllBytes(outputFile, cms.Encode());

            Console.WriteLine($"Encrypted: {inputFile} -> {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Encryption failed: {ex.Message}");
        }
    }
}
