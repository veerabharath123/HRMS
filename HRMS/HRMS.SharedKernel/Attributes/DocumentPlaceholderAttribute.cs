using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Attributes
{
    public class DocumentPlaceholderAttribute : Attribute
    {
        public string? Key { get; }
        public DocumentPlaceholderAttribute(string key)
        {
            Key = key;
        }
        public DocumentPlaceholderAttribute()
        {
            
        }
    }
}
