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
        public IActionResult Encrypt(string plainText) // Removed method parameter
        {
            if (string.IsNullOrEmpty(plainText))
            {
                ModelState.AddModelError("", "Metin alanı zorunludur."); // Simplified error message
                return View();
            }
            
            try
            {
                // Always use AES
                var (cipherText, key, iv) = _aesService.Encrypt(plainText);
                return View(new AesModel { PlainText = plainText, CipherText = cipherText, Key = key, IV = iv });
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
        public IActionResult Decrypt(string cipherText, string key, string iv) // Removed privateKey parameter
        {
            // Log parameter values to debug
            System.Diagnostics.Debug.WriteLine($"GET Decrypt - Parameters: cipherText={cipherText?.Length ?? 0}, key={key?.Length ?? 0}, iv={iv?.Length ?? 0}");
            
            // Pass the parameters to the view, so they can be pre-filled in the form
            var model = new AesModel
            {
                CipherText = cipherText,
                Key = key,
                IV = iv
            };
            return View(model);
        }

        [HttpPost]
        // To avoid overload conflict and to better suit form submissions, use a model parameter.
        // The form in Decrypt.cshtml should have its inputs named to match AesModel properties (CipherText, Key, IV).
        public IActionResult Decrypt(AesModel model) 
        {
            // The 'method' parameter is no longer needed as we are AES-only.
            // Ensure CipherText, Key, and IV are part of the AesModel and are bound from the form.
            if (string.IsNullOrEmpty(model.CipherText))
            {
                ModelState.AddModelError("", "Şifreli metin alanı zorunludur.");
                return View(model); // Pass the model back to retain user input
            }
            
            try
            {
                // Always use AES
                if (string.IsNullOrEmpty(model.Key) || string.IsNullOrEmpty(model.IV))
                {
                    ModelState.AddModelError("", "AES şifre çözme için anahtar ve IV değerleri gereklidir.");
                    return View(model); // Pass the model back
                }
                
                var plainText = _aesService.Decrypt(model.CipherText, model.Key, model.IV);
                // Update the model with the decrypted plainText to display it.
                model.PlainText = plainText;
                return View(model); // Return view with the model containing plainText and original inputs
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Şifre çözme sırasında bir hata oluştu. Lütfen doğru anahtar kullandığınızdan emin olun.");
                return View(model);
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
