using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Persistence.Configuration
{
    public class HashedIdConverter<T> : ValueConverter<T, string> where T : struct
    {
        public HashedIdConverter()
            : base(
                v => Convert.ToInt64(v).ToString(), //HashIdService.Encode(Convert.ToInt64(v)),  
                v => (T)Convert.ChangeType(v, typeof(T))//(T)Convert.ChangeType(HashIdService.Decode(v), typeof(T))
            )
        { }
    }
}
