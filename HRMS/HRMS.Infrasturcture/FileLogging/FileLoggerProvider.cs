using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace HRMS.Infrasturcture.FileLogging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();
        private readonly FileLoggerConfigDto _fileLoggerConfig;

        public FileLoggerProvider(IOptions<FileLoggerConfigDto> fileLoggerConfig)
        {
            _fileLoggerConfig = fileLoggerConfig.Value ?? new();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name =>
                   new FileLogger(_fileLoggerConfig)
                   );
        }

        public void Dispose()
        {
            _loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
