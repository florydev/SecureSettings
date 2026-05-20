using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace FloryDev.SecureSettings.Dpapi
{
    [SupportedOSPlatform("windows")]
    public class DpapiEncryptionProvider : IEncryptDecryptService
    {
        private readonly DataProtectionScope _scope;

        public DpapiEncryptionProvider(IOptions<DpapiEncryptionSettings> settings)
        {
            _scope = settings.Value.Scope;
        }

        public string EncryptString(string plainText)
        {
            var plainBytes = Encoding.Unicode.GetBytes(plainText);
            var encryptedBytes = ProtectedData.Protect(plainBytes, null, _scope);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string DecryptString(string base64)
        {
            var encryptedBytes = Convert.FromBase64String(base64);
            var plainBytes = ProtectedData.Unprotect(encryptedBytes, null, _scope);
            return Encoding.Unicode.GetString(plainBytes);
        }

        public bool ValueIsEncrypted(string value) => SecureValueEncoding.IsEncrypted(value);
    }
}
