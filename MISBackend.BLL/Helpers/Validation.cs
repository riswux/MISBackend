using System.Text.RegularExpressions;

namespace MISBackend.BLL.Helpers
{
    public class Validation
    {
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new(pattern);
            return regex.IsMatch(email);
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^(?:\+?62|0)(?:\d{9,15}|\(\d{1,4}\)\d{6,12}|\d{7,14})$";
            Regex regex = new(pattern);
            return regex.IsMatch(phoneNumber);
        }

        private static string AppVersions = "Apps_1.0,Apps_1.1,Apps_1.2";
        public static bool IsValidApps(string AppVersion)
        {
            var apps = AppVersions.Split(',');
            return apps.FirstOrDefault(o => o.Trim().Equals(AppVersion.Trim())) != null;
        }
    }
}
