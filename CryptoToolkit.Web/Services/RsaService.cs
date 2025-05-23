using System.Security.Cryptography;
using System.Text;

namespace CryptoToolkit.Web.Services
{
    public class RsaService
    {
        public (string publicKey, string privateKey) GenerateKeys()
        {
            using var rsa = RSA.Create(2048);
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            return (publicKey, privateKey);
        }

        public string Encrypt(string plainText, string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText, string privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            var bytes = Convert.FromBase64String(cipherText);
            var decrypted = rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
