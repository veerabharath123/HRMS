
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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested) 
            {
                await DoWorkAsync(stoppingToken);
            }
        }
        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            await _fileServices.ProcessFileMaintenanceAsync(stoppingToken);
        }
    }
}
