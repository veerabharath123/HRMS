using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface ISystemNotificationServices
    {
        Task SendNotificationAsync(string userId, string title, string body);
    }
}
