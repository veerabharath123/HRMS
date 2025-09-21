using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface IFileStorageFactory
    {
        IFileStorageProvider CreateProvider(FileLocationConfigDto locationConfig);
    }
}
