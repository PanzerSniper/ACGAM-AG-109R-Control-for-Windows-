using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Ac109RDriverWin.Macros
{
    /// <summary>
    /// Describes one global hotkey and the action executed when it is pressed.
    /// </summary>
    internal sealed class MacroBinding
    {
        /// <summary>
        /// Gets or sets the virtual-key code used by RegisterHotKey.
        /// </summary>
        public int KeyCode { get; set; }

        /// <summary>
        /// Gets or sets whether the Ctrl modifier is required.
        /// </summary>
        public bool Control { get; set; }

        /// <summary>
        /// Gets or sets whether the Alt modifier is required.
        /// </summary>
        public bool Alt { get; set; }

        /// <summary>
        /// Gets or sets whether the Shift modifier is required.
        /// </summary>
        public bool Shift { get; set; }

        /// <summary>
        /// Gets or sets whether the Windows modifier is required.
        /// </summary>
        public bool Windows { get; set; }

        /// <summary>
        /// Gets or sets the action executed for this binding.
        /// </summary>
        public MacroAction Action { get; set; }

        /// <summary>
        /// Gets a user-facing hotkey label.
        /// </summary>
        [ScriptIgnore]
        public string ShortcutText
        {
            get
            {
                string shortcut = string.Empty;

                if (Control)
                {
                    shortcut += "Ctrl+";
                }

                if (Alt)
                {
                    shortcut += "Alt+";
                }

                if (Shift)
                {
                    shortcut += "Shift+";
                }

                if (Windows)
                {
                    shortcut += "Win+";
                }

                shortcut += ((Keys)KeyCode).ToString();
                return shortcut;
            }
        }

        /// <summary>
        /// Gets a user-facing action label.
        /// </summary>
        [ScriptIgnore]
        public string ActionText
        {
            get { return MacroActionLabels.GetLabel(Action); }
        }

        /// <summary>
        /// Returns a stable key used to detect duplicate shortcuts.
        /// </summary>
        public string GetShortcutKey()
        {
            return Control + "|" + Alt + "|" + Shift + "|" + Windows + "|" + KeyCode;
        }
    }

    /// <summary>
    /// Converts macro action values to readable labels.
    /// </summary>
    internal static class MacroActionLabels
    {
        /// <summary>
        /// Gets the display label for a macro action.
        /// </summary>
        public static string GetLabel(MacroAction action)
        {
            switch (action)
            {
                case MacroAction.VolumeUp:
                    return Localization.MacroActionLabel(action);
                case MacroAction.VolumeDown:
                    return Localization.MacroActionLabel(action);
                case MacroAction.VolumeMute:
                    return Localization.MacroActionLabel(action);
                case MacroAction.MediaPlayPause:
                    return Localization.MacroActionLabel(action);
                case MacroAction.MediaNextTrack:
                    return Localization.MacroActionLabel(action);
                case MacroAction.MediaPreviousTrack:
                    return Localization.MacroActionLabel(action);
                default:
                    return action.ToString();
            }
        }
    }
}
