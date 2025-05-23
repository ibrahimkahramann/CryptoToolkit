using Microsoft.AspNetCore.Http;

namespace CryptoToolkit.Web.Models
{
    public class FileUploadModel
    {
        public IFormFile? UploadedFile { get; set; }
        public string? HashResult { get; set; }
    }
}
