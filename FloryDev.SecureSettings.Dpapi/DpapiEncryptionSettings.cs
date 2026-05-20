using System.Security.Cryptography;

namespace FloryDev.SecureSettings.Dpapi
{
    public class DpapiEncryptionSettings
    {
        public const string SectionName = "DpapiSettings";

        /// <summary>
        /// CurrentUser: only the same Windows user on the same machine can decrypt.
        /// LocalMachine: any process on the machine can decrypt — use for service accounts.
        /// </summary>
        public DataProtectionScope Scope { get; set; } = DataProtectionScope.CurrentUser;
    }
}
