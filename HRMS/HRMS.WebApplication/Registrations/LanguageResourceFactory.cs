using HRMS.WebApplication.Class;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace HRMS.WebApplication.Registrations
{
    public class LanguageResourceFactory : IStringLocalizerFactory
    {
        private readonly ApiRequest _apiRequest;
        private readonly IMemoryCache _cache;

        public LanguageResourceFactory(ApiRequest apiRequest, IMemoryCache cache)
        {
            _apiRequest = apiRequest;
            _cache = cache;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return new LanguageResources(_apiRequest, culture, _cache);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return new LanguageResources(_apiRequest, culture, _cache);
        }
    }
}
