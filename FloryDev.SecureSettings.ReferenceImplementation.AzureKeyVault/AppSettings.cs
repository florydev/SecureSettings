namespace FloryDev.SecureSettings.ReferenceImplementation.AzureKeyVault
{
    public class AppSettings
    {
        public static string SectionName = "AppSettings";
        public string Username { get; set; }
        public SecuredConfigSetting ApiKey { get; set; }
    }
}
