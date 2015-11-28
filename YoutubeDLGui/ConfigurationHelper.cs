// //
// // ConfigurationHelper.cs
// //
// // Author:
// //       Brian Allred <brian.d.allred@gmail.com>
// //
// // Copyright (c) 2015 Brian Allred
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in
// // all copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

namespace YoutubeDLGui
{
    #region Using

    using System.Configuration;
    using System.Reflection;

    using log4net;

    #endregion

    /// <summary>
    /// Helper for configuration
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Setting constant.
        /// </summary>
        public const string EnableEmbeddedBinary = "EnableEmbeddedBinary";

        /// <summary>
        /// Setting constant.
        /// </summary>
        public const string EnableVerboseOutput = "EnableVerboseOutput";

        /// <summary>
        /// Setting constant.
        /// </summary>
        public const string DestinationFolder = "DestinationFolder";

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Add or update app settings.
        /// </summary>
        /// <param name="key">Setting to add or update.</param>
        /// <param name="value">Value for setting.</param>
        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Error($"Error writing app settings: {ex}");
            }
        }

        /// <summary>
        /// Read setting.
        /// </summary>
        /// <param name="key">Setting to read</param>
        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? "Not Found";
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Error($"Error reading app settings: {ex}");
                return string.Empty;
            }
        }
    }
}