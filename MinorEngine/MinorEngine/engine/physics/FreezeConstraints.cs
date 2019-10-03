using System;

namespace MinorEngine.engine.physics
{
    [Flags]
    public enum FreezeConstraints
    {
        NONE = 0,
        X = 1,
        Y = 2,
        Z = 4
    }
}