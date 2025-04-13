namespace MockExams.Infra.AwsS3
{
    public class AwsS3Settings
    {
        public bool IsActive { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
    }
}
