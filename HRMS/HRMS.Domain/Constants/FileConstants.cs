using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Constants
{
    public static class FileConstants
    {
        public const string LOCAL_STORAGE_CONFIG = "{\"providerType\":\"Local\"}";
        public const string CONTENT_EMPTY_MSG = "File content is empty.";
        public const string DOWLOAD_SUCCESS_MSG = "File downloaded successfully.";
        public const string DOWLOAD_FAIL_MSG = "Failed to download File.";
        public const string NO_STORAGE_CONFIG_MSG = "File storage location is not configured.";
        public const string UPLOAD_FAILED_MSG = "Failed to upload File.";
        public const string PROCESSING_FAILED_MSG = "Unable to process the file.";
        public const string PROCESSING_SUCCESS_MSG = "File processed successfully.";
        public const string MAINTENANCE_PROCESS_SUCCESS_MSG = "File maintenance process completed.";


        public enum FileStorageProvider
        {
            Local,
            Ftp,
            S3
        }
    }
}
