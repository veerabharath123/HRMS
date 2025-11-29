using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Records.EmployeeRecords;

namespace HRMS.Domain.Entites
{
    public class Employee : AuditableWithBaseEntity<int>
    {
        public string Bio { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string MiddleName { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public DateTime BirthDate { get; private set; }
        public int? GenderId { get; private set; }
        public int? MaritalStatusId { get; private set; }
        public int? PhotoPictureId { get; private set; }
        public int? DepartmentId { get; private set; }
        public int? DesignationId { get; private set; }
        public DateTime JoiningDate { get; private set; }
        public DateTime? RelievingDate { get; private set; }
        public DateTime? ResignationDate { get; private set; }
        public int? ReportingManagerId { get; private set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        public void Add(EmployeeFullRecord employeeFullRecord)
        {
            AddPersonalDetails(employeeFullRecord);
            AddJobDetails(employeeFullRecord);
        }
        public void AddPersonalDetails(EmployeeFullRecord employeeFullRecord)
        {
            LastName = employeeFullRecord.Lastname;
            FirstName = employeeFullRecord.Firstname;
            MiddleName = string.Empty;
            BirthDate = employeeFullRecord.Birthdate;
            GenderId = employeeFullRecord.GenderId;
            MaritalStatusId = employeeFullRecord.MaritalStatusId;
            Bio = employeeFullRecord.Bio;
        }
        public void AddJobDetails(EmployeeFullRecord employeeFullRecord)
        {
            JoiningDate = employeeFullRecord.JoiningDate;
            DepartmentId = employeeFullRecord.DepartmentId;
            DesignationId = employeeFullRecord.DesignationId;
            RelievingDate = employeeFullRecord.RelievingDate;
            ReportingManagerId = employeeFullRecord.ReportingManagerId;
        }
        public void AddProfilePicture(int? pictureId)
        {
            PhotoPictureId = pictureId;
        }
    }
}