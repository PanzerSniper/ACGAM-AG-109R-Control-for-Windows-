namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Immutable description of a built-in static lighting profile.
    /// </summary>
    internal sealed class BuiltInProfile
    {
        /// <summary>
        /// Creates a built-in profile from a generated key-color array.
        /// </summary>
        public BuiltInProfile(string name, string description, KeyColor[] keys)
        {
            Name = name;
            Description = description;
            Keys = keys;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public KeyColor[] Keys { get; private set; }

        /// <summary>
        /// Returns the display name for combo boxes.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}
