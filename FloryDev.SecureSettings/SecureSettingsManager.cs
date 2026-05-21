using FloryDev.SecureSettings.ConnectionStrings;
using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FloryDev.SecureSettings
{
    public class SecureSettingsManager
    {
        public SecureSettingsManager(
            IEnumerable<ISecureService> secureServices,
            IUnsecureService unsecureService,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            // ISecureService is optional — a read-only provider (e.g. Key Vault) registers
            // only IUnsecureService. Auto-securing of plain-text values is skipped when absent.
            var secureService = secureServices.FirstOrDefault();
            if (secureService != null)
            {
                SecuredConfigSetting.Securer = secureService;
                Extensions.ConfigurationExtensions.Securer = secureService;
            }

            SecuredConfigSetting.Unsecurer = unsecureService;
            Extensions.ConfigurationExtensions.Unsecurer = unsecureService;

            if (secureService != null)
            {
                SecureConnectionStringConfigurationProvider.EncryptAll();
                SecureSettingsFileScanner.EncryptInDirectory(environment.ContentRootPath);
            }

            ((IConfigurationRoot)configuration).Reload();
        }
    }
}
