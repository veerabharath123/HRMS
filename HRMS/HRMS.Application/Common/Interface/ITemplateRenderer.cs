using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface ITemplateRenderer
    {
        Task<string> RenderTemplateAsync(string templateName, object data);
    }
}
