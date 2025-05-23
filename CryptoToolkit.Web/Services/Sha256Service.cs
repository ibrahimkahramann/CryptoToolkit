using System.Security.Cryptography;
using System.Text;

namespace CryptoToolkit.Web.Services
{
    public class Sha256Service
    {
        public string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public string ComputeHash(byte[] fileBytes)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(fileBytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
