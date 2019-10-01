using System.Collections.Generic;

namespace MinorEngine.engine.core
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
            var a = GetUniqueMasksSet(flag);
            foreach (var f in a)
            {
                if ((mask & f) == f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Splits up parameter mask into Unique Flags(power of 2 numbers)
        /// </summary>
        /// <param name="mask">the mask you want to split</param>
        /// <returns></returns>
        public static List<int> GetUniqueMasksSet(int mask)
        {
            if (IsUniqueMask(mask))
            {
                return new List<int> { mask };
            }
            var ret = new List<int>();
            for (var i = 0; i < sizeof(int) * sizeof(byte); i++)
            {
                var f = 1 << i;
                if (IsContainedInMask(mask, f, true))
                {
                    ret.Add(f);
                }
            }

            return ret;
        }

        /// <summary>
        ///     Checks if the specified mask is unique(e.g. a power of 2 number)
        /// </summary>
        /// <param name="mask">mask to test</param>
        /// <returns></returns>
        public static bool IsUniqueMask(int mask)
        {
            return mask != 0 && (mask & (mask - 1)) == 0;
        }
    }
}