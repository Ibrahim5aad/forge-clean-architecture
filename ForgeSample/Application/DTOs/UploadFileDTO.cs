using Microsoft.AspNetCore.Http;

namespace ForgeSample.Application.DTOs
{
    public class UploadFileDTO
    {
        //[ModelBinder(BinderType = typeof(FormDataJsonBinder))]
        public string bucketKey { get; set; }
        public IFormFile fileToUpload { get; set; }
        // Other properties
    }

}
