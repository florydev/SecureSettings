using FloryDev.SecureSettings.Windows.Dpapi;
using FloryDev.SecureSettings.Extensions;
using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.ReferenceImplementation.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.Versioning;


namespace FloryDev.SecureSettings.ReferenceImplementation
{
    class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.Configure<DpapiEncryptionSettings>(builder.Configuration.GetSection(DpapiEncryptionSettings.SectionName));

            //Always secure connection strings in appsettings.json, even in debug, so credentials
            //don't sit in plain text in a shared file
            builder.Configuration.SecureConnectionStrings();

            //Each developer maintains their own appsettings.local.json (excluded from source control)
            //so their encrypted credentials never interfere with the shared appsettings.json.
            if (Debugger.IsAttached)
            {
                //Add appsettings.local.json so its values override appsettings.json.
                //Connection strings are already covered by the SecureConnectionStrings() scan above.
                builder.Configuration.AddJsonFile("appsettings.local.json", optional: false);

                //Use a separate configuration builder to bind AppSettings so write-back targets
                //appsettings.local.json rather than the shared appsettings.json
                var localBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.local.json", optional: false);
                var local = localBuilder.Build();
                builder.Services.ConfigureSecured<AppSettings>(local.GetSection(AppSettings.SectionName), "appsettings.local.json");
            }
            else
            {
                builder.Services.ConfigureSecured<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
            }

            builder.Services.ConfigureSecured<MicrosoftGraphSettings>(builder.Configuration.GetSection(MicrosoftGraphSettings.SectionName));
            builder.Services.AddSingleton<IEncryptionService, DpapiEncryptionProvider>();
            builder.Services.AddSingleton<IDecryptionService, DpapiEncryptionProvider>();
            builder.Services.AddSingleton<SecureSettingsManager>();
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MainConnection")));
            var host = builder.Build();
            host.Run();
        }
    }
}
