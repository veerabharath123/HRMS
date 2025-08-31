using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace HRMS.WebApplication.Class
{
    public class LanguageResources : IStringLocalizer
    {
        private readonly ApiRequest _apiRequest;
        private readonly string _culture;
        private readonly IMemoryCache _cache;

        public LanguageResources(ApiRequest apiRequest, string culture, IMemoryCache cache)
        {
            _apiRequest = apiRequest;
            _culture = culture;
            _cache = cache;
        }

        private async Task<Dictionary<string, string>> LoadTranslations()
        {
            return await _cache.GetOrCreateAsync($"localization-{_culture}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // cache duration
                try
                {
                    var response = await _apiRequest.PostAsync<Dictionary<string,string>>("/Files/GetLanguageJson");

                    if(!response.Success || !response.HasResult) return [];

                    return response.Result;
                }
                catch
                {
                    return [];
                }
            }) ?? [];
        }

        public LocalizedString this[string name]
        {
            get
            {
                var translations = LoadTranslations().GetAwaiter().GetResult();
                if (translations.TryGetValue(name, out var value))
                    return new LocalizedString(name, value, false);

                return new LocalizedString(name, name, true); // fallback to key
            }
        }

        public LocalizedString this[string name, params object[] arguments]
            => new LocalizedString(name, string.Format(this[name].Value, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var translations = LoadTranslations().GetAwaiter().GetResult();
            return translations.Select(kvp => new LocalizedString(kvp.Key, kvp.Value));
        }
    }
}
