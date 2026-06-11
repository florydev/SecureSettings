using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace FloryDev.SecureSettings.Azure.KeyVault
{
    /// <summary>
    /// Read-only IUnsecureService backed by Azure Key Vault. Implements only the retrieval
    /// side — secret provisioning is an explicit deployment concern and requires no write
    /// access from the running application.
    ///
    /// Secured values in config files use the prefix "kv:" inside the standard sentinel:
    ///   {-FDSS:kv:MySecretName}
    ///
    /// Authentication uses DefaultAzureCredential by default, which covers managed identity,
    /// Azure CLI, Visual Studio, and environment-variable service principals automatically.
    /// Register a TokenCredential in DI to override with any Azure.Identity credential type.
    /// </summary>
    public class KeyVaultUnsecureProvider : IUnsecureService
    {
        public const string SecretPrefix = "kv:";

        private readonly SecretClient _client;
        private readonly TimeSpan _cacheDuration;
        private readonly ConcurrentDictionary<string, (string Value, DateTimeOffset ExpiresAt)> _cache = new();

        public KeyVaultUnsecureProvider(IOptions<KeyVaultSecuritySettings> settings, IServiceProvider serviceProvider)
        {
            var s = settings.Value;
            var credential = serviceProvider.GetService(typeof(global::Azure.Core.TokenCredential)) as global::Azure.Core.TokenCredential
                ?? new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = s.ManagedIdentityClientId,
                    ExcludeInteractiveBrowserCredential = s.ExcludeInteractiveBrowserCredential
                });

            _client = new SecretClient(new Uri(s.VaultUri), credential);
            _cacheDuration = s.CacheDuration;
        }

        public async Task<string> UnsecureAsync(string securedValue)
        {
            if (!securedValue.StartsWith(SecretPrefix, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Key Vault provider received a value that does not start with '{SecretPrefix}'. " +
                    $"Ensure the secured form in your config file is '{SecureValueEncoding.Wrap(SecretPrefix + "SecretName")}'.");

            var secretName = securedValue.Substring(SecretPrefix.Length);

            if (_cache.TryGetValue(secretName, out var cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
                return cached.Value;

            var response = await _client.GetSecretAsync(secretName);
            var value = response.Value.Value;
            _cache[secretName] = (value, DateTimeOffset.UtcNow.Add(_cacheDuration));
            return value;
        }

        public bool IsSecured(string value) => SecureValueEncoding.IsSecured(value);
    }
}
