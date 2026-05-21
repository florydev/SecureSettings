using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace FloryDev.SecureSettings.Windows.Dpapi
{
    [SupportedOSPlatform("windows")]
    public class DpapiSecurityProvider : ISecureUnsecureService
    {
        private readonly DataProtectionScope _scope;

        public DpapiSecurityProvider(IOptions<DpapiSecuritySettings> settings)
        {
            _scope = settings.Value.Scope;
        }

        public string Secure(string value)
        {
            var plainBytes = Encoding.Unicode.GetBytes(value);
            var securedBytes = ProtectedData.Protect(plainBytes, null, _scope);
            return Convert.ToBase64String(securedBytes);
        }

        public string Unsecure(string securedValue)
        {
            var securedBytes = Convert.FromBase64String(securedValue);
            var plainBytes = ProtectedData.Unprotect(securedBytes, null, _scope);
            return Encoding.Unicode.GetString(plainBytes);
        }

        public bool IsSecured(string value) => SecureValueEncoding.IsSecured(value);
    }
}
