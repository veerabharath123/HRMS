using System;

namespace HRMS.Application.Common.Utilities
{
    public static class UriValidator
    {
        public static bool IsValidUri(string uriString)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uriResult))
            {
                return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }

        public static bool TryCreateAbsoluteUri(string uriString, out Uri? uriResult)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out uriResult))
            {
                return Uri.IsWellFormedUriString(uriResult.OriginalString, UriKind.Absolute) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            return false;
        }
    }
}
