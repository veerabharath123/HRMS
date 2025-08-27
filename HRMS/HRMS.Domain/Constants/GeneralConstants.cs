using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Constants
{
    public class GeneralConstants
    {
        public const string WEEKLY_EXPENSE_NOTE = "Your total expenses that you spent this week is {0}";
        public const string USER_ALREADY_EXISTS_MSG = "User with this name {0} already exists, please choose different name";
        public const string USER_PERMISSION_KEY = "UserPermissions";
        public const string QR_NO_DATA_MSG = "QR data cannot be null or empty.";
        public enum DependencyInjectionTypes
        {
            Transient,
            Scoped,
            Singleton
        }
    }
}
