using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class EmployeeDto
    {
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int? GenderId { get; set; }
        public int MaritalStatusId { get; set; }
        public Guid? PhotoPictureId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? RelievingDate { get; set; }
        public DateTime? ResignationDate { get; set; }
        public int? ReportingManagerId { get; set; }
    }
}
