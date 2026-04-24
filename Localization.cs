using Ac109RDriverWin.Macros;
using System.Collections.Generic;

namespace Ac109RDriverWin
{
    /// <summary>
    /// Provides user-facing text for the supported application languages.
    /// </summary>
    internal static class Localization
    {
        public const string French = "fr";
        public const string English = "en";

        private static string languageCode = French;

        private static readonly Dictionary<string, string> FrenchTexts = new Dictionary<string, string>
        {
            { "KeyboardProfile", "Profil clavier" },
            { "Detect", "D\u00e9tecter" },
            { "Ping", "Tester" },
            { "Activate", "Activer" },
            { "TurnOff", "\u00c9teindre" },
            { "StaticProfiles", "Profils statiques" },
            { "Macros", "Macros" },
            { "Settings", "Param\u00e8tres" },
            { "Log", "Journal" },
            { "Changelog", "Changelog" },
            { "BuiltInPresets", "Profils int\u00e9gr\u00e9s" },
            { "PersonalProfiles", "Profils personnels" },
            { "SendToKeyboard", "Envoyer au clavier" },
            { "CreateProfile", "Cr\u00e9er un profil" },
            { "ImportJson", "Importer JSON" },
            { "Rename", "Renommer" },
            { "Duplicate", "Dupliquer" },
            { "Delete", "Supprimer" },
            { "OpenFolder", "Ouvrir le dossier" },
            { "Refresh", "Actualiser" },
            { "QuickActions", "Actions rapides" },
            { "PickColor", "Choisir couleur" },
            { "FillProfile", "Remplir profil" },
            { "SaveColorProfile", "Enregistrer en profil" },
            { "Browse", "Parcourir" },
            { "LoadJson", "Charger JSON" },
            { "Shortcut", "Raccourci" },
            { "Action", "Action" },
            { "AddMacro", "Ajouter macro" },
            { "RemoveSelected", "Retirer s\u00e9lection" },
            { "MacroNote", "Les macros sont des raccourcis globaux Windows. Elles fonctionnent tant que l'application est ouverte ou r\u00e9duite dans la zone de notification." },
            { "StartWithWindows", "Lancer avec Windows" },
            { "StartMinimized", "Lancer r\u00e9duit au d\u00e9marrage Windows" },
            { "Language", "Langue" },
            { "SaveSettings", "Enregistrer" },
            { "ApplicationData", "Donn\u00e9es application : {0}" },
            { "Open", "Ouvrir" },
            { "Exit", "Quitter" },
            { "BuildLabel", "Build" },
            { "Ready", "Pr\u00eat. VID/PID USB attendu : 1EA7:0907." },
            { "ClosedToTray", "Fen\u00eatre r\u00e9duite dans la zone de notification. Utilisez Quitter dans le menu de l'ic\u00f4ne pour fermer l'application." },
            { "NoUserProfile", "Aucun profil personnel. Cr\u00e9ez un profil depuis un mod\u00e8le ou importez un fichier JSON." },
            { "MappedKeys", "{0} touche(s) mapp\u00e9e(s)" },
            { "InvalidProfile", "Profil invalide : {0}" },
            { "BuiltInReadOnly", "Les profils int\u00e9gr\u00e9s sont en lecture seule." },
            { "NoDeviceFound", "Aucun clavier ACGAM AG-109R accessible trouv\u00e9." },
            { "DeviceFound", "Interface(s) ACGAM AG-109R trouv\u00e9e(s) :\r\n{0}" },
            { "PingOk", "Test OK." },
            { "ProfileActivated", "Profil {0} activ\u00e9." },
            { "ProfileTurnedOff", "Profil {0} \u00e9teint." },
            { "ProfileFilled", "Profil {0} rempli en RGB({1}, {2}, {3})." },
            { "SaveColorProfileTitle", "Enregistrer la couleur" },
            { "ColorProfileDefaultName", "Couleur RGB {0}-{1}-{2}" },
            { "ColorProfileCreated", "Profil couleur cr\u00e9\u00e9 : {0}" },
            { "PresetSent", "Profil int\u00e9gr\u00e9 \"{0}\" envoy\u00e9 au profil clavier {1}." },
            { "PresetCopied", "Profil personnel cr\u00e9\u00e9 : {0}" },
            { "NoUserProfileSelected", "Aucun profil personnel s\u00e9lectionn\u00e9." },
            { "JsonProfileSent", "Profil JSON \"{0}\" envoy\u00e9 au profil clavier {1}." },
            { "JsonProfileFilter", "Profils JSON (*.json)|*.json|Tous les fichiers (*.*)|*.*" },
            { "ImportJsonTitle", "Importer un profil JSON" },
            { "ProfileImported", "Profil import\u00e9 : {0}" },
            { "RenameProfileTitle", "Renommer le profil" },
            { "ProfileRenamed", "Profil renomm\u00e9 : {0}" },
            { "ProfileDuplicated", "Profil dupliqu\u00e9 : {0}" },
            { "DeleteProfileQuestion", "Supprimer le profil \"{0}\" ?" },
            { "ProfileDeleted", "Profil supprim\u00e9 : {0}" },
            { "ProfileListRefreshed", "Liste des profils actualis\u00e9e." },
            { "ChooseProfileTitle", "Choisir un profil AG109R" },
            { "JsonLoaded", "Profil {0} charg\u00e9 depuis le JSON ({1} touche(s))." },
            { "IgnoredKeys", " Touches ignor\u00e9es : {0}" },
            { "DuplicateHotkey", "Ce raccourci est d\u00e9j\u00e0 configur\u00e9." },
            { "MacroTitle", "Macros AG109R" },
            { "HotkeyWithoutModifiers", "Les raccourcis globaux sans modificateur peuvent g\u00eaner la saisie normale. Continuer ?" },
            { "SettingsSaved", "Param\u00e8tres enregistr\u00e9s." },
            { "ErrorPrefix", "Erreur : {0}" },
            { "ProfileName", "Nom du profil" },
            { "ProfileNameRequired", "Le nom du profil est obligatoire." },
            { "ProfileAlreadyExists", "Un profil porte d\u00e9j\u00e0 ce nom." },
            { "ManagedProfilesOnly", "Seuls les profils personnels g\u00e9r\u00e9s par l'application peuvent \u00eatre modifi\u00e9s." },
            { "JsonProfilesOnly", "Seuls les fichiers de profil JSON peuvent \u00eatre modifi\u00e9s." },
            { "ProfileFileNotFound", "Fichier de profil introuvable." },
            { "JsonFileNotSelected", "Aucun fichier JSON s\u00e9lectionn\u00e9." },
            { "JsonFileNotFound", "Fichier JSON introuvable." },
            { "JsonMustBeObject", "Le profil JSON doit \u00eatre un objet cl\u00e9/valeur." },
            { "JsonValueMustBeArray", "Chaque valeur du profil JSON doit \u00eatre un tableau." },
            { "JsonKeyValueCount", "La touche \"{0}\" doit contenir 1, 3 ou 4 valeurs." },
            { "JsonKeyByteRange", "La touche \"{0}\" contient une valeur hors de la plage 0-255." },
            { "ProfileSlotCount", "Le profil doit contenir 131 emplacements de touches." },
            { "HotkeyRegisterFailed", "Impossible d'enregistrer le raccourci {0}." },
            { "AlreadyRunning", "AG109R Control est d\u00e9j\u00e0 lanc\u00e9." },
            { "ChangelogText", "Version 1.0.1 - 24/04/2026\r\n\r\nCorrections\r\n- D\u00e9tection HID plus stricte : l'application essaie chaque interface du clavier jusqu'\u00e0 trouver celle qui r\u00e9pond vraiment.\r\n- Les timeouts de lecture HID restent affich\u00e9s au lieu d'\u00eatre masqu\u00e9s comme des succ\u00e8s.\r\n- Messages et documentation align\u00e9s pour la version 1.0.1.\r\n\r\nNotes\r\n- Si le clavier est d\u00e9tect\u00e9 mais ne r\u00e9pond pas au ping HID, aucune commande lumineuse n'est consid\u00e9r\u00e9e comme r\u00e9ussie.\r\n- Fermez le logiciel officiel ACGAM s'il est ouvert, puis essayez un autre port USB si le timeout persiste." },
            { "Ok", "OK" },
            { "Cancel", "Annuler" },
            { "PreviewNoProfile", "Aucun profil" },
            { "MacroRegistered", "{0} macro(s) enregistr\u00e9e(s)." },
            { "MacroRegistrationFailed", "\u00c9chec d'enregistrement des macros : {0}" },
            { "VolumeUp", "Augmenter le volume" },
            { "VolumeDown", "Baisser le volume" },
            { "VolumeMute", "Couper le son" },
            { "MediaPlayPause", "Lecture / pause" },
            { "MediaNextTrack", "Piste suivante" },
            { "MediaPreviousTrack", "Piste pr\u00e9c\u00e9dente" },
            { "PresetRainbowHorizontal", "Arc-en-ciel horizontal" },
            { "PresetRainbowHorizontalDescription", "D\u00e9grad\u00e9 multicolore de gauche \u00e0 droite." },
            { "PresetRainbowDiagonal", "Arc-en-ciel diagonal" },
            { "PresetRainbowDiagonalDescription", "D\u00e9grad\u00e9 multicolore en diagonale." },
            { "PresetPinkBlueAurora", "Aurore rose/bleu" },
            { "PresetPinkBlueAuroraDescription", "D\u00e9grad\u00e9 doux rose, violet et bleu." },
            { "PresetOcean", "Oc\u00e9an" },
            { "PresetOceanDescription", "Bleu profond vers cyan lumineux." },
            { "PresetFire", "Feu" },
            { "PresetFireDescription", "D\u00e9grad\u00e9 rouge, orange et jaune." },
            { "PresetWarmWhite", "Blanc chaud" },
            { "PresetWarmWhiteDescription", "\u00c9clairage blanc l\u00e9g\u00e8rement chaud." },
            { "PresetRoseAG109R", "Rose AG109R" },
            { "PresetRoseAG109RDescription", "Couleur proche du modèle fourni par le dépôt Linux." }
        };

        private static readonly Dictionary<string, string> EnglishTexts = new Dictionary<string, string>
        {
            { "KeyboardProfile", "Keyboard profile" },
            { "Detect", "Detect" },
            { "Ping", "Ping" },
            { "Activate", "Activate" },
            { "TurnOff", "Turn off" },
            { "StaticProfiles", "Static profiles" },
            { "Macros", "Macros" },
            { "Settings", "Settings" },
            { "Log", "Log" },
            { "Changelog", "Changelog" },
            { "BuiltInPresets", "Built-in presets" },
            { "PersonalProfiles", "Personal profiles" },
            { "SendToKeyboard", "Send to keyboard" },
            { "CreateProfile", "Create profile" },
            { "ImportJson", "Import JSON" },
            { "Rename", "Rename" },
            { "Duplicate", "Duplicate" },
            { "Delete", "Delete" },
            { "OpenFolder", "Open folder" },
            { "Refresh", "Refresh" },
            { "QuickActions", "Quick actions" },
            { "PickColor", "Pick color" },
            { "FillProfile", "Fill profile" },
            { "SaveColorProfile", "Save as profile" },
            { "Browse", "Browse" },
            { "LoadJson", "Load JSON" },
            { "Shortcut", "Shortcut" },
            { "Action", "Action" },
            { "AddMacro", "Add macro" },
            { "RemoveSelected", "Remove selected" },
            { "MacroNote", "Macros are global Windows hotkeys. They run while the application is open or minimized to the notification area." },
            { "StartWithWindows", "Start with Windows" },
            { "StartMinimized", "Start minimized when launched by Windows startup" },
            { "Language", "Language" },
            { "SaveSettings", "Save settings" },
            { "ApplicationData", "Application data: {0}" },
            { "Open", "Open" },
            { "Exit", "Exit" },
            { "BuildLabel", "Build" },
            { "Ready", "Ready. Expected USB VID/PID: 1EA7:0907." },
            { "ClosedToTray", "Window closed to the notification area. Use Exit from the tray menu to quit." },
            { "NoUserProfile", "No personal profile yet. Create one from a preset or import a JSON file." },
            { "MappedKeys", "{0} mapped key(s)" },
            { "InvalidProfile", "Invalid profile: {0}" },
            { "BuiltInReadOnly", "Built-in presets are read-only." },
            { "NoDeviceFound", "No accessible ACGAM AG-109R device found." },
            { "DeviceFound", "ACGAM AG-109R interface(s) found:\r\n{0}" },
            { "PingOk", "Ping OK." },
            { "ProfileActivated", "Profile {0} activated." },
            { "ProfileTurnedOff", "Profile {0} turned off." },
            { "ProfileFilled", "Profile {0} filled with RGB({1}, {2}, {3})." },
            { "SaveColorProfileTitle", "Save color" },
            { "ColorProfileDefaultName", "RGB color {0}-{1}-{2}" },
            { "ColorProfileCreated", "Color profile created: {0}" },
            { "PresetSent", "Preset \"{0}\" sent to profile {1}." },
            { "PresetCopied", "Personal profile created: {0}" },
            { "NoUserProfileSelected", "No personal profile selected." },
            { "JsonProfileSent", "JSON profile \"{0}\" sent to profile {1}." },
            { "JsonProfileFilter", "JSON profiles (*.json)|*.json|All files (*.*)|*.*" },
            { "ImportJsonTitle", "Import a JSON profile" },
            { "ProfileImported", "Profile imported: {0}" },
            { "RenameProfileTitle", "Rename profile" },
            { "ProfileRenamed", "Profile renamed: {0}" },
            { "ProfileDuplicated", "Profile duplicated: {0}" },
            { "DeleteProfileQuestion", "Delete the profile \"{0}\"?" },
            { "ProfileDeleted", "Profile deleted: {0}" },
            { "ProfileListRefreshed", "User profile list refreshed." },
            { "ChooseProfileTitle", "Choose an AG109R profile" },
            { "JsonLoaded", "Profile {0} loaded from JSON ({1} keys)." },
            { "IgnoredKeys", " Ignored keys: {0}" },
            { "DuplicateHotkey", "This shortcut is already configured." },
            { "MacroTitle", "AG109R macros" },
            { "HotkeyWithoutModifiers", "Global hotkeys without modifiers can interfere with normal typing. Continue?" },
            { "SettingsSaved", "Settings saved." },
            { "ErrorPrefix", "Error: {0}" },
            { "ProfileName", "Profile name" },
            { "ProfileNameRequired", "Profile name is required." },
            { "ProfileAlreadyExists", "A profile with this name already exists." },
            { "ManagedProfilesOnly", "Only managed user profiles can be modified." },
            { "JsonProfilesOnly", "Only JSON profile files can be modified." },
            { "ProfileFileNotFound", "Profile file not found." },
            { "JsonFileNotSelected", "No JSON file selected." },
            { "JsonFileNotFound", "JSON file not found." },
            { "JsonMustBeObject", "The JSON profile must be a key/value object." },
            { "JsonValueMustBeArray", "Each JSON profile value must be an array." },
            { "JsonKeyValueCount", "Key \"{0}\" must contain 1, 3, or 4 values." },
            { "JsonKeyByteRange", "Key \"{0}\" contains a value outside the 0-255 range." },
            { "ProfileSlotCount", "The profile must contain 131 key slots." },
            { "HotkeyRegisterFailed", "Unable to register hotkey {0}." },
            { "AlreadyRunning", "AG109R Control is already running." },
            { "ChangelogText", "Version 1.0.1 - 2026-04-24\r\n\r\nFixes\r\n- Stricter HID probing: the application now tries each keyboard interface until one really answers.\r\n- HID read timeouts remain visible instead of being treated as successful writes.\r\n- Messages and documentation updated for version 1.0.1.\r\n\r\nNotes\r\n- If the keyboard is detected but does not answer the HID ping, no lighting command is treated as successful.\r\n- Close the official ACGAM software if it is running, then try another USB port if the timeout persists." },
            { "Ok", "OK" },
            { "Cancel", "Cancel" },
            { "PreviewNoProfile", "No profile" },
            { "MacroRegistered", "Registered {0} macro(s)." },
            { "MacroRegistrationFailed", "Macro registration failed: {0}" },
            { "VolumeUp", "Volume up" },
            { "VolumeDown", "Volume down" },
            { "VolumeMute", "Mute volume" },
            { "MediaPlayPause", "Play / pause" },
            { "MediaNextTrack", "Next track" },
            { "MediaPreviousTrack", "Previous track" },
            { "PresetRainbowHorizontal", "Rainbow horizontal" },
            { "PresetRainbowHorizontalDescription", "Multicolor gradient from left to right." },
            { "PresetRainbowDiagonal", "Rainbow diagonal" },
            { "PresetRainbowDiagonalDescription", "Multicolor diagonal gradient." },
            { "PresetPinkBlueAurora", "Pink/blue aurora" },
            { "PresetPinkBlueAuroraDescription", "Soft pink, purple, and blue gradient." },
            { "PresetOcean", "Ocean" },
            { "PresetOceanDescription", "Deep blue to bright cyan." },
            { "PresetFire", "Fire" },
            { "PresetFireDescription", "Red, orange, and yellow fire gradient." },
            { "PresetWarmWhite", "Warm white" },
            { "PresetWarmWhiteDescription", "Slightly warm white lighting." },
            { "PresetRoseAG109R", "Rose AG109R" },
            { "PresetRoseAG109RDescription", "Color close to the template provided by the Linux repository." }
        };

        /// <summary>
        /// Gets or sets the active UI language code.
        /// </summary>
        public static string LanguageCode
        {
            get { return languageCode; }
            set { languageCode = NormalizeLanguage(value); }
        }

        /// <summary>
        /// Returns the supported language options for the settings combo box.
        /// </summary>
        public static IList<LanguageOption> GetLanguageOptions()
        {
            return new List<LanguageOption>
            {
                new LanguageOption(French, "Français"),
                new LanguageOption(English, "English")
            };
        }

        /// <summary>
        /// Normalizes an arbitrary language code to a supported application language.
        /// </summary>
        public static string NormalizeLanguage(string code)
        {
            if (string.Equals(code, English, System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(code, "en-US", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(code, "en-GB", System.StringComparison.OrdinalIgnoreCase))
            {
                return English;
            }

            return French;
        }

        /// <summary>
        /// Gets a localized text value by key.
        /// </summary>
        public static string Text(string key)
        {
            Dictionary<string, string> texts = languageCode == English ? EnglishTexts : FrenchTexts;
            string value;
            if (texts.TryGetValue(key, out value))
            {
                return value;
            }

            if (EnglishTexts.TryGetValue(key, out value))
            {
                return value;
            }

            return key;
        }

        /// <summary>
        /// Formats a localized text value.
        /// </summary>
        public static string Format(string key, params object[] args)
        {
            return string.Format(Text(key), args);
        }

        /// <summary>
        /// Gets the localized label for a macro action.
        /// </summary>
        public static string MacroActionLabel(MacroAction action)
        {
            switch (action)
            {
                case MacroAction.VolumeUp:
                    return Text("VolumeUp");
                case MacroAction.VolumeDown:
                    return Text("VolumeDown");
                case MacroAction.VolumeMute:
                    return Text("VolumeMute");
                case MacroAction.MediaPlayPause:
                    return Text("MediaPlayPause");
                case MacroAction.MediaNextTrack:
                    return Text("MediaNextTrack");
                case MacroAction.MediaPreviousTrack:
                    return Text("MediaPreviousTrack");
                default:
                    return action.ToString();
            }
        }

        /// <summary>
        /// Gets the localized display name for a built-in profile.
        /// </summary>
        public static string BuiltInProfileName(string internalName)
        {
            return Text(GetBuiltInProfileKey(internalName));
        }

        /// <summary>
        /// Gets the localized description for a built-in profile.
        /// </summary>
        public static string BuiltInProfileDescription(string internalName)
        {
            return Text(GetBuiltInProfileKey(internalName) + "Description");
        }

        /// <summary>
        /// Maps stable built-in profile names to localization keys.
        /// </summary>
        private static string GetBuiltInProfileKey(string internalName)
        {
            switch (internalName)
            {
                case "Rainbow horizontal":
                    return "PresetRainbowHorizontal";
                case "Rainbow diagonal":
                    return "PresetRainbowDiagonal";
                case "Pink/blue aurora":
                    return "PresetPinkBlueAurora";
                case "Ocean":
                    return "PresetOcean";
                case "Fire":
                    return "PresetFire";
                case "Warm white":
                    return "PresetWarmWhite";
                case "Rose AG109R":
                    return "PresetRoseAG109R";
                default:
                    return internalName;
            }
        }
    }

    /// <summary>
    /// Combo-box item representing one supported UI language.
    /// </summary>
    internal sealed class LanguageOption
    {
        public LanguageOption(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; private set; }

        public string Name { get; private set; }
    }
}
