using FloryDev.SecureSettings.Interfaces;
using Microsoft.Extensions.Options;

namespace FloryDev.SecureSettings.ReferenceImplementation
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public AppSettings Settings;

        public Worker(ILogger<Worker> logger, IEncrypter encrypter, IDecrypter decrypter, IOptions<AppSettings> settings)
        {
            _logger = logger;
            EncryptedConfigSetting.Encrypter = encrypter;
            EncryptedConfigSetting.Decrypter = decrypter;   
            Settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
