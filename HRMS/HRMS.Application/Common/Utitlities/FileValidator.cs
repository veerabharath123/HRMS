using HRMS.SharedKernel.Models.Common.Enum;
using HRMS.SharedKernel.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Utitlities
{
    public static class FileValidator
    {
        private static readonly Dictionary<FileType, string[]> FileSignatures = new()
        {
            { FileType.PDF, new[] { "%PDF" } },
            { FileType.PNG, new[] { "89-50-4E-47" } }, // PNG magic number
            { FileType.JPG, new[] { "FF-D8-FF" } }     // JPEG magic number
        };

        public static bool Validate(FileRequestDto file, params FileType[] allowedTypes)
        {
            if (file.FileContent is null || file.FileContent.Length == 0)
                return false;

            foreach (var type in allowedTypes)
            {
                if (IsOfType(file.FileContent, type))
                    return true;
            }

            return false;
        }

        private static bool IsOfType(byte[] fileBytes, FileType type)
        {
            if (!FileSignatures.TryGetValue(type, out var signatures))
                return false;

            foreach (var signature in signatures)
            {
                var sigBytes = ConvertSignature(signature);
                if (StartsWith(fileBytes, sigBytes))
                    return true;
            }

            return false;
        }

        private static byte[] ConvertSignature(string hexSignature)
        {
            var parts = hexSignature.Split('-');
            var bytes = new byte[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                bytes[i] = Convert.ToByte(parts[i], 16);
            return bytes;
        }

        private static bool StartsWith(byte[] fileBytes, byte[] signature)
        {
            if (fileBytes.Length < signature.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (fileBytes[i] != signature[i])
                    return false;
            }

            return true;
        }
    }
}
