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
            { "Detect", "Détecter" },
            { "Ping", "Tester" },
            { "Activate", "Activer" },
            { "TurnOff", "Éteindre" },
            { "StaticProfiles", "Profils statiques" },
            { "Macros", "Macros" },
            { "Settings", "Paramètres" },
            { "Log", "Journal" },
            { "Changelog", "Changelog" },
            { "BuiltInPresets", "Profils intégrés" },
            { "PersonalProfiles", "Profils personnels" },
            { "SendToKeyboard", "Envoyer au clavier" },
            { "CreateProfile", "Créer un profil" },
            { "ImportJson", "Importer JSON" },
            { "Rename", "Renommer" },
            { "Duplicate", "Dupliquer" },
            { "Delete", "Supprimer" },
            { "OpenFolder", "Ouvrir le dossier" },
            { "Refresh", "Actualiser" },
            { "QuickActions", "Actions rapides" },
            { "PickColor", "Choisir couleur" },
            { "FillProfile", "Remplir profil" },
            { "Browse", "Parcourir" },
            { "LoadJson", "Charger JSON" },
            { "Shortcut", "Raccourci" },
            { "Action", "Action" },
            { "AddMacro", "Ajouter macro" },
            { "RemoveSelected", "Retirer sélection" },
            { "MacroNote", "Les macros sont des raccourcis globaux Windows. Elles fonctionnent tant que l'application est ouverte ou réduite dans la zone de notification." },
            { "StartWithWindows", "Lancer avec Windows" },
            { "StartMinimized", "Lancer réduit au démarrage Windows" },
            { "Language", "Langue" },
            { "SaveSettings", "Enregistrer" },
            { "ApplicationData", "Données application : {0}" },
            { "Open", "Ouvrir" },
            { "Exit", "Quitter" },
            { "BuildLabel", "Build" },
            { "Ready", "Prêt. VID/PID USB attendu : 1EA7:0907." },
            { "ClosedToTray", "Fenêtre réduite dans la zone de notification. Utilisez Quitter dans le menu de l'icône pour fermer l'application." },
            { "NoUserProfile", "Aucun profil personnel. Créez un profil depuis un modèle ou importez un fichier JSON." },
            { "MappedKeys", "{0} touche(s) mappée(s)" },
            { "InvalidProfile", "Profil invalide : {0}" },
            { "BuiltInReadOnly", "Les profils intégrés sont en lecture seule." },
            { "NoDeviceFound", "Aucun clavier AC109R accessible trouvé." },
            { "DeviceFound", "Clavier trouvé : {0}" },
            { "PingOk", "Test OK." },
            { "ProfileActivated", "Profil {0} activé." },
            { "ProfileTurnedOff", "Profil {0} éteint." },
            { "ProfileFilled", "Profil {0} rempli en RGB({1}, {2}, {3})." },
            { "PresetSent", "Profil intégré \"{0}\" envoyé au profil clavier {1}." },
            { "PresetCopied", "Profil personnel créé : {0}" },
            { "NoUserProfileSelected", "Aucun profil personnel sélectionné." },
            { "JsonProfileSent", "Profil JSON \"{0}\" envoyé au profil clavier {1}." },
            { "JsonProfileFilter", "Profils JSON (*.json)|*.json|Tous les fichiers (*.*)|*.*" },
            { "ImportJsonTitle", "Importer un profil JSON" },
            { "ProfileImported", "Profil importé : {0}" },
            { "RenameProfileTitle", "Renommer le profil" },
            { "ProfileRenamed", "Profil renommé : {0}" },
            { "ProfileDuplicated", "Profil dupliqué : {0}" },
            { "DeleteProfileQuestion", "Supprimer le profil \"{0}\" ?" },
            { "ProfileDeleted", "Profil supprimé : {0}" },
            { "ProfileListRefreshed", "Liste des profils actualisée." },
            { "ChooseProfileTitle", "Choisir un profil AC109R" },
            { "JsonLoaded", "Profil {0} chargé depuis le JSON ({1} touche(s))." },
            { "IgnoredKeys", " Touches ignorées : {0}" },
            { "DuplicateHotkey", "Ce raccourci est déjà configuré." },
            { "MacroTitle", "Macros AC109R" },
            { "HotkeyWithoutModifiers", "Les raccourcis globaux sans modificateur peuvent gêner la saisie normale. Continuer ?" },
            { "SettingsSaved", "Paramètres enregistrés." },
            { "ErrorPrefix", "Erreur : {0}" },
            { "ProfileName", "Nom du profil" },
            { "ProfileNameRequired", "Le nom du profil est obligatoire." },
            { "ProfileAlreadyExists", "Un profil porte déjà ce nom." },
            { "ManagedProfilesOnly", "Seuls les profils personnels gérés par l'application peuvent être modifiés." },
            { "JsonProfilesOnly", "Seuls les fichiers de profil JSON peuvent être modifiés." },
            { "ProfileFileNotFound", "Fichier de profil introuvable." },
            { "JsonFileNotSelected", "Aucun fichier JSON sélectionné." },
            { "JsonFileNotFound", "Fichier JSON introuvable." },
            { "JsonMustBeObject", "Le profil JSON doit être un objet clé/valeur." },
            { "JsonValueMustBeArray", "Chaque valeur du profil JSON doit être un tableau." },
            { "JsonKeyValueCount", "La touche \"{0}\" doit contenir 1, 3 ou 4 valeurs." },
            { "JsonKeyByteRange", "La touche \"{0}\" contient une valeur hors de la plage 0-255." },
            { "ProfileSlotCount", "Le profil doit contenir 131 emplacements de touches." },
            { "HotkeyRegisterFailed", "Impossible d'enregistrer le raccourci {0}." },
            { "AlreadyRunning", "AC109R Control est déjà lancé." },
            { "ChangelogText", "1.0.0\r\n- Première version stable d'AC109R Control.\r\n- Profils intégrés en lecture seule avec création de profils personnels.\r\n- Gestion des profils personnels : import, renommage, duplication, suppression et ouverture du dossier.\r\n- Macros logicielles globales pour le volume et les médias.\r\n- Lancement avec Windows, démarrage réduit et icône de zone de notification.\r\n- Interface français / anglais avec réglage persistant.\r\n- Instance unique pour éviter deux lancements simultanés.\r\n- Aperçu clavier proportionnel et logo embarqué dans les ressources." },
            { "Ok", "OK" },
            { "Cancel", "Annuler" },
            { "PreviewNoProfile", "Aucun profil" },
            { "MacroRegistered", "{0} macro(s) enregistrée(s)." },
            { "MacroRegistrationFailed", "Échec d'enregistrement des macros : {0}" },
            { "VolumeUp", "Augmenter le volume" },
            { "VolumeDown", "Baisser le volume" },
            { "VolumeMute", "Couper le son" },
            { "MediaPlayPause", "Lecture / pause" },
            { "MediaNextTrack", "Piste suivante" },
            { "MediaPreviousTrack", "Piste précédente" },
            { "PresetRainbowHorizontal", "Arc-en-ciel horizontal" },
            { "PresetRainbowHorizontalDescription", "Dégradé multicolore de gauche à droite." },
            { "PresetRainbowDiagonal", "Arc-en-ciel diagonal" },
            { "PresetRainbowDiagonalDescription", "Dégradé multicolore en diagonale." },
            { "PresetPinkBlueAurora", "Aurore rose/bleu" },
            { "PresetPinkBlueAuroraDescription", "Dégradé doux rose, violet et bleu." },
            { "PresetOcean", "Océan" },
            { "PresetOceanDescription", "Bleu profond vers cyan lumineux." },
            { "PresetFire", "Feu" },
            { "PresetFireDescription", "Dégradé rouge, orange et jaune." },
            { "PresetWarmWhite", "Blanc chaud" },
            { "PresetWarmWhiteDescription", "Éclairage blanc légèrement chaud." },
            { "PresetRoseAC109", "Rose AC109" },
            { "PresetRoseAC109Description", "Couleur proche du modèle fourni par le dépôt Linux." }
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
            { "NoDeviceFound", "No accessible AC109R device found." },
            { "DeviceFound", "Device found: {0}" },
            { "PingOk", "Ping OK." },
            { "ProfileActivated", "Profile {0} activated." },
            { "ProfileTurnedOff", "Profile {0} turned off." },
            { "ProfileFilled", "Profile {0} filled with RGB({1}, {2}, {3})." },
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
            { "ChooseProfileTitle", "Choose an AC109R profile" },
            { "JsonLoaded", "Profile {0} loaded from JSON ({1} keys)." },
            { "IgnoredKeys", " Ignored keys: {0}" },
            { "DuplicateHotkey", "This shortcut is already configured." },
            { "MacroTitle", "AC109R macros" },
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
            { "AlreadyRunning", "AC109R Control is already running." },
            { "ChangelogText", "1.0.0\r\n- First stable AC109R Control release.\r\n- Read-only built-in presets with personal profile creation.\r\n- Personal profile management: import, rename, duplicate, delete, and open folder.\r\n- Global software macros for volume and media actions.\r\n- Start with Windows, start minimized, and notification area icon.\r\n- Persistent French / English interface setting.\r\n- Single-instance guard to prevent duplicate launches.\r\n- Proportional keyboard preview and logo embedded in application resources." },
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
            { "PresetRoseAC109", "Rose AC109" },
            { "PresetRoseAC109Description", "Color close to the template provided by the Linux repository." }
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
                case "Rose AC109":
                    return "PresetRoseAC109";
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
