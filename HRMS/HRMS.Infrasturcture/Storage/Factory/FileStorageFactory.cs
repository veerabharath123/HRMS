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
            var jsonConfig = JsonConvert.DeserializeObject<dynamic?>(locationConfig.ConfigJson);
            string? providerType = jsonConfig?.ProviderType?.ToString();            

            if (jsonConfig is null || !Enum.TryParse(providerType, out FileStorageProvider provider))
                throw new ArgumentException("ProviderType or JsonConfig is missing in the configuration.");

            return provider switch
            {
                FileStorageProvider.Ftp => new FtpStorageProvider(jsonConfig!.baseUrl,jsonConfig!.username, GetConfigValue(locationConfig.ConfigName, jsonConfig.passowrd), jsonConfig.useSsl),
                FileStorageProvider.Local => new LocalStorageProvider(jsonConfig),
                FileStorageProvider.S3 => new S3StorageProvider(jsonConfig!.bucket,jsonConfig.key, jsonConfig.secret, jsonConfig.serviceurl),
                _ => throw new NotSupportedException($"The provider type '{providerType}' is not supported.")
            };
        }
        private string GetConfigValue(string configName, string configSuffix, string defaultValue = "")
        {
            return _configuration[$"{configName}_{configSuffix}"] ?? defaultValue;
        }
    }
}




