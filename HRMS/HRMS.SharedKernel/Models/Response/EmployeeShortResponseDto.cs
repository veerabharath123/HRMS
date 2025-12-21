using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class EmployeeShortResponseDto
    {
        public int Id { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Bio { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid GuidId { get; set; } 
        public bool HasPicture { get; set; } 
    }
    public class EmployeeDetaisResponseDto
    {
        public EmployeeDetailsDto EmployeeDetails { get; set; } = new();
    }
}
