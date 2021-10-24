using Microsoft.AspNetCore.Http;

namespace ForgeSample.Models
{

    public class UploadFile
    {
        public string bucketKey { get; set; }
        public IFormFile fileToUpload { get; set; }
    }

}
