using System.Security.Cryptography;
using System.Text;

namespace CryptoToolkit.Web.Services
{
    public class Sha256Service
    {
        public string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Özet hesaplanacak metin boş olamaz", nameof(input));
            }

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public string ComputeHash(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                throw new ArgumentException("Özet hesaplanacak dosya içeriği boş olamaz", nameof(fileBytes));
            }

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(fileBytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
        
        /// <summary>
        /// Dosya yolundan doğrudan SHA-256 özeti hesaplar.
        /// Büyük dosyalar için daha etkilidir çünkü tüm dosyayı belleğe yüklemez.
        /// </summary>
        /// <param name="filePath">Özeti hesaplanacak dosyanın yolu</param>
        /// <returns>SHA-256 özeti (hex formatında)</returns>
        public string ComputeHashFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("Dosya yolu boş olamaz", nameof(filePath));
            }
            
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("Dosya bulunamadı", filePath);
            }

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
