using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;

namespace FloryDev.SecureSettings.ReferenceImplementation
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public AppSettings Settings;

        public Worker(ILogger<Worker> logger, SecureSettingsManager secureSettingsManager, ISecuredOptions<AppSettings> settings)
        {
            _logger = logger;
            Settings = settings.Value;
            settings.Update(opt => {
                opt = Settings;
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var password = Settings.Password.GetDecryptedValue();
               
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
