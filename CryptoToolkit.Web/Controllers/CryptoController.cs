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
        private readonly Sha256Service _sha256Service;        public CryptoController(AesService aesService, Sha256Service sha256Service)
        {
            _aesService = aesService;
            _sha256Service = sha256Service;
        }

        [HttpGet]
        public IActionResult Encrypt() => View();        [HttpPost]
        public IActionResult Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                ModelState.AddModelError("", "Metin alanı zorunludur.");
                return View();
            }
            
            try
            {
                var (cipherText, key, iv) = _aesService.Encrypt(plainText);
                return View(new AesModel { PlainText = plainText, CipherText = cipherText, Key = key, IV = iv });
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"ArgumentException in Encrypt: {ex.Message}"); 
                ModelState.AddModelError("", "Şifreleme sırasında geçersiz bir parametre girildi. Lütfen girdilerinizi kontrol edin.");
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in Encrypt: {ex.Message}");
                ModelState.AddModelError("", "Şifreleme sırasında beklenmedik bir hata oluştu. Lütfen tekrar deneyin.");
                return View();
            }
        }
        [HttpGet]
        public IActionResult Decrypt(string cipherText, string key, string iv) // Removed privateKey parameter
        {
            System.Diagnostics.Debug.WriteLine($"GET Decrypt - Parameters: cipherText={cipherText?.Length ?? 0}, key={key?.Length ?? 0}, iv={iv?.Length ?? 0}");
            
            var model = new AesModel
            {
                CipherText = cipherText,
                Key = key,
                IV = iv
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Decrypt(AesModel model) 
        {
            if (string.IsNullOrEmpty(model.CipherText))
            {
                ModelState.AddModelError("", "Şifreli metin alanı zorunludur.");
                return View(model); 
            }

            try
            {

                if (string.IsNullOrEmpty(model.Key) || string.IsNullOrEmpty(model.IV))
                {
                    ModelState.AddModelError("", "AES şifre çözme için anahtar ve IV değerleri gereklidir.");
                    return View(model);
                }

                var plainText = _aesService.Decrypt(model.CipherText, model.Key, model.IV);
                model.PlainText = plainText;
                return View(model);
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"ArgumentException in Decrypt: {ex.Message}");
                ModelState.AddModelError("", "Şifre çözme sırasında geçersiz bir parametre girildi. Anahtar, IV veya şifreli metin formatını kontrol edin.");
                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in Decrypt: {ex.Message}");
                ModelState.AddModelError("", "Şifre çözme sırasında beklenmedik bir hata oluştu. Lütfen doğru anahtar ve IV kullandığınızdan emin olun.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Hash() => View();
        [HttpPost]
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
                System.Diagnostics.Debug.WriteLine($"Exception in Hash: {ex.Message}");
                ModelState.AddModelError("", $"Özet hesaplama sırasında beklenmedik bir hata oluştu. Lütfen tekrar deneyin.");
                return View();
            }
        }
    }
}
