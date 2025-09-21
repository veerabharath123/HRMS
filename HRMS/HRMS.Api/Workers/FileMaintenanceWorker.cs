
using HRMS.Application.Services.File;

namespace HRMS.Api.Workers
{
    public class FileMaintenanceWorker : BackgroundService
    {
        private readonly ILogger<FileMaintenanceWorker> _logger;
        private readonly IFileServices _fileServices;
        public FileMaintenanceWorker(ILogger<FileMaintenanceWorker> logger, IFileServices fileServices)
        {
            _logger = logger;
            _fileServices = fileServices;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("File Maintenance Worker running at: {time}", DateTimeOffset.Now);
            // Implement file maintenance tasks here, e.g., deleting old files, archiving, etc.
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Example delay
        }
    }
}
