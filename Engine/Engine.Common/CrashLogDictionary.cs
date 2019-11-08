using System;
using System.Collections.Generic;
using ADL.Configs;

namespace Engine.Common
{
    /// <summary>
    /// Internal Class to Log Crashes in a Dictionary
    /// </summary>
    internal class CrashLogDictionary : SerializableDictionary<string, ApplicationException>
    {
        public CrashLogDictionary() : this(new Dictionary<string, ApplicationException>())
        {
        }

        public CrashLogDictionary(Dictionary<string, ApplicationException> dict) : base(dict)
        {
        }
    }
}