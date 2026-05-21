namespace FloryDev.SecureSettings.Windows.Rsa
{
    public class RsaEncryptionSettings
    {
        public const string SectionName = "RsaEncryptionSettings";

        /// <summary>
        /// Name of the CNG key in the Windows Key Storage Provider.
        /// The key is created automatically on first use if it does not exist.
        /// </summary>
        public string KeyName { get; set; } = "FloryDev.SecureSettings";

        /// <summary>
        /// RSA key size in bits. 2048 is the recommended minimum; 4096 for higher security.
        /// Only applies when a new key is created — changing this after a key exists has no effect.
        /// </summary>
        public int KeySize { get; set; } = 2048;

        /// <summary>
        /// False (default): key is stored in the current user's key store — only the same user
        /// on the same machine can decrypt. True: machine-level key store — any process on the
        /// machine can decrypt, suitable for Windows service accounts.
        /// </summary>
        public bool MachineKey { get; set; } = false;
    }
}
