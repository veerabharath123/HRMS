using HRMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Records
{
    public static class EmployeeRecords
    {
        public record EmployeeFullRecord(
            string Lastname, 
            string Firstname, 
            DateTime Birthdate, 
            int? GenderId, 
            int MaritalStatusId, 
            DateTime JoiningDate,
            int? DepartmentId,
            int? DesignationId,
            DateTime? RelievingDate,
            int? ReportingManagerId
            );
    }
}


