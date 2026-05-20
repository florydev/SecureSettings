using Microsoft.Extensions.Configuration;

namespace FloryDev.SecureSettings.ConnectionStrings
{
    public class SecureConnectionStringConfigurationSource : IConfigurationSource
    {
        public string File { get; set; } = "appsettings.json";

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            // Use current directory (content root) so write-back targets the source file,
            // not the output directory copy that gets overwritten on every build.
            var physicalPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), File));
            return new SecureConnectionStringConfigurationProvider(physicalPath);
        }
    }
}
