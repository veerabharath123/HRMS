using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface IImageCompressor
    {
        byte[] Compress(byte[] inputBytes, long targetSizeInBytes, bool preserveTransparency = true);
    }
}
