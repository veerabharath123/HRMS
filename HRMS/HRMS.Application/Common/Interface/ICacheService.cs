using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface ICacheService
    {
        T GetOrCreate<T>(
            object key,
            Func<T> factory,
            TimeSpan? absoluteExpiration = null);
        Task<T> GetOrCreateAsync<T>(
                    object key,
                    Func<Task<T>> factory,
                    TimeSpan? absoluteExpiration = null);

    }
}
