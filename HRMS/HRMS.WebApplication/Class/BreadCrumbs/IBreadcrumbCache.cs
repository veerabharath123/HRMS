using HRMS.WebApplication.Models;

namespace HRMS.WebApplication.Class.BreadCrumbs
{
    public interface IBreadcrumbCache
    {
        List<BreadCrumbModel> GetTrail();
        void SaveTrail(IEnumerable<BreadCrumbModel> trail);
        void Clear();
    }
}
