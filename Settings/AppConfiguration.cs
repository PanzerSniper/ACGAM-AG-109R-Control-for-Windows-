using Ac109RDriverWin.Macros;
using System.Collections.Generic;

namespace Ac109RDriverWin.Settings
{
    /// <summary>
    /// Persistent user configuration stored under the user's local application data folder.
    /// </summary>
    internal sealed class AppConfiguration
    {
        /// <summary>
        /// Creates a configuration object with conservative defaults.
        /// </summary>
        public AppConfiguration()
        {
            StartWithWindows = false;
            StartMinimized = false;
            LanguageCode = "fr";
            MacroBindings = new List<MacroBinding>();
        }

        /// <summary>
        /// Gets or sets whether the application should register itself in the current user's Run key.
        /// </summary>
        public bool StartWithWindows { get; set; }

        /// <summary>
        /// Gets or sets whether the startup registry entry should launch the application minimized.
        /// Manual launches only minimize when the --minimized command-line argument is present.
        /// </summary>
        public bool StartMinimized { get; set; }

        /// <summary>
        /// Gets or sets the selected user interface language.
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the globally registered macro shortcuts.
        /// </summary>
        public List<MacroBinding> MacroBindings { get; set; }
    }
}
