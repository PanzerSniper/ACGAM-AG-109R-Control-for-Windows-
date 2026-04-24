using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ac109RDriverWin.Macros
{
    /// <summary>
    /// Registers global hotkeys and executes software media actions.
    /// </summary>
    internal sealed class MacroManager : IDisposable
    {
        private const int WmHotKey = 0x0312;
        private const uint ModAlt = 0x0001;
        private const uint ModControl = 0x0002;
        private const uint ModShift = 0x0004;
        private const uint ModWin = 0x0008;
        private const uint ModNoRepeat = 0x4000;
        private const byte KeyEventKeyUp = 0x02;
        private const byte VkVolumeMute = 0xAD;
        private const byte VkVolumeDown = 0xAE;
        private const byte VkVolumeUp = 0xAF;
        private const byte VkMediaNext = 0xB0;
        private const byte VkMediaPrevious = 0xB1;
        private const byte VkMediaPlayPause = 0xB3;

        private readonly IntPtr windowHandle;
        private readonly Dictionary<int, MacroBinding> registeredBindings = new Dictionary<int, MacroBinding>();
        private int nextHotKeyId = 100;

        /// <summary>
        /// Creates a macro manager for the provided WinForms window handle.
        /// </summary>
        public MacroManager(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;
        }

        /// <summary>
        /// Registers all configured macro bindings.
        /// </summary>
        public void RegisterAll(IEnumerable<MacroBinding> bindings)
        {
            UnregisterAll();

            foreach (MacroBinding binding in bindings)
            {
                Register(binding);
            }
        }

        /// <summary>
        /// Handles a WM_HOTKEY message if it belongs to a registered macro.
        /// </summary>
        public bool ProcessWindowMessage(ref Message message)
        {
            if (message.Msg != WmHotKey)
            {
                return false;
            }

            int id = message.WParam.ToInt32();
            MacroBinding binding;
            if (!registeredBindings.TryGetValue(id, out binding))
            {
                return false;
            }

            Execute(binding.Action);
            return true;
        }

        /// <summary>
        /// Unregisters all hotkeys held by this manager.
        /// </summary>
        public void UnregisterAll()
        {
            foreach (int id in new List<int>(registeredBindings.Keys))
            {
                UnregisterHotKey(windowHandle, id);
            }

            registeredBindings.Clear();
        }

        /// <summary>
        /// Releases all global hotkey registrations.
        /// </summary>
        public void Dispose()
        {
            UnregisterAll();
        }

        /// <summary>
        /// Registers one global hotkey.
        /// </summary>
        private void Register(MacroBinding binding)
        {
            int id = nextHotKeyId++;
            uint modifiers = ModNoRepeat | ToModifierFlags(binding);

            if (!RegisterHotKey(windowHandle, id, modifiers, (uint)binding.KeyCode))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), Localization.Format("HotkeyRegisterFailed", binding.ShortcutText));
            }

            registeredBindings.Add(id, binding);
        }

        /// <summary>
        /// Converts binding modifier booleans to Win32 RegisterHotKey flags.
        /// </summary>
        private static uint ToModifierFlags(MacroBinding binding)
        {
            uint modifiers = 0;

            if (binding.Alt)
            {
                modifiers |= ModAlt;
            }

            if (binding.Control)
            {
                modifiers |= ModControl;
            }

            if (binding.Shift)
            {
                modifiers |= ModShift;
            }

            if (binding.Windows)
            {
                modifiers |= ModWin;
            }

            return modifiers;
        }

        /// <summary>
        /// Executes a software macro action by sending the corresponding media virtual key.
        /// </summary>
        private static void Execute(MacroAction action)
        {
            switch (action)
            {
                case MacroAction.VolumeUp:
                    TapVirtualKey(VkVolumeUp);
                    break;
                case MacroAction.VolumeDown:
                    TapVirtualKey(VkVolumeDown);
                    break;
                case MacroAction.VolumeMute:
                    TapVirtualKey(VkVolumeMute);
                    break;
                case MacroAction.MediaPlayPause:
                    TapVirtualKey(VkMediaPlayPause);
                    break;
                case MacroAction.MediaNextTrack:
                    TapVirtualKey(VkMediaNext);
                    break;
                case MacroAction.MediaPreviousTrack:
                    TapVirtualKey(VkMediaPrevious);
                    break;
            }
        }

        /// <summary>
        /// Sends a key press followed by a key release for one virtual key.
        /// </summary>
        private static void TapVirtualKey(byte virtualKey)
        {
            keybd_event(virtualKey, 0, 0, UIntPtr.Zero);
            keybd_event(virtualKey, 0, KeyEventKeyUp, UIntPtr.Zero);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}
