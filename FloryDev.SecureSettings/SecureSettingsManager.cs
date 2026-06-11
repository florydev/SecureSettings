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
            WireStatics(secureService, unsecureService);

            if (secureService != null)
            {
                SecureConnectionStringConfigurationProvider.EncryptAll();
                SecureSettingsFileScanner.EncryptInDirectory(environment.ContentRootPath);
            }

            ((IConfigurationRoot)configuration).Reload();
        }

        /// <summary>
        /// Wires the static Securer/Unsecurer used by <see cref="SecuredConfigSetting"/> and
        /// <see cref="Extensions.ConfigurationExtensions.GetSecuredConnectionStringAsync"/>, then
        /// reloads <paramref name="configuration"/> so <see cref="SecureConnectionStringConfigurationProvider"/>
        /// re-runs <c>TryGet</c> and decrypts secured connection string values.
        /// Use this directly (instead of resolving <see cref="SecureSettingsManager"/> from DI) in
        /// contexts where the full app host isn't available — e.g. an <c>IDesignTimeDbContextFactory</c>
        /// used by <c>dotnet ef</c> migrations.
        /// </summary>
        public static void Bootstrap(IConfiguration configuration, ISecureUnsecureService provider)
        {
            WireStatics(provider, provider);
            ((IConfigurationRoot)configuration).Reload();
        }

        private static void WireStatics(ISecureService? secureService, IUnsecureService unsecureService)
        {
            if (secureService != null)
            {
                SecuredConfigSetting.Securer = secureService;
                Extensions.ConfigurationExtensions.Securer = secureService;
            }

            SecuredConfigSetting.Unsecurer = unsecureService;
            Extensions.ConfigurationExtensions.Unsecurer = unsecureService;
        }
    }
}
