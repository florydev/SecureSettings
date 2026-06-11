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
            builder.Services.Configure<DpapiSecuritySettings>(builder.Configuration.GetSection(DpapiSecuritySettings.SectionName));

            //Scan all appsettings*.json files so connection strings are secured in every variant
            builder.Configuration.SecureConnectionStrings();

            //Each developer maintains their own appsettings.local.json (excluded from source control)
            //so their secured credentials never interfere with the shared appsettings.json.
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
            builder.Services.AddSingleton<ISecureService, DpapiSecurityProvider>();
            builder.Services.AddSingleton<IUnsecureService, DpapiSecurityProvider>();
            builder.Services.AddSingleton<SecureSettingsManager>();
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
            {
                // Resolving SecureSettingsManager wires up SecuredConfigSetting's static
                // Securer/Unsecurer. It must happen before GetConnectionString() is called,
                // otherwise the connection string still has the encrypted user id/password
                // blobs in it (this is also what dotnet ef hits when it builds the context).
                sp.GetRequiredService<SecureSettingsManager>();
                options.UseSqlServer(sp.GetRequiredService<IConfiguration>().GetConnectionString("MainConnection"));
            });
            var host = builder.Build();
            host.Run();
        }
    }
}
