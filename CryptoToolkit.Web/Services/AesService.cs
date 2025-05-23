using System.Security.Cryptography;
using System.Text;

namespace CryptoToolkit.Web.Services
{
    public class AesService
    {
        public (string cipherText, string key, string iv) Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.GenerateKey();
            aes.GenerateIV();
            var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return (Convert.ToBase64String(cipherBytes), Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
        }

        public string Decrypt(string cipherText, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);
            var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
