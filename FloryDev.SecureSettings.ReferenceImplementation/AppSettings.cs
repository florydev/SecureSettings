namespace FloryDev.SecureSettings.ReferenceImplementation
{
    public class AppSettings
    {
        public static string SectionName = "AppSettings";
        public string Username { get; set; }
        public SecuredConfigSetting Password { get; set; }
    }
}
