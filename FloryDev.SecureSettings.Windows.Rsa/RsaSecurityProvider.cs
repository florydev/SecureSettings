using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace FloryDev.SecureSettings.Windows.Rsa
{
    [SupportedOSPlatform("windows")]
    public class RsaSecurityProvider : ISecureUnsecureService
    {
        private readonly RsaSecuritySettings _settings;

        public RsaSecurityProvider(IOptions<RsaSecuritySettings> settings)
        {
            _settings = settings.Value;
        }

        public string Secure(string value)
        {
            using var rsa = OpenOrCreateKey();
            var plainBytes = Encoding.Unicode.GetBytes(value);
            var securedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(securedBytes);
        }

        public string Unsecure(string securedValue)
        {
            using var rsa = OpenOrCreateKey();
            var securedBytes = Convert.FromBase64String(securedValue);
            var plainBytes = rsa.Decrypt(securedBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.Unicode.GetString(plainBytes);
        }

        public bool IsSecured(string value) => SecureValueEncoding.IsSecured(value);

        private RSACng OpenOrCreateKey()
        {
            var openOptions = _settings.MachineKey ? CngKeyOpenOptions.MachineKey : CngKeyOpenOptions.None;

            if (CngKey.Exists(_settings.KeyName, CngProvider.MicrosoftSoftwareKeyStorageProvider, openOptions))
            {
                var key = CngKey.Open(_settings.KeyName, CngProvider.MicrosoftSoftwareKeyStorageProvider, openOptions);
                return new RSACng(key);
            }

            var creationParams = new CngKeyCreationParameters
            {
                Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider,
                KeyCreationOptions = _settings.MachineKey ? CngKeyCreationOptions.MachineKey : CngKeyCreationOptions.None,
                KeyUsage = CngKeyUsages.Decryption,
                ExportPolicy = CngExportPolicies.None
            };
            creationParams.Parameters.Add(
                new CngProperty("Length", BitConverter.GetBytes(_settings.KeySize), CngPropertyOptions.None));

            return new RSACng(CngKey.Create(CngAlgorithm.Rsa, _settings.KeyName, creationParams));
        }
    }
}
