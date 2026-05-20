using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace FloryDev.SecureSettings.WindowsEncryption
{
    [SupportedOSPlatform("windows")]
    public class EncryptionProvider : IEncryptDecryptService
    {
        private WindowsEncryptionSettings Settings { get; set; }

        public EncryptionProvider(IOptions<WindowsEncryptionSettings> settings)
        {
            Settings = settings.Value;
        }

        public string DecryptString(string base64)
        {
            var cipherBytes = Convert.FromBase64String(base64);
            var csp = GetProviderFromContainerKey(Settings.KeyContainerName);
            var plainBytes = csp.Decrypt(cipherBytes, false);
            return System.Text.Encoding.Unicode.GetString(plainBytes);
        }

        public string EncryptString(string plainText)
        {
            var plainBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
            var csp = GetProviderFromContainerKey(Settings.KeyContainerName);
            var cipherBytes = csp.Encrypt(plainBytes, false);
            return Convert.ToBase64String(cipherBytes);
        }

        public bool ValueIsEncrypted(string value) => SecureValueEncoding.IsEncrypted(value);

        public static RSACryptoServiceProvider GetProviderFromContainerKey(string containerName)
        {
            var cp = new CspParameters { KeyContainerName = containerName };
            return new RSACryptoServiceProvider(cp);
        }
    }
}
