using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HRMS.Infrastructure.FileLogging
{
    public class FileLogger : ILogger
    {
        private readonly string _folderPath;
        private readonly int _retentionDays;
        private readonly int _maxRetries;
        private readonly int _retryDelayMs;

        public FileLogger(FileLoggerConfigDto options)
        {
            _folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,options.FileBasePath);
            _maxRetries = options.MaxRetries;
            _retryDelayMs = options.RetryDelayMilliseconds;
            _retentionDays = options.RetentionDays;

            var directory = Path.GetDirectoryName(_folderPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            CleanupOldLogs();
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
                           TState state, Exception? exception,
                           Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            string logFile = Path.Combine(
                _folderPath,
                $"app-{DateTime.Now:yyyy-MM-dd}.log");

            //using var _ = File.Create(logFile);

            var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)}";

            if (exception != null)
                message += Environment.NewLine + exception;

            WriteWithRetry(logFile, message);
        }
        private void WriteWithRetry(string filePath, string text)
        {
            for (int attempt = 0; attempt < _maxRetries; attempt++)
            {
                try
                {
                    File.AppendAllText(filePath, text + Environment.NewLine);
                    return; // success
                }
                catch (IOException) when (attempt < _maxRetries - 1)
                {
                    Thread.Sleep(_retryDelayMs); // wait before retry
                }
            }
        }

        private void CleanupOldLogs()
        {
            try
            {
                var files = Directory.GetFiles(_folderPath, "app-*.log");
                foreach (var file in files)
                {
                    var creationDate = File.GetCreationTime(file);
                    if (creationDate < DateTime.Now.AddDays(-_retentionDays))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Fail silently to not crash the app
            }
        }
    }
}
