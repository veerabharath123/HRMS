using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class UserListResponseDto: UserDetailsResponseDto
    {
        public int? EmployeeId { get; set; }
        public string EmpFullName { get; set; } = string.Empty;
    }
    public class UserDetailsResponseDto: UpdateUserResponseDto
    {
        
    }   
    public class UpdateUserResponseDto
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class LinkEmployeeRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public int EmployeeId { get; set; }

    }
}
