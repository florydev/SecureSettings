using FloryDev.SecureSettings.Extensions;
using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.WindowsEncryption;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace FloryDev.SecureSettings.ReferenceImplementation
{
    class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.Configure<WindowsEncryptionSettings>(builder.Configuration.GetSection(WindowsEncryptionSettings.SectionName));
            
            //This is just an example of how you might determine when to use shadow settings
            if (Debugger.IsAttached)
            {
                //The purpose of this in the implementation is in a environment with a number of developers they can put the secured information
                //in a shadow.json file that is not checked into source control. This way the developers can have their own settings, that
                //are encypted, and not stepping over others.
  
                //We first use a configuration builder to access the shadow file and then use that to bind that section to the Settings object
                var shadowBuilder = new ConfigurationBuilder()
                    .AddJsonFile("shadow.json", optional: false);
                var shadow = shadowBuilder.Build();
                builder.Services.ConfigureSecured<AppSettings>(shadow.GetSection(AppSettings.SectionName), "shadow.json");
                //This reference example use a companion class to serialize to that matches the full appsettings file but you could just use
                //a class specific for just secured values
            }
            else
            {
                //If we are not in development we just use the base appsettings.json file
                builder.Services.ConfigureSecured<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
            }

            builder.Services.AddSingleton<IEncryptionService, EncryptionProvider>();
            builder.Services.AddSingleton<IDecryptionService, EncryptionProvider>();
            builder.Services.AddSingleton<SecureSettingsManager>();
            builder.Services.AddHostedService<Worker>();
            var host = builder.Build();
            host.Run();
        }
    }
}
