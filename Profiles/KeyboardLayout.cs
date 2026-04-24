using System.Collections.Generic;

namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Describes one visible key position in the preview and gradient coordinate system.
    /// </summary>
    internal sealed class KeyboardKeySlot
    {
        /// <summary>
        /// Creates a key slot with an AG109R firmware index and layout coordinates.
        /// </summary>
        public KeyboardKeySlot(int index, string name, float x, float y, float width)
        {
            Index = index;
            Name = name;
            X = x;
            Y = y;
            Width = width;
        }

        public int Index { get; private set; }

        public string Name { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Width { get; private set; }
    }

    /// <summary>
    /// Provides a compact Spanish-layout key map used for previews and gradient generation.
    /// </summary>
    internal static class KeyboardLayout
    {
        private static readonly List<KeyboardKeySlot> Slots = CreateSlots();

        public static IList<KeyboardKeySlot> Keys
        {
            get { return Slots; }
        }

        /// <summary>
        /// Builds the visible key layout in keyboard rows.
        /// </summary>
        private static List<KeyboardKeySlot> CreateSlots()
        {
            List<KeyboardKeySlot> keys = new List<KeyboardKeySlot>();

            AddRow(keys, 0, 0f, new object[]
            {
                "esc", 1f, "", 0.6f, "f1", 1f, "f2", 1f, "f3", 1f, "f4", 1f, "", 0.45f,
                "f5", 1f, "f6", 1f, "f7", 1f, "f8", 1f, "", 0.45f,
                "f9", 1f, "f10", 1f, "f11", 1f, "f12", 1f, "", 0.35f,
                "imprpant", 1f, "bloqdespl", 1f, "pausainter", 1f
            });

            AddRow(keys, 1, 0f, new object[]
            {
                "\u00ba", 1f, "1", 1f, "2", 1f, "3", 1f, "4", 1f, "5", 1f, "6", 1f,
                "7", 1f, "8", 1f, "9", 1f, "0", 1f, "'", 1f, "\u00a1", 1f, "back", 2f,
                "", 0.35f, "ins", 1f, "inicio", 1f, "repag", 1f,
                "", 0.35f, "bloqnum", 1f, "/", 1f, "*", 1f, "-r", 1f
            });

            AddRow(keys, 2, 0f, new object[]
            {
                "tab", 1.5f, "q", 1f, "w", 1f, "e", 1f, "r", 1f, "t", 1f, "y", 1f,
                "u", 1f, "i", 1f, "o", 1f, "p", 1f, "[", 1f, "+", 1f, "}", 1.5f,
                "", 0.35f, "supr", 1f, "fin", 1f, "avpag", 1f,
                "", 0.35f, "7r", 1f, "8r", 1f, "9r", 1f, "+r", 1f
            });

            AddRow(keys, 3, 0f, new object[]
            {
                "bloqmayus", 1.75f, "a", 1f, "s", 1f, "d", 1f, "f", 1f, "g", 1f,
                "h", 1f, "j", 1f, "k", 1f, "l", 1f, "\u00f1", 1f, "{", 1f, "enter", 2.25f,
                "", 3.4f, "4r", 1f, "5r", 1f, "6r", 1f
            });

            AddRow(keys, 4, 0f, new object[]
            {
                "shiftleft", 1.25f, "<", 1f, "z", 1f, "x", 1f, "c", 1f, "v", 1f, "b", 1f,
                "n", 1f, "m", 1f, ",", 1f, ".", 1f, "-", 1f, "shiftright", 2.75f,
                "", 1.35f, "up", 1f, "", 1.35f, "1r", 1f, "2r", 1f, "3r", 1f, "enterr", 1f
            });

            AddRow(keys, 5, 0f, new object[]
            {
                "ctrlleft", 1.25f, "win", 1.25f, "alt", 1.25f, "space", 6.25f, "altgr", 1.25f,
                "fn", 1.25f, "select", 1.25f, "ctrlright", 1.25f,
                "", 0.35f, "left", 1f, "down", 1f, "right", 1f,
                "", 0.35f, "0r", 2f, ".r", 1f
            });

            return keys;
        }

        /// <summary>
        /// Adds one logical keyboard row using alternating key-name and width entries.
        /// </summary>
        private static void AddRow(List<KeyboardKeySlot> keys, float row, float startX, object[] cells)
        {
            float x = startX;
            for (int i = 0; i < cells.Length; i += 2)
            {
                string name = (string)cells[i];
                float width = (float)cells[i + 1];

                if (!string.IsNullOrEmpty(name))
                {
                    int index;
                    if (KeyMap.TryGetIndex(name, out index))
                    {
                        keys.Add(new KeyboardKeySlot(index, name, x, row, width));
                    }
                }

                x += width + 0.12f;
            }
        }
    }
}
