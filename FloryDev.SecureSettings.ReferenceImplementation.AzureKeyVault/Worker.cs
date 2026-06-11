using FloryDev.SecureSettings.Interfaces;

namespace FloryDev.SecureSettings.ReferenceImplementation.AzureKeyVault
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISecuredOptions<AppSettings> _settings;

        public Worker(ILogger<Worker> logger, SecureSettingsManager secureSettingsManager, ISecuredOptions<AppSettings> settings)
        {
            _logger = logger;
            // Accessing .Value here triggers auto-securing for local providers (DPAPI).
            // With Key Vault, ISecureService is not registered so no write-back occurs.
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // GetUnsecuredValueAsync() is always async — works identically whether the
                // backing provider is DPAPI (completes immediately) or Key Vault (network call,
                // served from cache after first fetch).
                var apiKey = await _settings.Value.ApiKey.GetUnsecuredValueAsync();

                _logger.LogInformation("ApiKey resolved, length: {length}", apiKey.Length);

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
