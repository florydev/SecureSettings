using FloryDev.SecureSettings.ConnectionStrings;
using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FloryDev.SecureSettings
{
    public class SecureSettingsManager
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IDecryptionService _decryptionService;

        public SecureSettingsManager(IEncryptionService encryptionService, IDecryptionService decryptionService, IConfiguration configuration, IHostEnvironment environment)
        {
            _encryptionService = encryptionService;
            _decryptionService = decryptionService;
            EncryptedConfigSetting.Encrypter = encryptionService;
            EncryptedConfigSetting.Decrypter = decryptionService;
            Extensions.ConfigurationExtensions.Encrypter = encryptionService;
            Extensions.ConfigurationExtensions.Decrypter = decryptionService;

            SecureConnectionStringConfigurationProvider.EncryptAll();
            SecureSettingsFileScanner.EncryptInDirectory(environment.ContentRootPath);
            ((IConfigurationRoot)configuration).Reload();
        }
    }
}
