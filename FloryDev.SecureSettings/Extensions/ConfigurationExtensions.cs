using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FloryDev.SecureSettings.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IDecryptionService Decrypter { get; set; }
        public static IEncryptionService Encrypter { get; set; }

        /// <summary>
        /// Gets a connection string by name, throws if not found or empty.
        /// </summary>
        public static string GetSecuredConnectionString(this IConfiguration configuration, string name)
        {
            var value = configuration.GetConnectionString(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Connection string '{name}' is missing or empty.");
            }
            if (Decrypter.ValueIsEncrypted(value))
            {
                value = Decrypter.DecryptString(value);
            }
            else
            {
                value = Encrypter.EncryptString(value);
            }
            return value;
        }
    }
}
