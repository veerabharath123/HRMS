using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface ILocalStorageProvider : IFileStorageProvider
    {
        bool FileExists(string path);
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
    }
}
