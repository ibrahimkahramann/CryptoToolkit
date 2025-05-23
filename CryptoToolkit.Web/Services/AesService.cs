using System.Security.Cryptography;
using System.Text;

namespace CryptoToolkit.Web.Services
{
    public class AesService
    {
        public (string cipherText, string key, string iv) Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentException("Şifrelenecek metin boş olamaz", nameof(plainText));
            }

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateKey();
            aes.GenerateIV();
            var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return (Convert.ToBase64String(cipherBytes), Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
        }        public string Decrypt(string cipherText, string key, string iv)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentException("Şifreli metin boş olamaz", nameof(cipherText));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Anahtar boş olamaz", nameof(key));
            }

            if (string.IsNullOrEmpty(iv))
            {
                throw new ArgumentException("IV boş olamaz", nameof(iv));
            }

            try
            {
                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);
                var decryptor = aes.CreateDecryptor();
                var cipherBytes = Convert.FromBase64String(cipherText);
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Şifreli metin, anahtar veya IV geçerli Base64 formatında değil");
            }
            catch (CryptographicException)
            {
                throw new ArgumentException("Deşifreleme işlemi başarısız. Yanlış anahtar veya IV kullanılmış olabilir.");
            }
        }
    }
}
