using HRMS.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HRMS.WebApplication.Class.BreadCrumbs
{
    public class BreadcrumbManager
    {
        private readonly IBreadcrumbCache _cache;

        public BreadcrumbManager(IBreadcrumbCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<BreadCrumbModel> UpdateTrail(BreadCrumbModel newCrumb, BreadCrumbModel defaultCrumb, bool isRootModule = false)
        {

            var trail = isRootModule ? [defaultCrumb, newCrumb] : GetCachedTrails(newCrumb);

            if (trail.Count > 0) 
                trail.Last().IsActive = true;

            _cache.SaveTrail(trail);

            return trail;
        }

        private List<BreadCrumbModel> GetCachedTrails(BreadCrumbModel newCrumb)
        {
            var trail = _cache.GetTrail();

            var existingIndex = trail.FindIndex(b => b.Action.Equals(newCrumb.Action, StringComparison.OrdinalIgnoreCase) && b.Controller.Equals(newCrumb.Controller,StringComparison.OrdinalIgnoreCase));

            if(existingIndex >= 0)
                trail = [.. trail.Take(existingIndex)];

            trail.Add(newCrumb);

            foreach (var bc in trail)
                bc.IsActive = false;

            return trail;
        }
    }
}
