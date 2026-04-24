using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Parses and writes the JSON profile format used by the original Linux project.
    /// </summary>
    internal static class ProfileParser
    {
        /// <summary>
        /// Number of key slots transferred by the AG109R profile stream.
        /// </summary>
        public const int KeyboardKeyCount = 131;

        /// <summary>
        /// Creates an empty 131-slot profile.
        /// </summary>
        public static KeyColor[] CreateEmpty()
        {
            return new KeyColor[KeyboardKeyCount];
        }

        /// <summary>
        /// Creates a full profile filled with one opaque RGB color.
        /// </summary>
        public static KeyColor[] CreateFilled(byte red, byte green, byte blue)
        {
            KeyColor[] keys = new KeyColor[KeyboardKeyCount];
            KeyColor color = KeyColor.FromRgb(red, green, blue);

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = color;
            }

            return keys;
        }

        /// <summary>
        /// Loads a JSON profile and maps known key names to firmware indexes.
        /// </summary>
        public static ProfileData LoadJson(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(Localization.Text("JsonFileNotSelected"));
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(Localization.Text("JsonFileNotFound"), fileName);
            }

            string json = File.ReadAllText(fileName, new UTF8Encoding(false, true));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            object root = serializer.DeserializeObject(json);
            Dictionary<string, object> values = root as Dictionary<string, object>;

            if (values == null)
            {
                throw new FormatException(Localization.Text("JsonMustBeObject"));
            }

            KeyColor[] keys = CreateEmpty();
            List<string> unknownKeys = new List<string>();
            int appliedKeys = 0;

            foreach (KeyValuePair<string, object> pair in values)
            {
                int index;
                if (!KeyMap.TryGetIndex(pair.Key, out index))
                {
                    unknownKeys.Add(pair.Key);
                    continue;
                }

                keys[index] = ParseColor(pair.Key, pair.Value);
                appliedKeys++;
            }

            return new ProfileData(keys, appliedKeys, unknownKeys);
        }

        /// <summary>
        /// Saves a full key-color array as a human-editable JSON profile.
        /// </summary>
        public static void SaveJson(string fileName, KeyColor[] keys)
        {
            if (keys == null || keys.Length != KeyboardKeyCount)
            {
                throw new ArgumentException(Localization.Text("ProfileSlotCount"), "keys");
            }

            using (StreamWriter writer = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                writer.WriteLine("{");
                for (int i = 0; i < KeyMap.VisibleKeys.Count; i++)
                {
                    KeyMapEntry entry = KeyMap.VisibleKeys[i];
                    KeyColor color = keys[entry.Index];
                    string comma = i + 1 == KeyMap.VisibleKeys.Count ? string.Empty : ",";
                    writer.WriteLine(
                        "\t\"{0}\": [{1}, {2}, {3}, {4}]{5}",
                        EscapeJson(entry.Name),
                        color.Red,
                        color.Green,
                        color.Blue,
                        color.Alpha,
                        comma);
                }

                writer.WriteLine("}");
            }
        }

        /// <summary>
        /// Escapes a key name for JSON output.
        /// </summary>
        private static string EscapeJson(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        /// <summary>
        /// Converts one JSON value array into an AG109R key color.
        /// </summary>
        private static KeyColor ParseColor(string keyName, object rawValue)
        {
            List<object> values = ToList(rawValue);

            if (values.Count == 1)
            {
                byte brightness = ToByte(values[0], keyName);
                return new KeyColor(brightness, brightness, brightness, brightness);
            }

            if (values.Count == 3)
            {
                return KeyColor.FromRgb(
                    ToByte(values[0], keyName),
                    ToByte(values[1], keyName),
                    ToByte(values[2], keyName));
            }

            if (values.Count == 4)
            {
                return new KeyColor(
                    ToByte(values[0], keyName),
                    ToByte(values[1], keyName),
                    ToByte(values[2], keyName),
                    ToByte(values[3], keyName));
            }

            throw new FormatException(Localization.Format("JsonKeyValueCount", keyName));
        }

        /// <summary>
        /// Converts JavaScriptSerializer array output into a list.
        /// </summary>
        private static List<object> ToList(object rawValue)
        {
            if (rawValue == null || rawValue is string)
            {
                throw new FormatException(Localization.Text("JsonValueMustBeArray"));
            }

            IEnumerable enumerable = rawValue as IEnumerable;
            if (enumerable == null)
            {
                throw new FormatException(Localization.Text("JsonValueMustBeArray"));
            }

            List<object> result = new List<object>();
            foreach (object value in enumerable)
            {
                result.Add(value);
            }

            return result;
        }

        /// <summary>
        /// Converts a JSON numeric value to a protocol byte with range validation.
        /// </summary>
        private static byte ToByte(object rawValue, string keyName)
        {
            int value = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
            if (value < 0 || value > 255)
            {
                throw new FormatException(Localization.Format("JsonKeyByteRange", keyName));
            }

            return (byte)value;
        }
    }
}
