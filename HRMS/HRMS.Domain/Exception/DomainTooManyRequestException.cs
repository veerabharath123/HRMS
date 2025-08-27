using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Exception
{
    public class DomainTooManyRequestException : DomainException
    {
        public DomainTooManyRequestException(string message) : base(message)
        {

        }
    }
}
