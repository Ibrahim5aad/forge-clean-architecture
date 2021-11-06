using static Autodesk.Forge.Model.PostBucketsPayload;

namespace ForgeSample.Application.DTOs
{
    public class BucketDTO
    {
        public string bucketKey { get; set; }
        public PolicyKeyEnum policyKey { get; set; }
    }

}
