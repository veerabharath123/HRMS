using AutoMapper;
using HRMS.Domain.Entites;
using HRMS.Domain.Records;
using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Records.EmployeeRecords;

namespace HRMS.Application.Common.Class.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<EmployeeDto, EmployeeFullRecord>();
        }
    }
}
