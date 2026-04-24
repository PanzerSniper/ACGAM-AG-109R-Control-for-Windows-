namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Represents one AC109R key color in firmware byte order.
    /// </summary>
    internal struct KeyColor
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;

        /// <summary>
        /// Creates a key color with explicit red, green, blue, and alpha values.
        /// </summary>
        public KeyColor(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Creates an opaque key color from RGB channels.
        /// </summary>
        public static KeyColor FromRgb(byte red, byte green, byte blue)
        {
            return new KeyColor(red, green, blue, 0xff);
        }
    }
}
