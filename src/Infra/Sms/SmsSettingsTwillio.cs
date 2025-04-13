namespace MockExams.Infra.Sms
{
    public class SmsSettingsTwillio
    {
        public bool IsActive { get; set; }
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string MessagingServiceSid { get; set; }
    }

    public class SmsSettings
    {
        public bool IsActive { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
