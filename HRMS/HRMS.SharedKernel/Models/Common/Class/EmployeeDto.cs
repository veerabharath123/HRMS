using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class EmployeeDto
    {
        public string? Bio { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;        
        public string? MiddleName { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public int? GenderId { get; set; }
        [Required]
        public int MaritalStatusId { get; set; }
        public Guid? PhotoPictureId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? RelievingDate { get; set; }
        public DateTime? ResignationDate { get; set; }
        public int? ReportingManagerId { get; set; }
    }
    public class EmployeeDetailsDto: EmployeeDto
    {
        public int EmployeeId { get; set; } 
        public string MartialStatus { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string ReportingManager { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public bool UserExists { get; set; } 
        public string Email { get; set; } = string.Empty;
        public Guid GuidId { get; set; } 
        public bool HasPicture { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
