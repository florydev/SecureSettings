using FloryDev.SecureSettings.ConnectionStrings;
using Microsoft.Extensions.Configuration;

namespace FloryDev.SecureSettings.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder SecureConnectionStrings(
            this IConfigurationBuilder builder,
            string file = ServiceCollectionExtensions.DEFAULT_SETTINGS_FILE)
        {
            return builder.Add(new SecureConnectionStringConfigurationSource { File = file });
        }
    }
}
