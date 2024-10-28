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
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
            builder.Services.AddSingleton<IEncrypter, EncryptionProvider>();
            builder.Services.AddSingleton<IDecrypter, EncryptionProvider>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}
