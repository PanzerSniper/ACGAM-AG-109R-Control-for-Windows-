using System;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace Ac109RDriverWin.Settings
{
    /// <summary>
    /// Reads and writes the application configuration JSON file.
    /// </summary>
    internal static class ConfigurationStore
    {
        /// <summary>
        /// Gets the root directory used for all user-specific application files.
        /// </summary>
        public static string AppDataDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AC109RDriverWin");
            }
        }

        /// <summary>
        /// Gets the full path of the JSON configuration file.
        /// </summary>
        public static string ConfigurationPath
        {
            get { return Path.Combine(AppDataDirectory, "settings.json"); }
        }

        /// <summary>
        /// Loads configuration from disk or returns defaults when the file does not exist.
        /// </summary>
        public static AppConfiguration Load()
        {
            try
            {
                if (!File.Exists(ConfigurationPath))
                {
                    return new AppConfiguration();
                }

                string json = File.ReadAllText(ConfigurationPath, new UTF8Encoding(false, true));
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                AppConfiguration configuration = serializer.Deserialize<AppConfiguration>(json);
                return Normalize(configuration);
            }
            catch
            {
                return new AppConfiguration();
            }
        }

        /// <summary>
        /// Saves configuration to disk using a UTF-8 JSON file.
        /// </summary>
        public static void Save(AppConfiguration configuration)
        {
            Directory.CreateDirectory(AppDataDirectory);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(Normalize(configuration));
            File.WriteAllText(ConfigurationPath, json, new UTF8Encoding(false));
        }

        /// <summary>
        /// Ensures nullable properties from older configuration files have valid values.
        /// </summary>
        private static AppConfiguration Normalize(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                return new AppConfiguration();
            }

            if (configuration.MacroBindings == null)
            {
                configuration.MacroBindings = new System.Collections.Generic.List<Macros.MacroBinding>();
            }

            configuration.LanguageCode = Localization.NormalizeLanguage(configuration.LanguageCode);

            return configuration;
        }
    }
}
