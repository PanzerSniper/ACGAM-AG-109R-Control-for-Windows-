using Ac109RDriverWin.Hardware;
using Ac109RDriverWin.Macros;
using Ac109RDriverWin.Profiles;
using Ac109RDriverWin.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ac109RDriverWin
{
    /// <summary>
    /// Main WinForms window for lighting profiles, software macros, and application settings.
    /// </summary>
    internal sealed class MainForm : Form
    {
        private readonly List<Button> actionButtons = new List<Button>();
        private readonly AppConfiguration configuration;
        private readonly bool launchMinimized;

        private const string ProductDisplayName = "AC109R Control";

        private bool allowApplicationExit;
        private MacroManager macroManager;
        private NotifyIcon notifyIcon;
        private NumericUpDown profileNumber;
        private ComboBox builtInProfileCombo;
        private ComboBox userProfileCombo;
        private ComboBox macroKeyCombo;
        private ComboBox macroActionCombo;
        private ComboBox languageCombo;
        private CheckBox macroCtrlCheckBox;
        private CheckBox macroAltCheckBox;
        private CheckBox macroShiftCheckBox;
        private CheckBox macroWinCheckBox;
        private CheckBox startWithWindowsCheckBox;
        private CheckBox startMinimizedCheckBox;
        private NumericUpDown redNumber;
        private NumericUpDown greenNumber;
        private NumericUpDown blueNumber;
        private TextBox jsonPathTextBox;
        private TextBox logTextBox;
        private DataGridView macroGrid;
        private KeyboardPreviewPanel builtInPreview;
        private KeyboardPreviewPanel userPreview;
        private Label builtInDescriptionLabel;
        private Label userProfileInfoLabel;
        private Panel colorPreview;
        private Color selectedColor = Color.FromArgb(255, 0, 255);

        /// <summary>
        /// Creates the main form and applies persisted user configuration.
        /// </summary>
        public MainForm(string[] args)
        {
            configuration = ConfigurationStore.Load();
            Localization.LanguageCode = configuration.LanguageCode;
            launchMinimized = HasArgument(args, "--minimized");

            Text = ProductDisplayName;
            ClientSize = new Size(1060, 680);
            MinimumSize = new Size(940, 620);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);
            Icon = LoadApplicationIcon();

            BuildLayout();
            InitializeTrayIcon();
            UpdateColorFields(selectedColor);
            LoadBuiltInProfiles();
            RefreshUserProfiles(null);
            LoadMacroEditor();
            LoadSettingsEditor();
            AppendLog(T("Ready"));
        }

        /// <summary>
        /// Registers macros after the native window handle has been created.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            macroManager = new MacroManager(Handle);
            TryRegisterMacros();
        }

        /// <summary>
        /// Hides the window to the notification area when requested at startup.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (launchMinimized)
            {
                BeginInvoke(new MethodInvoker(HideToTray));
            }
        }

        /// <summary>
        /// Hides the window when minimized, keeping macros alive.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }

        /// <summary>
        /// Forwards global hotkey messages to the macro manager.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (macroManager != null && macroManager.ProcessWindowMessage(ref m))
            {
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Minimizes the application to the notification area for normal window closes, and releases resources on explicit exit.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowApplicationExit && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideToTray();
                AppendLog(T("ClosedToTray"));
                return;
            }

            if (macroManager != null)
            {
                macroManager.Dispose();
            }

            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Builds the full tabbed user interface.
        /// </summary>
        private void BuildLayout()
        {
            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 2;
            root.Padding = new Padding(8);
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(root);

            root.Controls.Add(CreateHeaderPanel(), 0, 0);

            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Margin = new Padding(0);
            tabs.Controls.Add(CreateStaticProfilesTab());
            tabs.Controls.Add(CreateMacrosTab());
            tabs.Controls.Add(CreateSettingsTab());
            tabs.Controls.Add(CreateChangelogTab());
            tabs.Controls.Add(CreateLogTab());
            root.Controls.Add(tabs, 0, 1);
        }

        /// <summary>
        /// Creates the always-visible device and onboard profile controls.
        /// </summary>
        private Control CreateHeaderPanel()
        {
            TableLayoutPanel header = new TableLayoutPanel();
            header.Dock = DockStyle.Fill;
            header.ColumnCount = 2;
            header.RowCount = 1;
            header.Margin = Padding.Empty;
            header.Padding = new Padding(0, 6, 0, 6);
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.WrapContents = false;
            panel.Margin = Padding.Empty;
            panel.Padding = new Padding(0, 2, 0, 0);

            panel.Controls.Add(CreateInlineLabel(T("KeyboardProfile")));
            profileNumber = new NumericUpDown { Minimum = 1, Maximum = 3, Value = 1, Width = 56, Margin = new Padding(3, 4, 8, 0) };
            panel.Controls.Add(profileNumber);

            panel.Controls.Add(CreateButton(T("Detect"), DetectButtonClick));
            panel.Controls.Add(CreateButton(T("Ping"), PingButtonClick));
            panel.Controls.Add(CreateButton(T("Activate"), SetProfileButtonClick));
            panel.Controls.Add(CreateButton(T("TurnOff"), ClearProfileButtonClick));

            header.Controls.Add(panel, 0, 0);
            header.Controls.Add(CreateBrandPanel(), 1, 0);

            return header;
        }

        /// <summary>
        /// Creates the tab that manages built-in and user JSON profiles.
        /// </summary>
        private TabPage CreateStaticProfilesTab()
        {
            TabPage tab = new TabPage(T("StaticProfiles"));

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(8);
            panel.ColumnCount = 2;
            panel.RowCount = 2;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tab.Controls.Add(panel);

            Control quickActions = CreateFillAndJsonGroup();
            panel.Controls.Add(CreateBuiltInProfileGroup(), 0, 0);
            panel.Controls.Add(CreateUserProfileGroup(), 1, 0);
            panel.SetColumnSpan(quickActions, 2);
            panel.Controls.Add(quickActions, 0, 1);

            return tab;
        }

        /// <summary>
        /// Creates the read-only built-in profile section.
        /// </summary>
        private Control CreateBuiltInProfileGroup()
        {
            GroupBox group = CreateGroup(T("BuiltInPresets"));

            TableLayoutPanel panel = CreateStackPanel();
            builtInProfileCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300, DropDownWidth = 300 };
            builtInProfileCombo.SelectedIndexChanged += BuiltInProfileComboSelectedIndexChanged;

            builtInDescriptionLabel = CreateDescriptionLabel();
            builtInPreview = new KeyboardPreviewPanel { Dock = DockStyle.Fill };

            FlowLayoutPanel buttons = CreateButtonRow();
            buttons.Controls.Add(CreateButton(T("SendToKeyboard"), SendBuiltInButtonClick));
            buttons.Controls.Add(CreateButton(T("CreateProfile"), CreateProfileFromBuiltInButtonClick));

            panel.Controls.Add(builtInProfileCombo, 0, 0);
            panel.Controls.Add(builtInDescriptionLabel, 0, 1);
            panel.Controls.Add(builtInPreview, 0, 2);
            panel.Controls.Add(buttons, 0, 3);
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            group.Controls.Add(panel);

            return group;
        }

        /// <summary>
        /// Creates the user JSON profile section.
        /// </summary>
        private Control CreateUserProfileGroup()
        {
            GroupBox group = CreateGroup(T("PersonalProfiles"));

            TableLayoutPanel panel = CreateStackPanel();
            userProfileCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300, DropDownWidth = 300 };
            userProfileCombo.SelectedIndexChanged += UserProfileComboSelectedIndexChanged;

            userProfileInfoLabel = CreateDescriptionLabel();
            userPreview = new KeyboardPreviewPanel { Dock = DockStyle.Fill };

            FlowLayoutPanel buttons = CreateButtonRow();
            buttons.Controls.Add(CreateButton(T("SendToKeyboard"), SendUserProfileButtonClick));
            buttons.Controls.Add(CreateButton(T("ImportJson"), ImportUserProfileButtonClick));
            buttons.Controls.Add(CreateButton(T("Rename"), RenameUserProfileButtonClick));
            buttons.Controls.Add(CreateButton(T("Duplicate"), DuplicateUserProfileButtonClick));
            buttons.Controls.Add(CreateButton(T("Delete"), DeleteUserProfileButtonClick));
            buttons.Controls.Add(CreateButton(T("OpenFolder"), OpenUserProfileFolderButtonClick));
            buttons.Controls.Add(CreateButton(T("Refresh"), RefreshUserProfilesButtonClick));

            panel.Controls.Add(userProfileCombo, 0, 0);
            panel.Controls.Add(userProfileInfoLabel, 0, 1);
            panel.Controls.Add(userPreview, 0, 2);
            panel.Controls.Add(buttons, 0, 3);
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            group.Controls.Add(panel);

            return group;
        }

        /// <summary>
        /// Creates compact controls for single-color fills and direct JSON loading.
        /// </summary>
        private Control CreateFillAndJsonGroup()
        {
            GroupBox group = CreateGroup(T("QuickActions"));
            group.AutoSize = true;
            group.Dock = DockStyle.Top;

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Top;
            panel.AutoSize = true;
            panel.ColumnCount = 1;
            panel.RowCount = 2;

            FlowLayoutPanel colorRow = CreateButtonRow();
            colorPreview = new Panel { Width = 32, Height = 24, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(3, 5, 8, 3) };
            redNumber = CreateByteInput();
            greenNumber = CreateByteInput();
            blueNumber = CreateByteInput();
            redNumber.ValueChanged += ColorNumberChanged;
            greenNumber.ValueChanged += ColorNumberChanged;
            blueNumber.ValueChanged += ColorNumberChanged;

            colorRow.Controls.Add(colorPreview);
            colorRow.Controls.Add(CreateButton(T("PickColor"), PickColorButtonClick));
            colorRow.Controls.Add(CreateInlineLabel("R"));
            colorRow.Controls.Add(redNumber);
            colorRow.Controls.Add(CreateInlineLabel("G"));
            colorRow.Controls.Add(greenNumber);
            colorRow.Controls.Add(CreateInlineLabel("B"));
            colorRow.Controls.Add(blueNumber);
            colorRow.Controls.Add(CreateButton(T("FillProfile"), FillProfileButtonClick));
            colorRow.Controls.Add(CreateButton(T("SaveColorProfile"), SaveColorProfileButtonClick));

            TableLayoutPanel jsonRow = new TableLayoutPanel();
            jsonRow.Dock = DockStyle.Top;
            jsonRow.AutoSize = true;
            jsonRow.ColumnCount = 3;
            jsonRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            jsonRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            jsonRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            jsonPathTextBox = new TextBox { Dock = DockStyle.Fill };
            jsonRow.Controls.Add(jsonPathTextBox, 0, 0);
            jsonRow.Controls.Add(CreateButton(T("Browse"), BrowseButtonClick), 1, 0);
            jsonRow.Controls.Add(CreateButton(T("LoadJson"), LoadJsonButtonClick), 2, 0);

            panel.Controls.Add(colorRow, 0, 0);
            panel.Controls.Add(jsonRow, 0, 1);
            group.Controls.Add(panel);

            return group;
        }

        /// <summary>
        /// Creates the software macro configuration tab.
        /// </summary>
        private TabPage CreateMacrosTab()
        {
            TabPage tab = new TabPage(T("Macros"));

            TableLayoutPanel panel = CreateStackPanel();
            panel.Padding = new Padding(8);
            tab.Controls.Add(panel);

            macroGrid = new DataGridView();
            macroGrid.Dock = DockStyle.Fill;
            macroGrid.AllowUserToAddRows = false;
            macroGrid.AllowUserToDeleteRows = false;
            macroGrid.ReadOnly = true;
            macroGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            macroGrid.MultiSelect = false;
            macroGrid.AutoGenerateColumns = false;
            macroGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = T("Shortcut"), DataPropertyName = "ShortcutText", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            macroGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = T("Action"), DataPropertyName = "ActionText", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            FlowLayoutPanel editor = CreateButtonRow();
            macroCtrlCheckBox = new CheckBox { Text = "Ctrl", AutoSize = true };
            macroAltCheckBox = new CheckBox { Text = "Alt", AutoSize = true };
            macroShiftCheckBox = new CheckBox { Text = "Shift", AutoSize = true };
            macroWinCheckBox = new CheckBox { Text = "Win", AutoSize = true };
            macroKeyCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            macroActionCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };

            editor.Controls.Add(macroCtrlCheckBox);
            editor.Controls.Add(macroAltCheckBox);
            editor.Controls.Add(macroShiftCheckBox);
            editor.Controls.Add(macroWinCheckBox);
            editor.Controls.Add(macroKeyCombo);
            editor.Controls.Add(macroActionCombo);
            editor.Controls.Add(CreateButton(T("AddMacro"), AddMacroButtonClick));
            editor.Controls.Add(CreateButton(T("RemoveSelected"), RemoveMacroButtonClick));

            Label note = CreateDescriptionLabel();
            note.Text = T("MacroNote");

            panel.Controls.Add(macroGrid, 0, 0);
            panel.Controls.Add(editor, 0, 1);
            panel.Controls.Add(note, 0, 2);
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            return tab;
        }

        /// <summary>
        /// Creates the application settings tab.
        /// </summary>
        private TabPage CreateSettingsTab()
        {
            TabPage tab = new TabPage(T("Settings"));
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.FlowDirection = FlowDirection.TopDown;
            panel.WrapContents = false;
            panel.Padding = new Padding(12);
            tab.Controls.Add(panel);

            FlowLayoutPanel languageRow = new FlowLayoutPanel();
            languageRow.AutoSize = true;
            languageRow.WrapContents = false;
            languageRow.Controls.Add(CreateInlineLabel(T("Language")));
            languageCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
            languageCombo.DisplayMember = "Name";
            languageCombo.ValueMember = "Code";
            languageCombo.DataSource = Localization.GetLanguageOptions();
            languageRow.Controls.Add(languageCombo);

            startWithWindowsCheckBox = new CheckBox { Text = T("StartWithWindows"), AutoSize = true };
            startMinimizedCheckBox = new CheckBox { Text = T("StartMinimized"), AutoSize = true };

            Label dataLabel = CreateDescriptionLabel();
            dataLabel.AutoSize = true;
            dataLabel.Text = TF("ApplicationData", ConfigurationStore.AppDataDirectory);

            panel.Controls.Add(languageRow);
            panel.Controls.Add(startWithWindowsCheckBox);
            panel.Controls.Add(startMinimizedCheckBox);
            panel.Controls.Add(CreateButton(T("SaveSettings"), SaveSettingsButtonClick));
            panel.Controls.Add(dataLabel);

            return tab;
        }

        /// <summary>
        /// Creates the application log tab.
        /// </summary>
        private TabPage CreateLogTab()
        {
            TabPage tab = new TabPage(T("Log"));
            logTextBox = new TextBox();
            logTextBox.Dock = DockStyle.Fill;
            logTextBox.Multiline = true;
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            tab.Controls.Add(logTextBox);
            return tab;
        }

        /// <summary>
        /// Creates the release notes tab.
        /// </summary>
        private TabPage CreateChangelogTab()
        {
            TabPage tab = new TabPage(T("Changelog"));
            TextBox changelogTextBox = new TextBox();
            changelogTextBox.Dock = DockStyle.Fill;
            changelogTextBox.Multiline = true;
            changelogTextBox.ReadOnly = true;
            changelogTextBox.ScrollBars = ScrollBars.Vertical;
            changelogTextBox.Text = T("ChangelogText");
            tab.Controls.Add(changelogTextBox);
            return tab;
        }

        /// <summary>
        /// Creates a consistently styled group box.
        /// </summary>
        private static GroupBox CreateGroup(string title)
        {
            return new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(10) };
        }

        /// <summary>
        /// Creates a one-column table used as a vertical layout surface.
        /// </summary>
        private static TableLayoutPanel CreateStackPanel()
        {
            return new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        }

        /// <summary>
        /// Creates a horizontal flow panel for buttons and compact inputs.
        /// </summary>
        private static FlowLayoutPanel CreateButtonRow()
        {
            return new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
        }

        /// <summary>
        /// Creates a compact label aligned with input controls.
        /// </summary>
        private static Label CreateInlineLabel(string text)
        {
            return new Label { Text = text, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 7, 2, 0) };
        }

        /// <summary>
        /// Creates a muted descriptive label.
        /// </summary>
        private static Label CreateDescriptionLabel()
        {
            return new Label { AutoSize = true, Dock = DockStyle.Top, ForeColor = SystemColors.GrayText, Padding = new Padding(0, 6, 0, 6) };
        }

        /// <summary>
        /// Creates the top-right identity block with the application icon, name, version, and generated build date.
        /// </summary>
        private Control CreateBrandPanel()
        {
            TableLayoutPanel brand = new TableLayoutPanel();
            brand.AutoSize = true;
            brand.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            brand.ColumnCount = 2;
            brand.RowCount = 1;
            brand.Margin = Padding.Empty;
            brand.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            brand.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            PictureBox logo = new PictureBox();
            logo.Image = LoadBrandLogo();
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.Width = 40;
            logo.Height = 40;
            logo.Margin = new Padding(8, 0, 8, 0);

            TableLayoutPanel textPanel = new TableLayoutPanel();
            textPanel.AutoSize = true;
            textPanel.ColumnCount = 1;
            textPanel.RowCount = 2;

            Label nameLabel = new Label();
            nameLabel.Text = ProductDisplayName;
            nameLabel.AutoSize = true;
            nameLabel.Font = new Font(Font, FontStyle.Bold);
            nameLabel.TextAlign = ContentAlignment.MiddleRight;
            nameLabel.Dock = DockStyle.Right;

            Label versionLabel = new Label();
            versionLabel.Text = "v" + GetApplicationVersion() + " | " + T("BuildLabel") + " " + BuildInfo.BuildDateUtc;
            versionLabel.AutoSize = true;
            versionLabel.ForeColor = SystemColors.GrayText;
            versionLabel.TextAlign = ContentAlignment.MiddleRight;
            versionLabel.Dock = DockStyle.Right;

            textPanel.Controls.Add(nameLabel, 0, 0);
            textPanel.Controls.Add(versionLabel, 0, 1);

            brand.Controls.Add(logo, 0, 0);
            brand.Controls.Add(textPanel, 1, 0);
            return brand;
        }

        /// <summary>
        /// Creates a button and includes it in the shared busy-state list.
        /// </summary>
        private Button CreateButton(string text, EventHandler clickHandler)
        {
            Button button = new Button();
            button.Text = text;
            button.AutoSize = true;
            button.MinimumSize = new Size(88, 28);
            button.Margin = new Padding(3, 3, 8, 3);
            button.Click += clickHandler;
            actionButtons.Add(button);
            return button;
        }

        /// <summary>
        /// Creates an 8-bit numeric input for RGB channels.
        /// </summary>
        private static NumericUpDown CreateByteInput()
        {
            return new NumericUpDown { Minimum = 0, Maximum = 255, Width = 60 };
        }

        /// <summary>
        /// Creates the notification area icon and context menu.
        /// </summary>
        private void InitializeTrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Icon;
            notifyIcon.Text = ProductDisplayName;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += delegate { ShowFromTray(); };
            UpdateTrayIconMenu();
        }

        /// <summary>
        /// Refreshes the notification area menu in the active UI language.
        /// </summary>
        private void UpdateTrayIconMenu()
        {
            if (notifyIcon == null)
            {
                return;
            }

            ContextMenuStrip oldMenu = notifyIcon.ContextMenuStrip;
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add(T("Open"), null, delegate { ShowFromTray(); });
            menu.Items.Add(T("Exit"), null, delegate
            {
                allowApplicationExit = true;
                Close();
            });

            notifyIcon.ContextMenuStrip = menu;
            if (oldMenu != null)
            {
                oldMenu.Dispose();
            }
        }

        /// <summary>
        /// Loads the application icon embedded into the executable.
        /// </summary>
        private static Icon LoadApplicationIcon()
        {
            Icon icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (icon != null)
            {
                return icon;
            }

            return SystemIcons.Application;
        }

        /// <summary>
        /// Loads the high-resolution PNG logo embedded in the application resources.
        /// </summary>
        private Image LoadBrandLogo()
        {
            try
            {
                System.Resources.ResourceManager resources = new System.Resources.ResourceManager("Ac109RDriverWin.Properties.Resources", typeof(MainForm).Assembly);
                Bitmap icon = resources.GetObject("icon") as Bitmap;
                if (icon != null)
                {
                    return new Bitmap(icon);
                }
            }
            catch
            {
            }

            return Icon.ToBitmap();
        }

        /// <summary>
        /// Returns the semantic version from the application assembly.
        /// </summary>
        private string GetApplicationVersion()
        {
            Version version = GetType().Assembly.GetName().Version;
            return version.Major + "." + version.Minor + "." + version.Build;
        }

        /// <summary>
        /// Populates the built-in static profile combo box.
        /// </summary>
        private void LoadBuiltInProfiles()
        {
            List<BuiltInProfileListItem> items = new List<BuiltInProfileListItem>();
            foreach (BuiltInProfile profile in BuiltInProfiles.All)
            {
                items.Add(new BuiltInProfileListItem(profile));
            }

            builtInProfileCombo.DataSource = items;
            if (builtInProfileCombo.Items.Count > 0)
            {
                builtInProfileCombo.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Reloads user JSON profiles from the profile storage directory.
        /// </summary>
        private void RefreshUserProfiles(string selectedPath)
        {
            IList<string> files = UserProfileStore.GetProfileFiles();
            List<UserProfileListItem> items = new List<UserProfileListItem>();

            foreach (string file in files)
            {
                items.Add(new UserProfileListItem(file));
            }

            userProfileCombo.DataSource = null;
            userProfileCombo.DataSource = items;

            SelectUserProfile(selectedPath);

            if (items.Count == 0)
            {
                userProfileInfoLabel.Text = T("NoUserProfile");
                userPreview.Keys = null;
            }
            else
            {
                if (userProfileCombo.SelectedIndex < 0)
                {
                    userProfileCombo.SelectedIndex = 0;
                }
                else
                {
                    UserProfileComboSelectedIndexChanged(userProfileCombo, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Selects a user profile by path when possible.
        /// </summary>
        private void SelectUserProfile(string selectedPath)
        {
            if (string.IsNullOrEmpty(selectedPath))
            {
                return;
            }

            for (int i = 0; i < userProfileCombo.Items.Count; i++)
            {
                UserProfileListItem item = userProfileCombo.Items[i] as UserProfileListItem;
                if (item != null && string.Equals(item.Path, selectedPath, StringComparison.OrdinalIgnoreCase))
                {
                    userProfileCombo.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Populates macro editor dropdowns and the macro table.
        /// </summary>
        private void LoadMacroEditor()
        {
            macroKeyCombo.DataSource = BuildMacroKeyList();
            macroKeyCombo.DisplayMember = "Name";
            macroKeyCombo.ValueMember = "Key";

            macroActionCombo.DataSource = BuildMacroActionList();
            macroActionCombo.DisplayMember = "Name";
            macroActionCombo.ValueMember = "Action";

            RefreshMacroGrid();
        }

        /// <summary>
        /// Loads settings into the settings tab controls.
        /// </summary>
        private void LoadSettingsEditor()
        {
            SelectLanguage(configuration.LanguageCode);
            startWithWindowsCheckBox.Checked = StartupManager.IsEnabled();
            startMinimizedCheckBox.Checked = configuration.StartMinimized;
        }

        /// <summary>
        /// Selects a language option in the settings combo box.
        /// </summary>
        private void SelectLanguage(string languageCode)
        {
            if (languageCombo == null)
            {
                return;
            }

            string normalized = Localization.NormalizeLanguage(languageCode);
            for (int i = 0; i < languageCombo.Items.Count; i++)
            {
                LanguageOption option = languageCombo.Items[i] as LanguageOption;
                if (option != null && string.Equals(option.Code, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    languageCombo.SelectedIndex = i;
                    return;
                }
            }

            if (languageCombo.Items.Count > 0)
            {
                languageCombo.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gets the language selected in the settings combo box.
        /// </summary>
        private string GetSelectedLanguageCode()
        {
            LanguageOption option = languageCombo == null ? null : languageCombo.SelectedItem as LanguageOption;
            if (option == null)
            {
                return Localization.French;
            }

            return option.Code;
        }

        /// <summary>
        /// Rebuilds language-sensitive controls after the user changes the UI language.
        /// </summary>
        private void RebuildLayoutAfterLanguageChange()
        {
            string selectedUserProfile = null;
            UserProfileListItem selectedUserProfileItem = GetSelectedUserProfile();
            if (selectedUserProfileItem != null)
            {
                selectedUserProfile = selectedUserProfileItem.Path;
            }

            while (Controls.Count > 0)
            {
                Control control = Controls[0];
                Controls.RemoveAt(0);
                control.Dispose();
            }

            actionButtons.Clear();
            BuildLayout();
            UpdateTrayIconMenu();
            UpdateColorFields(selectedColor);
            LoadBuiltInProfiles();
            RefreshUserProfiles(selectedUserProfile);
            LoadMacroEditor();
            LoadSettingsEditor();
        }

        /// <summary>
        /// Rebinds the macro table to the current configuration.
        /// </summary>
        private void RefreshMacroGrid()
        {
            macroGrid.DataSource = null;
            macroGrid.DataSource = configuration.MacroBindings;
        }

        /// <summary>
        /// Registers configured macros and logs conflicts.
        /// </summary>
        private void TryRegisterMacros()
        {
            if (macroManager == null)
            {
                return;
            }

            try
            {
                macroManager.RegisterAll(configuration.MacroBindings);
                AppendLog(TF("MacroRegistered", configuration.MacroBindings.Count));
            }
            catch (Exception ex)
            {
                AppendLog(TF("MacroRegistrationFailed", ex.Message));
            }
        }

        /// <summary>
        /// Updates the built-in static preset preview.
        /// </summary>
        private void BuiltInProfileComboSelectedIndexChanged(object sender, EventArgs e)
        {
            BuiltInProfileListItem item = builtInProfileCombo.SelectedItem as BuiltInProfileListItem;
            if (item == null)
            {
                builtInDescriptionLabel.Text = string.Empty;
                builtInPreview.Keys = null;
                return;
            }

            builtInDescriptionLabel.Text = item.Description + " " + T("BuiltInReadOnly");
            builtInPreview.Keys = item.Profile.Keys;
        }

        /// <summary>
        /// Updates the user JSON profile preview.
        /// </summary>
        private void UserProfileComboSelectedIndexChanged(object sender, EventArgs e)
        {
            UserProfileListItem item = GetSelectedUserProfile();
            if (item == null)
            {
                userPreview.Keys = null;
                return;
            }

            try
            {
                ProfileData data = ProfileParser.LoadJson(item.Path);
                userPreview.Keys = data.Keys;
                userProfileInfoLabel.Text = TF("MappedKeys", data.AppliedKeys);
            }
            catch (Exception ex)
            {
                userPreview.Keys = null;
                userProfileInfoLabel.Text = TF("InvalidProfile", ex.Message);
            }
        }

        /// <summary>
        /// Detects the keyboard HID interface.
        /// </summary>
        private async void DetectButtonClick(object sender, EventArgs e)
        {
            await RunOperationAsync(delegate
            {
                IList<HidDeviceInfo> devices = Ac109KeyboardClient.FindDevices();
                if (devices.Count == 0)
                {
                    return T("NoDeviceFound");
                }

                return TF("DeviceFound", devices[0]);
            });
        }

        /// <summary>
        /// Sends a safe ping command to the keyboard.
        /// </summary>
        private async void PingButtonClick(object sender, EventArgs e)
        {
            await RunKeyboardOperationAsync(delegate(Ac109KeyboardClient client)
            {
                client.Ping();
                return T("PingOk");
            });
        }

        /// <summary>
        /// Activates the selected onboard profile.
        /// </summary>
        private async void SetProfileButtonClick(object sender, EventArgs e)
        {
            int profile = GetKeyboardProfile();
            await RunKeyboardOperationAsync(delegate(Ac109KeyboardClient client)
            {
                client.SetProfile(profile);
                return TF("ProfileActivated", profile);
            });
        }

        /// <summary>
        /// Clears the selected onboard profile.
        /// </summary>
        private async void ClearProfileButtonClick(object sender, EventArgs e)
        {
            int profile = GetKeyboardProfile();
            await RunKeyboardOperationAsync(delegate(Ac109KeyboardClient client)
            {
                client.ClearProfile(profile);
                return TF("ProfileTurnedOff", profile);
            });
        }

        /// <summary>
        /// Sends the selected built-in static profile to the keyboard.
        /// </summary>
        private async void SendBuiltInButtonClick(object sender, EventArgs e)
        {
            BuiltInProfile profile = GetSelectedBuiltInProfile();
            if (profile == null)
            {
                return;
            }

            int keyboardProfile = GetKeyboardProfile();
            string profileName = GetSelectedBuiltInProfileName();
            await RunKeyboardOperationAsync(delegate(Ac109KeyboardClient client)
            {
                client.SendProfile(keyboardProfile, profile.Keys);
                return TF("PresetSent", profileName, keyboardProfile);
            });
        }

        /// <summary>
        /// Creates a managed personal profile from the selected built-in preset.
        /// </summary>
        private void CreateProfileFromBuiltInButtonClick(object sender, EventArgs e)
        {
            BuiltInProfile profile = GetSelectedBuiltInProfile();
            if (profile == null)
            {
                return;
            }

            try
            {
                string path = UserProfileStore.SaveCopy(profile.Name, profile.Keys);
                RefreshUserProfiles(path);
                AppendLog(TF("PresetCopied", path));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Sends the selected user JSON profile to the keyboard.
        /// </summary>
        private async void SendUserProfileButtonClick(object sender, EventArgs e)
        {
            UserProfileListItem item = GetSelectedUserProfile();
            if (item == null)
            {
                AppendLog(T("NoUserProfileSelected"));
                return;
            }

            int keyboardProfile = GetKeyboardProfile();
            await RunOperationAsync(delegate
            {
                ProfileData data = ProfileParser.LoadJson(item.Path);
                using (Ac109KeyboardClient client = Ac109KeyboardClient.Open())
                {
                    client.SendProfile(keyboardProfile, data.Keys);
                }

                return TF("JsonProfileSent", item.Name, keyboardProfile);
            });
        }

        /// <summary>
        /// Imports an external JSON profile into the managed user profile directory.
        /// </summary>
        private void ImportUserProfileButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = T("JsonProfileFilter");
                dialog.Title = T("ImportJsonTitle");

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string path = UserProfileStore.Import(dialog.FileName);
                        RefreshUserProfiles(path);
                        AppendLog(TF("ProfileImported", path));
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Renames the selected managed JSON profile.
        /// </summary>
        private void RenameUserProfileButtonClick(object sender, EventArgs e)
        {
            UserProfileListItem item = GetSelectedUserProfile();
            if (item == null)
            {
                AppendLog(T("NoUserProfileSelected"));
                return;
            }

            string requestedName = PromptForProfileName(T("RenameProfileTitle"), item.Name);
            if (string.IsNullOrWhiteSpace(requestedName))
            {
                return;
            }

            try
            {
                string path = UserProfileStore.Rename(item.Path, requestedName);
                RefreshUserProfiles(path);
                AppendLog(TF("ProfileRenamed", path));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Creates a copy of the selected managed JSON profile.
        /// </summary>
        private void DuplicateUserProfileButtonClick(object sender, EventArgs e)
        {
            UserProfileListItem item = GetSelectedUserProfile();
            if (item == null)
            {
                AppendLog(T("NoUserProfileSelected"));
                return;
            }

            try
            {
                string path = UserProfileStore.Duplicate(item.Path);
                RefreshUserProfiles(path);
                AppendLog(TF("ProfileDuplicated", path));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Deletes the selected managed JSON profile after confirmation.
        /// </summary>
        private void DeleteUserProfileButtonClick(object sender, EventArgs e)
        {
            UserProfileListItem item = GetSelectedUserProfile();
            if (item == null)
            {
                AppendLog(T("NoUserProfileSelected"));
                return;
            }

            DialogResult result = MessageBox.Show(
                this,
                TF("DeleteProfileQuestion", item.Name),
                ProductDisplayName,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string deletedPath = item.Path;
                UserProfileStore.Delete(item.Path);
                RefreshUserProfiles(null);
                AppendLog(TF("ProfileDeleted", deletedPath));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Opens the managed profile folder in File Explorer.
        /// </summary>
        private void OpenUserProfileFolderButtonClick(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(UserProfileStore.DirectoryPath);
                Process.Start(new ProcessStartInfo
                {
                    FileName = UserProfileStore.DirectoryPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Refreshes the managed user profile list.
        /// </summary>
        private void RefreshUserProfilesButtonClick(object sender, EventArgs e)
        {
            RefreshUserProfiles(null);
            AppendLog(T("ProfileListRefreshed"));
        }

        /// <summary>
        /// Opens the color picker for quick full-profile fills.
        /// </summary>
        private void PickColorButtonClick(object sender, EventArgs e)
        {
            using (ColorDialog dialog = new ColorDialog())
            {
                dialog.Color = selectedColor;
                dialog.FullOpen = true;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    UpdateColorFields(dialog.Color);
                }
            }
        }

        /// <summary>
        /// Sends a single-color profile to the keyboard.
        /// </summary>
        private async void FillProfileButtonClick(object sender, EventArgs e)
        {
            int profile = GetKeyboardProfile();
            byte red = (byte)redNumber.Value;
            byte green = (byte)greenNumber.Value;
            byte blue = (byte)blueNumber.Value;

            await RunKeyboardOperationAsync(delegate(Ac109KeyboardClient client)
            {
                client.FillProfile(profile, red, green, blue);
                return TF("ProfileFilled", profile, red, green, blue);
            });
        }

        /// <summary>
        /// Saves the current RGB quick color as a managed personal profile.
        /// </summary>
        private void SaveColorProfileButtonClick(object sender, EventArgs e)
        {
            byte red = (byte)redNumber.Value;
            byte green = (byte)greenNumber.Value;
            byte blue = (byte)blueNumber.Value;
            string defaultName = TF("ColorProfileDefaultName", red, green, blue);
            string requestedName = PromptForProfileName(T("SaveColorProfileTitle"), defaultName);
            if (string.IsNullOrWhiteSpace(requestedName))
            {
                return;
            }

            try
            {
                KeyColor[] keys = ProfileParser.CreateFilled(red, green, blue);
                string path = UserProfileStore.SaveCopy(requestedName, keys);
                RefreshUserProfiles(path);
                AppendLog(TF("ColorProfileCreated", path));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
        /// <summary>
        /// Loads an external JSON profile directly to the keyboard.
        /// </summary>
        private async void LoadJsonButtonClick(object sender, EventArgs e)
        {
            int profile = GetKeyboardProfile();
            string path = jsonPathTextBox.Text.Trim();

            await RunOperationAsync(delegate
            {
                ProfileData data = ProfileParser.LoadJson(path);
                using (Ac109KeyboardClient client = Ac109KeyboardClient.Open())
                {
                    client.SendProfile(profile, data.Keys);
                }

                string message = TF("JsonLoaded", profile, data.AppliedKeys);
                if (data.UnknownKeys.Count > 0)
                {
                    message += TF("IgnoredKeys", string.Join(", ", data.UnknownKeys));
                }

                return message;
            });
        }

        /// <summary>
        /// Lets the user select an external JSON profile.
        /// </summary>
        private void BrowseButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = T("JsonProfileFilter");
                dialog.Title = T("ChooseProfileTitle");

                if (!string.IsNullOrWhiteSpace(jsonPathTextBox.Text))
                {
                    string directory = Path.GetDirectoryName(jsonPathTextBox.Text);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        dialog.InitialDirectory = directory;
                    }
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    jsonPathTextBox.Text = dialog.FileName;
                }
            }
        }

        /// <summary>
        /// Adds a new software macro binding.
        /// </summary>
        private void AddMacroButtonClick(object sender, EventArgs e)
        {
            MacroBinding binding = CreateBindingFromEditor();
            if (binding == null)
            {
                return;
            }

            foreach (MacroBinding existing in configuration.MacroBindings)
            {
                if (existing.GetShortcutKey() == binding.GetShortcutKey())
                {
                    MessageBox.Show(this, T("DuplicateHotkey"), T("MacroTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            configuration.MacroBindings.Add(binding);
            ConfigurationStore.Save(configuration);
            RefreshMacroGrid();
            TryRegisterMacros();
        }

        /// <summary>
        /// Removes the selected software macro binding.
        /// </summary>
        private void RemoveMacroButtonClick(object sender, EventArgs e)
        {
            if (macroGrid.CurrentRow == null)
            {
                return;
            }

            MacroBinding binding = macroGrid.CurrentRow.DataBoundItem as MacroBinding;
            if (binding == null)
            {
                return;
            }

            configuration.MacroBindings.Remove(binding);
            ConfigurationStore.Save(configuration);
            RefreshMacroGrid();
            TryRegisterMacros();
        }

        /// <summary>
        /// Saves startup and minimize settings.
        /// </summary>
        private void SaveSettingsButtonClick(object sender, EventArgs e)
        {
            try
            {
                configuration.StartWithWindows = startWithWindowsCheckBox.Checked;
                configuration.StartMinimized = startMinimizedCheckBox.Checked;
                string previousLanguage = Localization.LanguageCode;
                configuration.LanguageCode = GetSelectedLanguageCode();
                StartupManager.SetEnabled(configuration.StartWithWindows, configuration.StartMinimized);
                ConfigurationStore.Save(configuration);

                if (!string.Equals(previousLanguage, configuration.LanguageCode, StringComparison.OrdinalIgnoreCase))
                {
                    Localization.LanguageCode = configuration.LanguageCode;
                    RebuildLayoutAfterLanguageChange();
                }

                AppendLog(T("SettingsSaved"));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Updates the RGB preview when channel values change.
        /// </summary>
        private void ColorNumberChanged(object sender, EventArgs e)
        {
            selectedColor = Color.FromArgb((int)redNumber.Value, (int)greenNumber.Value, (int)blueNumber.Value);
            colorPreview.BackColor = selectedColor;
        }

        /// <summary>
        /// Updates RGB numeric controls from a Color value.
        /// </summary>
        private void UpdateColorFields(Color color)
        {
            selectedColor = color;
            redNumber.Value = color.R;
            greenNumber.Value = color.G;
            blueNumber.Value = color.B;
            colorPreview.BackColor = color;
        }

        /// <summary>
        /// Runs a keyboard operation on a background thread.
        /// </summary>
        private async Task RunKeyboardOperationAsync(Func<Ac109KeyboardClient, string> action)
        {
            await RunOperationAsync(delegate
            {
                using (Ac109KeyboardClient client = Ac109KeyboardClient.Open())
                {
                    return action(client);
                }
            });
        }

        /// <summary>
        /// Runs a blocking operation on a background thread and reports its result.
        /// </summary>
        private async Task RunOperationAsync(Func<string> operation)
        {
            SetBusy(true);

            try
            {
                string message = await Task.Factory.StartNew(operation);
                AppendLog(message);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>
        /// Enables or disables action buttons while a command is running.
        /// </summary>
        private void SetBusy(bool busy)
        {
            UseWaitCursor = busy;

            foreach (Button button in actionButtons)
            {
                button.Enabled = !busy;
            }
        }

        /// <summary>
        /// Creates a macro binding from the macro editor controls.
        /// </summary>
        private MacroBinding CreateBindingFromEditor()
        {
            MacroKeyItem keyItem = macroKeyCombo.SelectedItem as MacroKeyItem;
            MacroActionItem actionItem = macroActionCombo.SelectedItem as MacroActionItem;

            if (keyItem == null || actionItem == null)
            {
                return null;
            }

            if (!macroCtrlCheckBox.Checked && !macroAltCheckBox.Checked && !macroShiftCheckBox.Checked && !macroWinCheckBox.Checked)
            {
                DialogResult result = MessageBox.Show(
                    this,
                    T("HotkeyWithoutModifiers"),
                    T("MacroTitle"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return null;
                }
            }

            return new MacroBinding
            {
                KeyCode = (int)keyItem.Key,
                Control = macroCtrlCheckBox.Checked,
                Alt = macroAltCheckBox.Checked,
                Shift = macroShiftCheckBox.Checked,
                Windows = macroWinCheckBox.Checked,
                Action = actionItem.Action
            };
        }

        /// <summary>
        /// Prompts the user for a user profile display name.
        /// </summary>
        private string PromptForProfileName(string title, string currentName)
        {
            using (Form dialog = new Form())
            using (TextBox nameTextBox = new TextBox())
            using (Button okButton = new Button())
            using (Button cancelButton = new Button())
            using (Label label = new Label())
            {
                dialog.Text = title;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(360, 118);
                dialog.ShowInTaskbar = false;

                label.Text = T("ProfileName");
                label.AutoSize = true;
                label.Location = new Point(12, 14);

                nameTextBox.Text = currentName;
                nameTextBox.Location = new Point(12, 36);
                nameTextBox.Width = 336;
                nameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                okButton.Text = T("Ok");
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(184, 76);
                okButton.Width = 76;

                cancelButton.Text = T("Cancel");
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(272, 76);
                cancelButton.Width = 76;

                dialog.Controls.Add(label);
                dialog.Controls.Add(nameTextBox);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return null;
                }

                return nameTextBox.Text.Trim();
            }
        }

        /// <summary>
        /// Builds the selectable macro key list.
        /// </summary>
        private static List<MacroKeyItem> BuildMacroKeyList()
        {
            List<MacroKeyItem> keys = new List<MacroKeyItem>();

            for (Keys key = Keys.A; key <= Keys.Z; key++)
            {
                keys.Add(new MacroKeyItem(key.ToString(), key));
            }

            for (Keys key = Keys.F1; key <= Keys.F24; key++)
            {
                keys.Add(new MacroKeyItem(key.ToString(), key));
            }

            for (Keys key = Keys.D0; key <= Keys.D9; key++)
            {
                keys.Add(new MacroKeyItem(key.ToString().Substring(1), key));
            }

            for (Keys key = Keys.NumPad0; key <= Keys.NumPad9; key++)
            {
                keys.Add(new MacroKeyItem(key.ToString(), key));
            }

            keys.Add(new MacroKeyItem("Insert", Keys.Insert));
            keys.Add(new MacroKeyItem("Delete", Keys.Delete));
            keys.Add(new MacroKeyItem("Home", Keys.Home));
            keys.Add(new MacroKeyItem("End", Keys.End));
            keys.Add(new MacroKeyItem("PageUp", Keys.PageUp));
            keys.Add(new MacroKeyItem("PageDown", Keys.PageDown));

            return keys;
        }

        /// <summary>
        /// Builds the selectable macro action list.
        /// </summary>
        private static List<MacroActionItem> BuildMacroActionList()
        {
            return new List<MacroActionItem>
            {
                new MacroActionItem(Localization.MacroActionLabel(MacroAction.VolumeUp), MacroAction.VolumeUp),
                new MacroActionItem(Localization.MacroActionLabel(MacroAction.VolumeDown), MacroAction.VolumeDown),
                new MacroActionItem(Localization.MacroActionLabel(MacroAction.VolumeMute), MacroAction.VolumeMute),
                new MacroActionItem(Localization.MacroActionLabel(MacroAction.MediaPlayPause), MacroAction.MediaPlayPause),
                new MacroActionItem(Localization.MacroActionLabel(MacroAction.MediaNextTrack), MacroAction.MediaNextTrack),
                new MacroActionItem(Localization.MacroActionLabel(MacroAction.MediaPreviousTrack), MacroAction.MediaPreviousTrack)
            };
        }

        /// <summary>
        /// Gets the selected onboard keyboard profile number.
        /// </summary>
        private int GetKeyboardProfile()
        {
            return (int)profileNumber.Value;
        }

        /// <summary>
        /// Gets the selected built-in static profile.
        /// </summary>
        private BuiltInProfile GetSelectedBuiltInProfile()
        {
            BuiltInProfileListItem item = builtInProfileCombo.SelectedItem as BuiltInProfileListItem;
            return item == null ? null : item.Profile;
        }

        /// <summary>
        /// Gets the localized display name of the selected built-in profile.
        /// </summary>
        private string GetSelectedBuiltInProfileName()
        {
            BuiltInProfileListItem item = builtInProfileCombo.SelectedItem as BuiltInProfileListItem;
            return item == null ? string.Empty : item.Name;
        }

        /// <summary>
        /// Gets the selected user JSON profile.
        /// </summary>
        private UserProfileListItem GetSelectedUserProfile()
        {
            return userProfileCombo.SelectedItem as UserProfileListItem;
        }

        /// <summary>
        /// Hides the main window and keeps the notification icon visible.
        /// </summary>
        private void HideToTray()
        {
            Hide();
            WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Restores the main window from the notification area.
        /// </summary>
        private void ShowFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        /// <summary>
        /// Shows and logs an exception.
        /// </summary>
        private void ShowError(Exception ex)
        {
            AppendLog(TF("ErrorPrefix", ex.Message));
            MessageBox.Show(this, ex.Message, ProductDisplayName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Appends one timestamped line to the log.
        /// </summary>
        private void AppendLog(string message)
        {
            if (logTextBox == null)
            {
                return;
            }

            logTextBox.AppendText(DateTime.Now.ToString("HH:mm:ss") + "  " + message + Environment.NewLine);
        }

        /// <summary>
        /// Checks whether a command-line argument is present.
        /// </summary>
        private static bool HasArgument(string[] args, string name)
        {
            if (args == null)
            {
                return false;
            }

            foreach (string arg in args)
            {
                if (string.Equals(arg, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a localized string for the active language.
        /// </summary>
        private static string T(string key)
        {
            return Localization.Text(key);
        }

        /// <summary>
        /// Formats a localized string for the active language.
        /// </summary>
        private static string TF(string key, params object[] args)
        {
            return Localization.Format(key, args);
        }
    }

    /// <summary>
    /// Combo-box item for a localized built-in profile.
    /// </summary>
    internal sealed class BuiltInProfileListItem
    {
        public BuiltInProfileListItem(BuiltInProfile profile)
        {
            Profile = profile;
            Name = Localization.BuiltInProfileName(profile.Name);
            Description = Localization.BuiltInProfileDescription(profile.Name);
        }

        public BuiltInProfile Profile { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Combo-box item for a managed user profile file.
    /// </summary>
    internal sealed class UserProfileListItem
    {
        public UserProfileListItem(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string Path { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Combo-box item for a macro key.
    /// </summary>
    internal sealed class MacroKeyItem
    {
        public MacroKeyItem(string name, Keys key)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; private set; }

        public Keys Key { get; private set; }
    }

    /// <summary>
    /// Combo-box item for a macro action.
    /// </summary>
    internal sealed class MacroActionItem
    {
        public MacroActionItem(string name, MacroAction action)
        {
            Name = name;
            Action = action;
        }

        public string Name { get; private set; }

        public MacroAction Action { get; private set; }
    }

    /// <summary>
    /// Lightweight keyboard preview control that renders known key slots with their assigned colors.
    /// </summary>
    internal sealed class KeyboardPreviewPanel : Panel
    {
        private KeyColor[] keys;

        public KeyboardPreviewPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(28, 28, 32);
            MinimumSize = new Size(240, 120);
        }

        public KeyColor[] Keys
        {
            get { return keys; }
            set
            {
                keys = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Paints the keyboard preview using normalized key layout coordinates.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(BackColor);

            if (keys == null || keys.Length == 0)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(150, 150, 150)))
                {
                    e.Graphics.DrawString(Localization.Text("PreviewNoProfile"), Font, brush, new PointF(10, 10));
                }

                return;
            }

            float maxX = 1f;
            float maxY = 1f;

            foreach (KeyboardKeySlot key in KeyboardLayout.Keys)
            {
                maxX = Math.Max(maxX, key.X + key.Width);
                maxY = Math.Max(maxY, key.Y);
            }

            float padding = 8f;
            float availableWidth = Math.Max(1f, Width - (padding * 2f));
            float availableHeight = Math.Max(1f, Height - (padding * 2f));
            float unit = Math.Max(1f, Math.Min(availableWidth / maxX, availableHeight / (maxY + 1f)));
            float contentWidth = maxX * unit;
            float contentHeight = (maxY + 1f) * unit;
            float originX = padding + ((availableWidth - contentWidth) / 2f);
            float originY = padding + ((availableHeight - contentHeight) / 2f);
            float keyHeight = Math.Max(4f, unit * 0.72f);

            using (Pen borderPen = new Pen(Color.FromArgb(80, 80, 86)))
            {
                foreach (KeyboardKeySlot key in KeyboardLayout.Keys)
                {
                    KeyColor color = keys[key.Index];
                    RectangleF rect = new RectangleF(
                        originX + (key.X * unit),
                        originY + (key.Y * unit),
                        Math.Max(3f, (key.Width * unit) - 2f),
                        Math.Max(3f, keyHeight));

                    using (Brush brush = new SolidBrush(Color.FromArgb(color.Red, color.Green, color.Blue)))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }

                    e.Graphics.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }
    }
}



