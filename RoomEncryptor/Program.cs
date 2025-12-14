using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

class Program
{
    static void Main()
    {
        //string inputFile = "room_startroom.txt";
        //string outputFile = "room_startroom.enc";
        string inputFile = "room_keyroom.txt";
        string outputFile = "room_keyroom.enc";

        byte[] data = File.ReadAllBytes(inputFile);

        // certificaat maken
        using RSA rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            "cn=RoomCertificate",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(1));

        // CMS encryptie
        ContentInfo contentInfo = new ContentInfo(data);
        EnvelopedCms cms = new EnvelopedCms(contentInfo);

        CmsRecipient recipient = new CmsRecipient(certificate);
        cms.Encrypt(recipient);

        File.WriteAllBytes(outputFile, cms.Encode());

        Console.WriteLine("Bestand succesvol versleuteld!");
    }
}
