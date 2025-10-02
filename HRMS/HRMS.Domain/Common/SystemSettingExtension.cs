using HRMS.Domain.Entites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Common
{
    public static class SystemSettingExtension
    {
        public static T? GetSystemSetting<T>(this IEnumerable<SystemSettings> systemSettings, string settingName, T? defaultValue = default)
        {
            var setting = systemSettings.FirstOrDefault(s => s.SettingName  == settingName);

            if (setting == null || string.IsNullOrWhiteSpace(setting.SettingValue))
                return defaultValue;

            return JsonConvert.DeserializeObject<T>(setting.SettingValue) ?? defaultValue;
        }

        public static T2? GetSystemSetting<T1,T2>(this IEnumerable<SystemSettings> systemSettings, string settingName, Func<T1?, T2> transformSetting, T1? defaultValue = default)
        {
            var setting = systemSettings.GetSystemSetting(settingName, defaultValue);
            return transformSetting.Invoke(setting);
        }
    }
}
