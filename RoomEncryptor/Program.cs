using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

class RoomEncryptor
{
    static void Main()
    {
        Console.WriteLine("Room Encryptor");

        Console.Write("Enter path to plaintext room file (e.g., room_startroom.txt): ");
        string inputFile = Console.ReadLine()!.Trim();

        if (!File.Exists(inputFile))
        {
            Console.WriteLine("File does not exist.");
            return;
        }

        Console.Write("Enter output encrypted file path (e.g., room_startroom.enc): ");
        string outputFile = Console.ReadLine()!.Trim();

        try
        {
            byte[] data = File.ReadAllBytes(inputFile);

            // Self-signed certificate (X.509)
            using RSA rsa = RSA.Create(2048);
            var request = new CertificateRequest(
                "cn=RoomCertificate",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            var certificate = request.CreateSelfSigned(
                DateTimeOffset.Now,
                DateTimeOffset.Now.AddYears(5));

            // CMS encryptie
            ContentInfo contentInfo = new ContentInfo(data);
            EnvelopedCms cms = new EnvelopedCms(contentInfo);
            CmsRecipient recipient = new CmsRecipient(certificate);
            cms.Encrypt(recipient);

            File.WriteAllBytes(outputFile, cms.Encode());

            Console.WriteLine("Room successfully encrypted!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Encryption failed: {ex.Message}");
        }
    }
}
