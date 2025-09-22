using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class StoredFiles: AuditableWithBaseEntity<int>
    {
        public string FileName { get; private set; } = string.Empty;
        public string FileContentType{ get; private set; } = string.Empty;
        public string FileExtension { get; private set; } = string.Empty;
        public int FileLocationId { get; private set; } 
        public bool IsProcessed { get; private set; } 

        public void Add(string fileName, string fileContentType, string fileExtension, int fileLocationId)
        {
            FileName = fileName;
            FileContentType = fileContentType;
            FileExtension = fileExtension;
            FileLocationId = fileLocationId;
            IsProcessed = false;
        }
        public void MarkAsProcessed()
        {
            IsProcessed = true;
        }
        public void DeleteFile()
        {
            IsProcessed = false;
            IsDeleted = true;
        }
    }
}
