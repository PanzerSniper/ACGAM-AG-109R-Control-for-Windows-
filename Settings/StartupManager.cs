using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace Ac109RDriverWin.Settings
{
    /// <summary>
    /// Manages the current user's Windows startup registration.
    /// </summary>
    internal static class StartupManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "AG109RControl";

        /// <summary>
        /// Enables or disables launch at sign-in for the current Windows user.
        /// </summary>
        public static void SetEnabled(bool enabled, bool minimized)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (key == null)
                {
                    throw new InvalidOperationException("Unable to open the current user's Run registry key.");
                }

                if (enabled)
                {
                    string command = "\"" + GetExecutablePath() + "\"";
                    if (minimized)
                    {
                        command += " --minimized";
                    }

                    key.SetValue(ValueName, command, RegistryValueKind.String);
                }
                else
                {
                    key.DeleteValue(ValueName, false);
                }
            }
        }

        /// <summary>
        /// Returns true when the application is already registered for startup.
        /// </summary>
        public static bool IsEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
            {
                return key != null && key.GetValue(ValueName) != null;
            }
        }

        /// <summary>
        /// Resolves the executable path that Windows should launch at sign-in.
        /// </summary>
        private static string GetExecutablePath()
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            return Path.GetFullPath(path);
        }
    }
}
