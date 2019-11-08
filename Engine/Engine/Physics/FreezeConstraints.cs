using System;

namespace Engine.Physics
{
    /// <summary>
    /// Enum that is used to define constraints along the xyz axes
    /// </summary>
    [Flags]
    public enum FreezeConstraints
    {
        NONE = 0,
        X = 1,
        Y = 2,
        Z = 4
    }
}