using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Enum
{
    public enum FilterComparator
    {
        Equals,
        NotEquals,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }

    public enum FilterOperator
    {
        And,
        Or
    }

    public enum FilterPropertyType
    {
        String,
        Number,
        Date,
        Time,
        Bool
    }
}
