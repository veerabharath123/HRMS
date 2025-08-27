using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Records
{
    public class UserRecords
    {
        public record UserAddOrUpdateRec(
                                    string FirstName,
                                    string LastName,
                                    string Email,
                                    string UserName,
                                    bool IsActive,
                                    bool IsDeleted = false
                                    );
    }
}
