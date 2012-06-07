using System;

namespace Lapins.Data.Localization
{
    /// <summary>
    /// Languages management. Use the standard .NET API.
    /// </summary>
    public static class LocalizedStrings
    {
        /// <summary>
        /// Get a string
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static String GetString(String stringID)
        {
            String s = LapinsStrings.ResourceManager.GetString(stringID, LapinsStrings.Culture);
            return s ?? stringID;
        }
    }
}
