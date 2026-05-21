using FloryDev.SecureSettings.ConnectionStrings;
using Microsoft.Extensions.Configuration;

namespace FloryDev.SecureSettings.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Secures connection strings in all appsettings*.json files found in the current directory.
        /// Pass a specific filename to target a single file instead.
        /// </summary>
        public static IConfigurationBuilder SecureConnectionStrings(
            this IConfigurationBuilder builder,
            string? file = null)
        {
            if (file != null)
                return builder.Add(new SecureConnectionStringConfigurationSource { File = file });

            foreach (var path in Directory.GetFiles(Directory.GetCurrentDirectory(), "appsettings*.json"))
                builder.Add(new SecureConnectionStringConfigurationSource { File = Path.GetFileName(path) });

            return builder;
        }
    }
}
