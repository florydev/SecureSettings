﻿using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.Writer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FloryDev.SecureSettings.Extensions
{
    /// <summary>
    /// This add an extension to the ServiceCollection for IWritableOptions. This code was taken from this post here: https://learn.microsoft.com/en-us/answers/questions/609232/how-to-save-the-updates-i-made-to-appsettings-conf
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public const string DEFAULT_SETTINGS_FILE = "appsettings.json";
        public static void ConfigureSecured<T>(
        this IServiceCollection services,
        IConfigurationSection section,
        string file = DEFAULT_SETTINGS_FILE) where T : class, new()
        {
            services.Configure<T>(section); // Fix: Use section.Bind to convert IConfigurationSection to Action<T>
            services.AddTransient<ISecuredOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var environment = provider.GetService<IHostEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, configuration, section.Key, file);
            });
        }
    }
}
