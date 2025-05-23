using System.Security.Cryptography;
using System.Text;

namespace CryptoToolkit.Web.Services
{
    public class RsaService
    {
        private const int KeySize = 2048;

        public (string publicKey, string privateKey) GenerateKeys()
        {
            using var rsa = RSA.Create(KeySize);
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            return (publicKey, privateKey);
        }

        public string Encrypt(string plainText, string publicKey)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentException("Şifrelenecek metin boş olamaz", nameof(plainText));
            }

            if (string.IsNullOrEmpty(publicKey))
            {
                throw new ArgumentException("Açık anahtar boş olamaz", nameof(publicKey));
            }

            try
            {
                using var rsa = RSA.Create();
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                var bytes = Encoding.UTF8.GetBytes(plainText);
                
                // RSA şifreleme boyut sınırlamasını kontrol edin
                int maxDataLength = (KeySize / 8) - 42; // PKCS#1 padding için gereken alan çıkarılır
                if (bytes.Length > maxDataLength)
                {
                    throw new ArgumentException($"Metin çok uzun. En fazla {maxDataLength} bayt şifrelenebilir.");
                }
                
                var encrypted = rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(encrypted);
            }
            catch (CryptographicException)
            {
                throw new ArgumentException("Şifreleme işlemi başarısız. Geçersiz açık anahtar.");
            }
            catch (FormatException)
            {
                throw new ArgumentException("Açık anahtar geçerli Base64 formatında değil.");
            }
        }        public string Decrypt(string cipherText, string privateKey)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentException("Şifreli metin boş olamaz", nameof(cipherText));
            }

            if (string.IsNullOrEmpty(privateKey))
            {
                throw new ArgumentException("Özel anahtar boş olamaz", nameof(privateKey));
            }

            try
            {
                using var rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                var bytes = Convert.FromBase64String(cipherText);
                var decrypted = rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (CryptographicException)
            {
                throw new ArgumentException("Deşifreleme işlemi başarısız. Yanlış özel anahtar kullanılmış olabilir.");
            }
            catch (FormatException)
            {
                throw new ArgumentException("Şifreli metin veya özel anahtar geçerli Base64 formatında değil.");
            }
        }
    }
}
