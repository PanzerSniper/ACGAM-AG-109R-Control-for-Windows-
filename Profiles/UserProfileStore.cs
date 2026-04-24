using System;
using System.Collections.Generic;
using System.IO;

namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Manages editable JSON profiles stored in the user's local application data folder.
    /// </summary>
    internal static class UserProfileStore
    {
        /// <summary>
        /// Gets the directory where user-editable profiles are stored.
        /// </summary>
        public static string DirectoryPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AC109RDriverWin",
                    "Profiles");
            }
        }

        /// <summary>
        /// Returns all managed JSON profile files.
        /// </summary>
        public static IList<string> GetProfileFiles()
        {
            EnsureDirectory();

            string[] files = Directory.GetFiles(DirectoryPath, "*.json");
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);
            return files;
        }

        /// <summary>
        /// Saves a generated profile as a new user-editable JSON file.
        /// </summary>
        public static string SaveCopy(string name, KeyColor[] keys)
        {
            EnsureDirectory();

            string safeName = MakeSafeFileName(name);
            if (string.IsNullOrEmpty(safeName))
            {
                safeName = "profile";
            }

            string path = GetAvailablePath(safeName);
            ProfileParser.SaveJson(path, keys);
            return path;
        }

        /// <summary>
        /// Imports an external JSON profile by validating it and saving a managed copy.
        /// </summary>
        public static string Import(string sourcePath)
        {
            EnsureDirectory();

            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            {
                throw new FileNotFoundException(Localization.Text("JsonFileNotFound"), sourcePath);
            }

            ProfileData data = ProfileParser.LoadJson(sourcePath);
            string name = Path.GetFileNameWithoutExtension(sourcePath);
            return SaveCopy(name, data.Keys);
        }

        /// <summary>
        /// Renames a managed profile file and returns the new path.
        /// </summary>
        public static string Rename(string currentPath, string newName)
        {
            EnsureDirectory();
            ValidateManagedProfilePath(currentPath);

            string safeName = MakeSafeFileName(newName);
            if (string.IsNullOrEmpty(safeName))
            {
                throw new InvalidOperationException(Localization.Text("ProfileNameRequired"));
            }

            string targetPath = Path.Combine(DirectoryPath, safeName + ".json");
            if (string.Equals(Path.GetFullPath(currentPath), Path.GetFullPath(targetPath), StringComparison.OrdinalIgnoreCase))
            {
                return currentPath;
            }

            if (File.Exists(targetPath))
            {
                throw new IOException(Localization.Text("ProfileAlreadyExists"));
            }

            File.Move(currentPath, targetPath);
            return targetPath;
        }

        /// <summary>
        /// Duplicates a managed profile file and returns the copied path.
        /// </summary>
        public static string Duplicate(string sourcePath)
        {
            EnsureDirectory();
            ValidateManagedProfilePath(sourcePath);

            string baseName = Path.GetFileNameWithoutExtension(sourcePath) + "-copy";
            string targetPath = GetAvailablePath(MakeSafeFileName(baseName));
            File.Copy(sourcePath, targetPath);
            return targetPath;
        }

        /// <summary>
        /// Deletes a managed profile file.
        /// </summary>
        public static void Delete(string path)
        {
            EnsureDirectory();
            ValidateManagedProfilePath(path);
            File.Delete(path);
        }

        /// <summary>
        /// Ensures the profile storage directory exists.
        /// </summary>
        private static void EnsureDirectory()
        {
            Directory.CreateDirectory(DirectoryPath);
        }

        /// <summary>
        /// Creates a unique path for the given safe profile name.
        /// </summary>
        private static string GetAvailablePath(string safeName)
        {
            string path = Path.Combine(DirectoryPath, safeName + ".json");
            int suffix = 2;
            while (File.Exists(path))
            {
                path = Path.Combine(DirectoryPath, safeName + "-" + suffix + ".json");
                suffix++;
            }

            return path;
        }

        /// <summary>
        /// Verifies that a path points to an existing JSON profile in the managed profile directory.
        /// </summary>
        private static void ValidateManagedProfilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                throw new FileNotFoundException(Localization.Text("ProfileFileNotFound"), path);
            }

            string fullDirectory = Path.GetFullPath(DirectoryPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string fullPath = Path.GetFullPath(path);
            if (!fullPath.StartsWith(fullDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(Localization.Text("ManagedProfilesOnly"));
            }

            if (!string.Equals(Path.GetExtension(fullPath), ".json", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(Localization.Text("JsonProfilesOnly"));
            }
        }

        /// <summary>
        /// Converts a display name into a safe JSON file name.
        /// </summary>
        private static string MakeSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            string safeName = name.Trim().ToLowerInvariant().Replace(' ', '-');
            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                safeName = safeName.Replace(invalid.ToString(), string.Empty);
            }

            return safeName;
        }
    }
}
