using HRMS.Application.Common.Interface;
using HRMS.Infrastructure.Storage.Providers;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
            var jsonConfig = JsonConvert.DeserializeObject<dynamic?>(locationConfig.ConfigJson);
            var providerType = jsonConfig?.ProviderType?.ToString();

            if(string.IsNullOrWhiteSpace(providerType) || jsonConfig is null)
            {
                throw new ArgumentException("ProviderType or JsonConfig is missing in the configuration.");
            }

            return providerType switch
            {
                "FtpStorageProvider" => new FtpStorageProvider(jsonConfig!.baseUrl,jsonConfig!.username, GetConfigValue(locationConfig.ConfigName, jsonConfig.passowrd), jsonConfig.useSsl),
                //"LocalFileStorageProvider" => new LocalFileStorageProvider(jsonConfig),
                //"AzureBlobStorageProvider" => new AzureBlobStorageProvider(jsonConfig),
                //"AmazonS3FileStorageProvider" => new AmazonS3FileStorageProvider(jsonConfig),
                _ => throw new NotSupportedException($"The provider type '{providerType}' is not supported.")
            };
        }
        private string GetConfigValue(string configName, string configSuffix, string defaultValue = "")
        {
            return _configuration[$"{configName}_{configSuffix}"] ?? defaultValue;
        }
    }
}
