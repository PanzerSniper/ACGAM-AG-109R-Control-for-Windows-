using System.Collections.Generic;

namespace Ac109RDriverWin.Profiles
{
    /// <summary>
    /// Result returned after parsing a JSON lighting profile.
    /// </summary>
    internal sealed class ProfileData
    {
        /// <summary>
        /// Creates a profile parsing result.
        /// </summary>
        public ProfileData(KeyColor[] keys, int appliedKeys, IList<string> unknownKeys)
        {
            Keys = keys;
            AppliedKeys = appliedKeys;
            UnknownKeys = unknownKeys;
        }

        public KeyColor[] Keys { get; private set; }

        public int AppliedKeys { get; private set; }

        public IList<string> UnknownKeys { get; private set; }
    }
}
