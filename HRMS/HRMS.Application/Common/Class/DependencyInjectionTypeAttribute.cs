using HRMS.Domain.Constants;

namespace HRMS.Application.Common.Class
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyInjectionTypeAttribute : Attribute
    {
        private readonly GeneralConstants.DependencyInjectionTypes _dependencyTypeName;
        public DependencyInjectionTypeAttribute(GeneralConstants.DependencyInjectionTypes dependencyTypeName)
        {
            _dependencyTypeName = dependencyTypeName;
        }

        public virtual GeneralConstants.DependencyInjectionTypes DependencyTypeName { get { return _dependencyTypeName; } }
    }
}
