using System;
using Ac109RDriverWin.Settings;
using System.Threading;
using System.Windows.Forms;

namespace Ac109RDriverWin
{
    /// <summary>
    /// Application entry point for the AG109R Windows controller.
    /// </summary>
    internal static class Program
    {
        private const string SingleInstanceMutexName = "AG109RControl-0326e704-a7f7-f46bb6d4f780";

        /// <summary>
        /// Starts the WinForms message loop when no other AG109R Control instance is already running.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            try
            {
                using (Mutex mutex = new Mutex(true, SingleInstanceMutexName, out createdNew))
                {
                    Localization.LanguageCode = ConfigurationStore.Load().LanguageCode;
                    if (!createdNew)
                    {
                        ShowAlreadyRunningMessage();
                        return;
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm(args));
                }
            }
            catch (UnauthorizedAccessException)
            {
                Localization.LanguageCode = ConfigurationStore.Load().LanguageCode;
                ShowAlreadyRunningMessage();
            }
        }

        /// <summary>
        /// Notifies the user that another application instance owns the single-instance mutex.
        /// </summary>
        private static void ShowAlreadyRunningMessage()
        {
            MessageBox.Show(
                Localization.Text("AlreadyRunning"),
                "AG109R Control",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
