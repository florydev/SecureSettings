using FloryDev.SecureSettings.Azure.KeyVault;
using FloryDev.SecureSettings.Extensions;
using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.ReferenceImplementation.AzureKeyVault;
using FloryDev.SecureSettings.Windows.Dpapi;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Scan all appsettings*.json for connection strings and secure them.
        // In Key Vault mode, sensitive parts of connection strings should use
        // {-FDSS:kv:SecretName} references — SecureFile() is skipped since
        // ISecureService is not registered.
        builder.Configuration.SecureConnectionStrings();

        var kvSection = builder.Configuration.GetSection(KeyVaultSecuritySettings.SectionName);
        bool useKeyVault = kvSection.Exists() && !string.IsNullOrWhiteSpace(kvSection["VaultUri"]);

        if (useKeyVault)
        {
            // --- Key Vault mode (production / staging / dev with KV access) ---
            //
            // Only IUnsecureService is registered. The app has read-only access to Key Vault.
            // Secrets must be pre-configured in Key Vault by deployment tooling.
            //
            // Authentication is handled by DefaultAzureCredential, which automatically uses:
            //   - Managed Identity (App Service, VM, AKS)
            //   - Azure CLI credentials (az login) for local dev with KV access
            //   - Visual Studio / VS Code credentials
            //   - Environment variables (AZURE_CLIENT_ID etc.) for service principals
            //
            // To use a specific credential type, register Azure.Core.TokenCredential in DI:
            //   builder.Services.AddSingleton<Azure.Core.TokenCredential>(
            //       new ClientSecretCredential(tenantId, clientId, secret));

            builder.Services.Configure<KeyVaultSecuritySettings>(kvSection);
            builder.Services.AddSingleton<IUnsecureService, KeyVaultUnsecureProvider>();

            builder.Services.ConfigureSecured<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
        }
        else
        {
            // --- Local / dev mode (no Key Vault access) ---
            //
            // Both ISecureService and IUnsecureService are registered.
            // Plain-text values in appsettings.local.json are automatically secured with
            // DPAPI on first run and written back. Each developer's credentials are tied
            // to their own Windows user account.

            builder.Configuration.AddJsonFile("appsettings.local.json", optional: false);

            builder.Services.Configure<DpapiSecuritySettings>(
                builder.Configuration.GetSection(DpapiSecuritySettings.SectionName));
            builder.Services.AddSingleton<ISecureService, DpapiSecurityProvider>();
            builder.Services.AddSingleton<IUnsecureService, DpapiSecurityProvider>();

            var localBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.local.json", optional: false);
            var local = localBuilder.Build();
            builder.Services.ConfigureSecured<AppSettings>(
                local.GetSection(AppSettings.SectionName), "appsettings.local.json");
        }

        builder.Services.AddSingleton<SecureSettingsManager>();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}
