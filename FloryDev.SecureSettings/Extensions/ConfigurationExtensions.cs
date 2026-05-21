using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FloryDev.SecureSettings.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IUnsecureService Unsecurer { get; set; }
        public static ISecureService Securer { get; set; }

        /// <summary>
        /// Gets a connection string by name, securing it in the config file if it is plain text.
        /// Throws if the connection string is missing or empty.
        /// </summary>
        public static string GetSecuredConnectionString(this IConfiguration configuration, string name)
        {
            var value = configuration.GetConnectionString(name);
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Connection string '{name}' is missing or empty.");

            if (Unsecurer.IsSecured(value))
                value = Unsecurer.Unsecure(value);
            else
                value = Securer.Secure(value);

            return value;
        }
    }
}
