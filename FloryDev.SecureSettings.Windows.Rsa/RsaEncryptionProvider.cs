using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace FloryDev.SecureSettings.Windows.Rsa
{
    [SupportedOSPlatform("windows")]
    public class RsaEncryptionProvider : IEncryptDecryptService
    {
        private readonly RsaEncryptionSettings _settings;

        public RsaEncryptionProvider(IOptions<RsaEncryptionSettings> settings)
        {
            _settings = settings.Value;
        }

        public string EncryptString(string plainText)
        {
            using var rsa = OpenOrCreateKey();
            var plainBytes = Encoding.Unicode.GetBytes(plainText);
            var encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string DecryptString(string base64)
        {
            using var rsa = OpenOrCreateKey();
            var encryptedBytes = Convert.FromBase64String(base64);
            var plainBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.Unicode.GetString(plainBytes);
        }

        public bool ValueIsEncrypted(string value) => SecureValueEncoding.IsEncrypted(value);

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
