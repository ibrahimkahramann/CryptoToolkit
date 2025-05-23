using Microsoft.AspNetCore.Mvc;
using CryptoToolkit.Web.Models;
using CryptoToolkit.Web.Services;
using System.IO;
using System.Threading.Tasks;

namespace CryptoToolkit.Web.Controllers
{
    public class CryptoController : Controller
    {
        private readonly AesService _aesService;
        private readonly RsaService _rsaService;
        private readonly Sha256Service _sha256Service;

        public CryptoController()
        {
            _aesService = new AesService();
            _rsaService = new RsaService();
            _sha256Service = new Sha256Service();
        }

        [HttpGet]
        public IActionResult Encrypt() => View();

        [HttpPost]
        public IActionResult Encrypt(string method, string plainText)
        {
            if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(plainText))
                return View();
            if (method == "AES")
            {
                var (cipherText, key, iv) = _aesService.Encrypt(plainText);
                return View(new AesModel { PlainText = plainText, CipherText = cipherText, Key = key, IV = iv });
            }
            else if (method == "RSA")
            {
                var keys = _rsaService.GenerateKeys();
                var cipherText = _rsaService.Encrypt(plainText, keys.publicKey);
                return View(new RsaModel { PlainText = plainText, CipherText = cipherText, PublicKey = keys.publicKey, PrivateKey = keys.privateKey });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Decrypt() => View();

        [HttpPost]
        public IActionResult Decrypt(string method, string cipherText, string key, string iv, string privateKey)
        {
            if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(cipherText))
                return View();
            if (method == "AES")
            {
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
                    return View();
                var plainText = _aesService.Decrypt(cipherText, key, iv);
                return View(new AesModel { PlainText = plainText, CipherText = cipherText, Key = key, IV = iv });
            }
            else if (method == "RSA")
            {
                if (string.IsNullOrEmpty(privateKey))
                    return View();
                var plainText = _rsaService.Decrypt(cipherText, privateKey);
                return View(new RsaModel { PlainText = plainText, CipherText = cipherText, PrivateKey = privateKey });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Hash() => View();

        [HttpPost]
        public async Task<IActionResult> Hash(string inputText, Microsoft.AspNetCore.Http.IFormFile uploadedFile)
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                var hash = _sha256Service.ComputeHash(inputText);
                return View(new Sha256Model { InputText = inputText, HashResult = hash });
            }
            else if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await uploadedFile.CopyToAsync(ms);
                var hash = _sha256Service.ComputeHash(ms.ToArray());
                return View(new FileUploadModel { UploadedFile = uploadedFile, HashResult = hash });
            }
            return View();
        }
    }
}
