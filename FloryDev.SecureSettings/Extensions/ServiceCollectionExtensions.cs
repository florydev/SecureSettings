using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.Writer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FloryDev.SecureSettings.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string DEFAULT_SETTINGS_FILE = "appsettingsx.json";
        public static void ConfigureWritable<T>(
        this IServiceCollection services,
        IConfigurationSection section,
        string file = DEFAULT_SETTINGS_FILE) where T : class, new()
        {
            services.Configure<T>(section); // Fix: Use section.Bind to convert IConfigurationSection to Action<T>
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var environment = provider.GetService<IHostEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, configuration, section.Key, file);
            });
        }
    }
}
