﻿using System;

namespace Engine.Exceptions
{
    public class FLParseError : EngineException
    {
        public FLParseError(string varname, Exception inner) : base("Can not resolve symbol: " + varname, inner)
        {
        }

        public FLParseError(string varname) : this(varname, null)
        {
        }
    }
}