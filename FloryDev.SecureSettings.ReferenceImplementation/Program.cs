using FloryDev.SecureSettings.Extensions;
using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.WindowsEncryption;
using System;
using System.Collections.Generic;
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
            builder.Services.ConfigureWritable<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName), "appsettings.json");
            builder.Services.AddSingleton<IEncryptionService, EncryptionProvider>();
            builder.Services.AddSingleton<IDecryptionService, EncryptionProvider>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}
