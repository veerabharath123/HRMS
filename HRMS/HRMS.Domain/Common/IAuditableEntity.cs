using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Common
{
    public interface IAuditableEntity
    {
        public DateTime CreatedDate { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedUser { get; set; } 
        public bool IsDeleted { get; set; }
    }
}