using FloryDev.SecureSettings.ConnectionStrings;
using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FloryDev.SecureSettings
{
    public class SecureSettingsManager
    {
        private readonly ISecureService _secureService;
        private readonly IUnsecureService _unsecureService;

        public SecureSettingsManager(ISecureService secureService, IUnsecureService unsecureService, IConfiguration configuration, IHostEnvironment environment)
        {
            _secureService = secureService;
            _unsecureService = unsecureService;
            SecuredConfigSetting.Securer = secureService;
            SecuredConfigSetting.Unsecurer = unsecureService;
            Extensions.ConfigurationExtensions.Securer = secureService;
            Extensions.ConfigurationExtensions.Unsecurer = unsecureService;

            SecureConnectionStringConfigurationProvider.EncryptAll();
            SecureSettingsFileScanner.EncryptInDirectory(environment.ContentRootPath);
            ((IConfigurationRoot)configuration).Reload();
        }
    }
}
