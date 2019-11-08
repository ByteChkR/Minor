using System.Collections.Generic;

namespace Engine.Core
{
    public static class MaskHelper
    {
        /// <summary>
        ///     Returns true if the specified flag is also set in the mask
        /// </summary>
        /// <param name="mask">the mask</param>
        /// <param name="flag">the flag</param>
        /// <param name="matchType">if false, it will return true if ANY flag is set on both sides.</param>
        /// <returns></returns>
        public static bool IsContainedInMask(int mask, int flag, bool matchType)
        {
            if (mask == 0 || flag == 0)
            {
                return false; //Anti-Wildcard
            }

            if (matchType) //If true it compares the whole mask with the whole flag(if constructed from different flags)
            {
                return (mask & flag) == flag;
            }
            else
            {
                return (mask & flag) != 0;
            }
        }
    }
}