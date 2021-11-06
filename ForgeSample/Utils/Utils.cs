using System;

namespace ForgeSample.Utils
{
    /// <summary>
    /// Class Utils.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Reads appsettings from web.config
        /// </summary>
        public static string GetAppSetting(string settingKey)
        {
            return Environment.GetEnvironmentVariable(settingKey);
        }

    }
}
