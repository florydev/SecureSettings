using System.Security.Cryptography;

namespace FloryDev.SecureSettings.Windows.Dpapi
{
    public class DpapiSecuritySettings
    {
        public const string SectionName = "DpapiSettings";

        /// <summary>
        /// CurrentUser: only the same Windows user on the same machine can access secured values.
        /// LocalMachine: any process on the machine can access them — use for service accounts.
        /// </summary>
        public DataProtectionScope Scope { get; set; } = DataProtectionScope.CurrentUser;
    }
}
