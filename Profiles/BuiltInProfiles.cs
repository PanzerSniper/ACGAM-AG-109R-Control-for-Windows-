using System;
using System.Collections.Generic;

namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Generates the bundled read-only static lighting presets.
    /// </summary>
    internal static class BuiltInProfiles
    {
        private static readonly IList<BuiltInProfile> Profiles = CreateProfiles();

        public static IList<BuiltInProfile> All
        {
            get { return Profiles; }
        }

        /// <summary>
        /// Creates all static presets offered by the application.
        /// </summary>
        private static IList<BuiltInProfile> CreateProfiles()
        {
            List<BuiltInProfile> profiles = new List<BuiltInProfile>();

            profiles.Add(new BuiltInProfile(
                "Rainbow horizontal",
                "Multicolor gradient from left to right.",
                CreateByPosition(delegate(float x, float y)
                {
                    return FromHue(360f * x, 1f, 1f);
                })));

            profiles.Add(new BuiltInProfile(
                "Rainbow diagonal",
                "Multicolor diagonal gradient.",
                CreateByPosition(delegate(float x, float y)
                {
                    return FromHue(360f * ((x + y) % 1f), 1f, 1f);
                })));

            profiles.Add(new BuiltInProfile(
                "Pink/blue aurora",
                "Soft pink, purple, and blue gradient.",
                CreateByPosition(delegate(float x, float y)
                {
                    return BlendMany(x, new KeyColor[]
                    {
                        new KeyColor(255, 42, 128, 255),
                        new KeyColor(108, 82, 255, 255),
                        new KeyColor(0, 212, 255, 255)
                    });
                })));

            profiles.Add(new BuiltInProfile(
                "Ocean",
                "Deep blue to bright cyan.",
                CreateByPosition(delegate(float x, float y)
                {
                    return BlendMany((x * 0.8f) + (y * 0.2f), new KeyColor[]
                    {
                        new KeyColor(0, 24, 90, 255),
                        new KeyColor(0, 128, 255, 255),
                        new KeyColor(120, 255, 240, 255)
                    });
                })));

            profiles.Add(new BuiltInProfile(
                "Fire",
                "Red, orange, and yellow fire gradient.",
                CreateByPosition(delegate(float x, float y)
                {
                    return BlendMany((x * 0.6f) + ((1f - y) * 0.4f), new KeyColor[]
                    {
                        new KeyColor(120, 0, 0, 255),
                        new KeyColor(255, 52, 0, 255),
                        new KeyColor(255, 210, 40, 255)
                    });
                })));

            profiles.Add(new BuiltInProfile(
                "Warm white",
                "Slightly warm white lighting.",
                ProfileParser.CreateFilled(255, 214, 170)));

            profiles.Add(new BuiltInProfile(
                "Rose AC109",
                "Color close to the template provided by the Linux repository.",
                ProfileParser.CreateFilled(255, 0, 255)));

            return profiles.AsReadOnly();
        }

        /// <summary>
        /// Generates a 131-slot profile by evaluating each known key position.
        /// </summary>
        private static KeyColor[] CreateByPosition(Func<float, float, KeyColor> generator)
        {
            KeyColor[] keys = new KeyColor[ProfileParser.KeyboardKeyCount];
            for (int i = 0; i < keys.Length; i++)
            {
                float x = keys.Length == 1 ? 0f : (float)i / (keys.Length - 1);
                keys[i] = generator(x, 0.5f);
            }

            float maxX = 1f;
            float maxY = 1f;
            foreach (KeyboardKeySlot key in KeyboardLayout.Keys)
            {
                maxX = Math.Max(maxX, key.X + key.Width);
                maxY = Math.Max(maxY, key.Y);
            }

            foreach (KeyboardKeySlot key in KeyboardLayout.Keys)
            {
                float centerX = (key.X + (key.Width / 2f)) / maxX;
                float centerY = maxY <= 0 ? 0f : key.Y / maxY;
                keys[key.Index] = generator(centerX, centerY);
            }

            return keys;
        }

        /// <summary>
        /// Blends across a multi-stop color ramp.
        /// </summary>
        private static KeyColor BlendMany(float amount, KeyColor[] colors)
        {
            if (amount <= 0f)
            {
                return colors[0];
            }

            if (amount >= 1f)
            {
                return colors[colors.Length - 1];
            }

            float scaled = amount * (colors.Length - 1);
            int left = (int)Math.Floor(scaled);
            int right = Math.Min(left + 1, colors.Length - 1);
            float local = scaled - left;
            return Blend(colors[left], colors[right], local);
        }

        /// <summary>
        /// Blends two colors by a normalized amount.
        /// </summary>
        private static KeyColor Blend(KeyColor left, KeyColor right, float amount)
        {
            return new KeyColor(
                Lerp(left.Red, right.Red, amount),
                Lerp(left.Green, right.Green, amount),
                Lerp(left.Blue, right.Blue, amount),
                0xff);
        }

        /// <summary>
        /// Interpolates one byte channel.
        /// </summary>
        private static byte Lerp(byte left, byte right, float amount)
        {
            return (byte)Math.Round(left + ((right - left) * amount));
        }

        /// <summary>
        /// Converts an HSV hue to an RGB key color.
        /// </summary>
        private static KeyColor FromHue(float hue, float saturation, float value)
        {
            hue = hue % 360f;
            if (hue < 0f)
            {
                hue += 360f;
            }

            float c = value * saturation;
            float x = c * (1f - Math.Abs(((hue / 60f) % 2f) - 1f));
            float m = value - c;
            float r;
            float g;
            float b;

            if (hue < 60f)
            {
                r = c; g = x; b = 0f;
            }
            else if (hue < 120f)
            {
                r = x; g = c; b = 0f;
            }
            else if (hue < 180f)
            {
                r = 0f; g = c; b = x;
            }
            else if (hue < 240f)
            {
                r = 0f; g = x; b = c;
            }
            else if (hue < 300f)
            {
                r = x; g = 0f; b = c;
            }
            else
            {
                r = c; g = 0f; b = x;
            }

            return new KeyColor(
                ToByte((r + m) * 255f),
                ToByte((g + m) * 255f),
                ToByte((b + m) * 255f),
                0xff);
        }

        /// <summary>
        /// Clamps a floating-point channel to the byte range.
        /// </summary>
        private static byte ToByte(float value)
        {
            if (value < 0f)
            {
                return 0;
            }

            if (value > 255f)
            {
                return 255;
            }

            return (byte)Math.Round(value);
        }
    }
}
