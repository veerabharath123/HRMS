using HRMS.Application.Common.Interface;
using HRMS.Infrastructure.Storage.Providers;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static HRMS.Domain.Constants.FileConstants;

namespace HRMS.Infrastructure.Storage.Factory
{
    public class FileStorageFactory: IFileStorageFactory
    {
        private readonly IConfiguration _configuration;
        public FileStorageFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IFileStorageProvider CreateProvider(FileLocationConfigDto locationConfig)
        {
            string? providerType = locationConfig?.ProviderType?.ToString();            

            if (locationConfig?.ConfigJson is null || !Enum.TryParse(providerType, out FileStorageProvider provider))
                throw new ArgumentException("ProviderType or JsonConfig is missing in the configuration.");

            return provider switch
            {
                FileStorageProvider.Ftp => new FtpStorageProvider(ResolveJsonConfig<FtpConfigDto>(locationConfig.ConfigJson)),
                FileStorageProvider.Local => new LocalStorageProvider(locationConfig.ConfigJson),
                FileStorageProvider.S3 => new S3StorageProvider(ResolveJsonConfig<S3BucketConfigDto>(locationConfig.ConfigJson)),
                _ => throw new NotSupportedException($"The provider type '{providerType}' is not supported.")
            };
        }
        private string GetConfigValue(string configName, string configSuffix, string defaultValue = "")
        {
            return _configuration[$"{configName}_{configSuffix}"] ?? defaultValue;
        }
        private T ResolveJsonConfig<T>(string configJson)
        {
            var config = JsonConvert.DeserializeObject<T>(configJson);
            return config ?? throw new ArgumentException("JsonConfig is missing in the configuration.");
        }
    }
}




