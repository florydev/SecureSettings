namespace FloryDev.SecureSettings.Azure.KeyVault
{
    public class KeyVaultSecuritySettings
    {
        public const string SectionName = "KeyVaultSettings";

        /// <summary>The full URI of the Azure Key Vault, e.g. https://myvault.vault.azure.net/</summary>
        public string VaultUri { get; set; } = string.Empty;

        /// <summary>
        /// Client ID of a user-assigned managed identity. Leave null to use the
        /// system-assigned identity or the first identity found by DefaultAzureCredential.
        /// </summary>
        public string? ManagedIdentityClientId { get; set; }

        /// <summary>
        /// Prevents DefaultAzureCredential from prompting an interactive browser login.
        /// Defaults to true — disable only for local developer tooling scenarios.
        /// </summary>
        public bool ExcludeInteractiveBrowserCredential { get; set; } = true;

        /// <summary>How long a retrieved secret is held in the local cache before re-fetching.</summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    }
}
