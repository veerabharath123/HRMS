using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HashedIdAttribute : Attribute
    {
    }
}
