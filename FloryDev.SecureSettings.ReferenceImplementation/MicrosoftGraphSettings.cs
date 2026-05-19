namespace FloryDev.SecureSettings.ReferenceImplementation
{
    public class MicrosoftGraphSettings
    {
        public static string SectionName = "Email:MicrosoftGraph";
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public EncryptedConfigSetting ClientSecret { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
