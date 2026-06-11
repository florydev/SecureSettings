using FloryDev.SecureSettings.Extensions;
using FloryDev.SecureSettings.Windows.Dpapi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace FloryDev.SecureSettings.ReferenceImplementation.EntityFramework
{
    /// <summary>
    /// Lets `dotnet ef` create an <see cref="ApplicationDbContext"/> at design time without
    /// building the full app host. This is required because the app's connection string is
    /// secured (DPAPI-encrypted) in appsettings*.json — without this factory, `dotnet ef`
    /// would read the still-encrypted user id/password and fail to connect.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.local.json", optional: true)
                .SecureConnectionStrings()
                .Build();

            var dpapiSettings = configuration.GetSection(DpapiSecuritySettings.SectionName).Get<DpapiSecuritySettings>()
                ?? new DpapiSecuritySettings();
            var provider = new DpapiSecurityProvider(Options.Create(dpapiSettings));

            SecureSettingsManager.Bootstrap(configuration, provider);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MainConnection"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
