using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Common
{
    public abstract class AuditableWithBaseEntity<T> : BaseEntity<T>, IAuditableEntity
    {
        public DateTime CreatedDate { get; set; }
        public TimeSpan CreatedTime { get; set; }
        public int CreatedUser { get; set; }
        public TimeSpan UpdatedTime { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
