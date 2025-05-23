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
        private readonly Sha256Service _sha256Service;        public CryptoController(AesService aesService, RsaService rsaService, Sha256Service sha256Service)
        {
            _aesService = aesService;
            _rsaService = rsaService;
            _sha256Service = sha256Service;
        }

        [HttpGet]
        public IActionResult Encrypt() => View();        [HttpPost]
        public IActionResult Encrypt(string method, string plainText)
        {
            if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(plainText))
            {
                ModelState.AddModelError("", "Şifreleme yöntemi ve metin alanları zorunludur.");
                return View();
            }
            
            try
            {
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
                else
                {
                    ModelState.AddModelError("", "Geçersiz şifreleme yöntemi seçildi.");
                    return View();
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Şifreleme sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Decrypt() => View();        [HttpPost]
        public IActionResult Decrypt(string method, string cipherText, string key, string iv, string privateKey)
        {
            if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(cipherText))
            {
                ModelState.AddModelError("", "Şifre çözme yöntemi ve şifreli metin alanları zorunludur.");
                return View();
            }
            
            try
            {
                if (method == "AES")
                {
                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
                    {
                        ModelState.AddModelError("", "AES şifre çözme için anahtar ve IV değerleri gereklidir.");
                        return View();
                    }
                    
                    var plainText = _aesService.Decrypt(cipherText, key, iv);
                    return View(new AesModel { PlainText = plainText, CipherText = cipherText, Key = key, IV = iv });
                }
                else if (method == "RSA")
                {
                    if (string.IsNullOrEmpty(privateKey))
                    {
                        ModelState.AddModelError("", "RSA şifre çözme için özel anahtar gereklidir.");
                        return View();
                    }
                    
                    var plainText = _rsaService.Decrypt(cipherText, privateKey);
                    return View(new RsaModel { PlainText = plainText, CipherText = cipherText, PrivateKey = privateKey });
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz şifre çözme yöntemi seçildi.");
                    return View();
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Şifre çözme sırasında bir hata oluştu. Lütfen doğru anahtar kullandığınızdan emin olun.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Hash() => View();        [HttpPost]
        public async Task<IActionResult> Hash(string inputText, Microsoft.AspNetCore.Http.IFormFile uploadedFile)
        {
            try
            {
                if (!string.IsNullOrEmpty(inputText))
                {
                    var hash = _sha256Service.ComputeHash(inputText);
                    return View(new Sha256Model { InputText = inputText, HashResult = hash });
                }
                else if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    // Dosya boyutu kontrolü - çok büyük dosyalar için bellek kullanımını sınırlayalım
                    // 10 MB üzeri dosyalar için uyarı verelim
                    if (uploadedFile.Length > 10 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Dosya boyutu çok büyük (10MB'dan büyük). Daha küçük bir dosya seçin.");
                        return View();
                    }
                    
                    using var ms = new MemoryStream();
                    await uploadedFile.CopyToAsync(ms);
                    var hash = _sha256Service.ComputeHash(ms.ToArray());
                    return View(new FileUploadModel { UploadedFile = uploadedFile, HashResult = hash });
                }
                else
                {
                    ModelState.AddModelError("", "Lütfen metin girin veya bir dosya yükleyin.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Özet hesaplama sırasında bir hata oluştu: {ex.Message}");
                return View();
            }
        }
    }
}
