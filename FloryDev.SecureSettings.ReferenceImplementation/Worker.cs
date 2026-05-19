using FloryDev.SecureSettings.Interfaces;
using FloryDev.SecureSettings.ReferenceImplementation.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FloryDev.SecureSettings.ReferenceImplementation
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public AppSettings Settings;
        public IDbContextFactory<ApplicationDbContext> applicationContextFactory { get; }
        public MicrosoftGraphSettings GraphSettings { get; }

        public Worker(ILogger<Worker> logger, SecureSettingsManager secureSettingsManager, ISecuredOptions<AppSettings> settings, ISecuredOptions<MicrosoftGraphSettings> graphSettings, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _logger = logger;
            applicationContextFactory = contextFactory;
            Settings = settings.Value;
            GraphSettings = graphSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var password = Settings.Password.GetDecryptedValue();
                var mailtoken = GraphSettings.ClientSecret.GetDecryptedValue();

                using (var dbContext = applicationContextFactory.CreateDbContext())
                {
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
