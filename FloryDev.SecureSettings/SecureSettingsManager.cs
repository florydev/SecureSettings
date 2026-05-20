using FloryDev.SecureSettings.ConnectionStrings;
using FloryDev.SecureSettings.Extensions;
using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FloryDev.SecureSettings
{
    public class SecureSettingsManager
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IDecryptionService _decryptionService;

        public SecureSettingsManager(IEncryptionService encryptionService, IDecryptionService decryptionService, IConfiguration configuration)
        {
            _encryptionService = encryptionService;
            _decryptionService = decryptionService;
            EncryptedConfigSetting.Encrypter = encryptionService;
            EncryptedConfigSetting.Decrypter = decryptionService;
            Extensions.ConfigurationExtensions.Encrypter = encryptionService;
            Extensions.ConfigurationExtensions.Decrypter = decryptionService;

            SecureConnectionStringConfigurationProvider.EncryptAll();
            ((IConfigurationRoot)configuration).Reload();
        }
    }
}
