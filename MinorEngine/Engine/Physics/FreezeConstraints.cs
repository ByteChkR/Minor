using System;

namespace Engine.Physics
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