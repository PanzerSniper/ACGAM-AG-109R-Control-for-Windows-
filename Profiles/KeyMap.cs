using System.Collections.Generic;

namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Describes one JSON key name and its corresponding firmware index.
    /// </summary>
    internal sealed class KeyMapEntry
    {
        /// <summary>
        /// Creates a key-map entry.
        /// </summary>
        public KeyMapEntry(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public int Index { get; private set; }

        public string Name { get; private set; }
    }

    /// <summary>
    /// Maps profile JSON key names to AG109R firmware key indexes.
    /// </summary>
    internal static class KeyMap
    {
        private static readonly Dictionary<string, int> Names = CreateNames();
        private static readonly IList<KeyMapEntry> Entries = CreateEntries();

        public static IList<KeyMapEntry> VisibleKeys
        {
            get { return Entries; }
        }

        /// <summary>
        /// Resolves a JSON key name to a firmware key index.
        /// </summary>
        public static bool TryGetIndex(string name, out int index)
        {
            return Names.TryGetValue(name, out index);
        }

        /// <summary>
        /// Builds a lookup table including compatibility aliases from the original Linux files.
        /// </summary>
        private static Dictionary<string, int> CreateNames()
        {
            Dictionary<string, int> names = new Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);

            Add(names, 0, "esc");
            Add(names, 2, "f1");
            Add(names, 3, "f2");
            Add(names, 4, "f3");
            Add(names, 5, "f4");
            Add(names, 7, "f5");
            Add(names, 8, "f6");
            Add(names, 9, "f7");
            Add(names, 10, "f8");
            Add(names, 11, "f9");
            Add(names, 12, "f10");
            Add(names, 13, "f11");
            Add(names, 14, "f12");
            Add(names, 15, "imprpant");
            Add(names, 16, "bloqdespl");
            Add(names, 17, "pausainter");
            Add(names, 22, "\u00ba", "\u00c2\u00ba");
            Add(names, 23, "1");
            Add(names, 24, "2");
            Add(names, 25, "3");
            Add(names, 26, "4");
            Add(names, 27, "5");
            Add(names, 28, "6");
            Add(names, 29, "7");
            Add(names, 30, "8");
            Add(names, 31, "9");
            Add(names, 32, "0");
            Add(names, 33, "'");
            Add(names, 34, "\u00a1", "\u00c2\u00a1");
            Add(names, 36, "back");
            Add(names, 37, "ins");
            Add(names, 38, "inicio");
            Add(names, 39, "repag");
            Add(names, 40, "bloqnum");
            Add(names, 41, "/");
            Add(names, 42, "*");
            Add(names, 43, "-r");
            Add(names, 44, "tab");
            Add(names, 45, "q");
            Add(names, 46, "w");
            Add(names, 47, "e");
            Add(names, 48, "r");
            Add(names, 49, "t");
            Add(names, 50, "y");
            Add(names, 51, "u");
            Add(names, 52, "i");
            Add(names, 53, "o");
            Add(names, 54, "p");
            Add(names, 55, "[");
            Add(names, 56, "+");
            Add(names, 58, "}");
            Add(names, 59, "supr");
            Add(names, 60, "fin");
            Add(names, 61, "avpag");
            Add(names, 62, "7r");
            Add(names, 63, "8r");
            Add(names, 64, "9r");
            Add(names, 65, "+r");
            Add(names, 66, "bloqmayus");
            Add(names, 67, "a");
            Add(names, 68, "s");
            Add(names, 69, "d");
            Add(names, 70, "f");
            Add(names, 71, "g");
            Add(names, 72, "h");
            Add(names, 73, "j");
            Add(names, 74, "k");
            Add(names, 75, "l");
            Add(names, 76, "\u00f1", "\u00c3\u00b1");
            Add(names, 77, "{");
            Add(names, 79, "enter");
            Add(names, 84, "4r");
            Add(names, 85, "5r");
            Add(names, 86, "6r");
            Add(names, 88, "shiftleft");
            Add(names, 89, "<");
            Add(names, 90, "z");
            Add(names, 91, "x");
            Add(names, 92, "c");
            Add(names, 93, "v");
            Add(names, 94, "b");
            Add(names, 95, "n");
            Add(names, 96, "m");
            Add(names, 97, ",");
            Add(names, 98, ".");
            Add(names, 99, "-");
            Add(names, 102, "shiftright");
            Add(names, 104, "up");
            Add(names, 106, "1r");
            Add(names, 107, "2r");
            Add(names, 108, "3r");
            Add(names, 109, "enterr");
            Add(names, 110, "ctrlleft");
            Add(names, 111, "win");
            Add(names, 112, "alt");
            Add(names, 116, "space");
            Add(names, 120, "altgr");
            Add(names, 121, "fn");
            Add(names, 122, "select");
            Add(names, 124, "ctrlright");
            Add(names, 125, "left");
            Add(names, 126, "down");
            Add(names, 127, "right");
            Add(names, 128, "0r");
            Add(names, 130, ".r");

            return names;
        }

        /// <summary>
        /// Builds the canonical ordered key list used when exporting JSON profiles.
        /// </summary>
        private static IList<KeyMapEntry> CreateEntries()
        {
            List<KeyMapEntry> entries = new List<KeyMapEntry>();

            Add(entries, 0, "esc");
            Add(entries, 2, "f1");
            Add(entries, 3, "f2");
            Add(entries, 4, "f3");
            Add(entries, 5, "f4");
            Add(entries, 7, "f5");
            Add(entries, 8, "f6");
            Add(entries, 9, "f7");
            Add(entries, 10, "f8");
            Add(entries, 11, "f9");
            Add(entries, 12, "f10");
            Add(entries, 13, "f11");
            Add(entries, 14, "f12");
            Add(entries, 15, "imprpant");
            Add(entries, 16, "bloqdespl");
            Add(entries, 17, "pausainter");
            Add(entries, 22, "\u00ba");
            Add(entries, 23, "1");
            Add(entries, 24, "2");
            Add(entries, 25, "3");
            Add(entries, 26, "4");
            Add(entries, 27, "5");
            Add(entries, 28, "6");
            Add(entries, 29, "7");
            Add(entries, 30, "8");
            Add(entries, 31, "9");
            Add(entries, 32, "0");
            Add(entries, 33, "'");
            Add(entries, 34, "\u00a1");
            Add(entries, 36, "back");
            Add(entries, 37, "ins");
            Add(entries, 38, "inicio");
            Add(entries, 39, "repag");
            Add(entries, 40, "bloqnum");
            Add(entries, 41, "/");
            Add(entries, 42, "*");
            Add(entries, 43, "-r");
            Add(entries, 44, "tab");
            Add(entries, 45, "q");
            Add(entries, 46, "w");
            Add(entries, 47, "e");
            Add(entries, 48, "r");
            Add(entries, 49, "t");
            Add(entries, 50, "y");
            Add(entries, 51, "u");
            Add(entries, 52, "i");
            Add(entries, 53, "o");
            Add(entries, 54, "p");
            Add(entries, 55, "[");
            Add(entries, 56, "+");
            Add(entries, 58, "}");
            Add(entries, 59, "supr");
            Add(entries, 60, "fin");
            Add(entries, 61, "avpag");
            Add(entries, 62, "7r");
            Add(entries, 63, "8r");
            Add(entries, 64, "9r");
            Add(entries, 65, "+r");
            Add(entries, 66, "bloqmayus");
            Add(entries, 67, "a");
            Add(entries, 68, "s");
            Add(entries, 69, "d");
            Add(entries, 70, "f");
            Add(entries, 71, "g");
            Add(entries, 72, "h");
            Add(entries, 73, "j");
            Add(entries, 74, "k");
            Add(entries, 75, "l");
            Add(entries, 76, "\u00f1");
            Add(entries, 77, "{");
            Add(entries, 79, "enter");
            Add(entries, 84, "4r");
            Add(entries, 85, "5r");
            Add(entries, 86, "6r");
            Add(entries, 88, "shiftleft");
            Add(entries, 89, "<");
            Add(entries, 90, "z");
            Add(entries, 91, "x");
            Add(entries, 92, "c");
            Add(entries, 93, "v");
            Add(entries, 94, "b");
            Add(entries, 95, "n");
            Add(entries, 96, "m");
            Add(entries, 97, ",");
            Add(entries, 98, ".");
            Add(entries, 99, "-");
            Add(entries, 102, "shiftright");
            Add(entries, 104, "up");
            Add(entries, 106, "1r");
            Add(entries, 107, "2r");
            Add(entries, 108, "3r");
            Add(entries, 109, "enterr");
            Add(entries, 110, "ctrlleft");
            Add(entries, 111, "win");
            Add(entries, 112, "alt");
            Add(entries, 116, "space");
            Add(entries, 120, "altgr");
            Add(entries, 121, "fn");
            Add(entries, 122, "select");
            Add(entries, 124, "ctrlright");
            Add(entries, 125, "left");
            Add(entries, 126, "down");
            Add(entries, 127, "right");
            Add(entries, 128, "0r");
            Add(entries, 130, ".r");

            return entries.AsReadOnly();
        }

        /// <summary>
        /// Adds one or more aliases to the lookup dictionary.
        /// </summary>
        private static void Add(Dictionary<string, int> names, int index, params string[] aliases)
        {
            foreach (string alias in aliases)
            {
                if (!names.ContainsKey(alias))
                {
                    names.Add(alias, index);
                }
            }
        }

        /// <summary>
        /// Adds one canonical JSON export entry.
        /// </summary>
        private static void Add(List<KeyMapEntry> entries, int index, string name)
        {
            entries.Add(new KeyMapEntry(index, name));
        }
    }
}
