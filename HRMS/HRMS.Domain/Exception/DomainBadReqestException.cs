using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Exception
{
    public class DomainBadReqestException : DomainException
    {
        public DomainBadReqestException(string message) : base(message)
        {

        }
    }
}
