using HRMS.WebApplication.Models;

namespace HRMS.WebApplication.Class.BreadCrumbs
{
    public class SessionBreadcrumbCache: IBreadcrumbCache
    {
        private const string Key = "BreadcrumbTrail";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionBreadcrumbCache(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<BreadCrumbModel> GetTrail()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return [];

            var json = session.GetString(Key);
            return string.IsNullOrEmpty(json)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<BreadCrumbModel>>(json) ?? [];
        }

        public void SaveTrail(IEnumerable<BreadCrumbModel> trail)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            var json = System.Text.Json.JsonSerializer.Serialize(trail);
            session.SetString(Key, json);
        }

        public void Clear()
        {
            _httpContextAccessor.HttpContext?.Session?.Remove(Key);
        }
    }
}
