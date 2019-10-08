using System;
using System.Runtime.InteropServices;
using OpenTK;

namespace Engine.DataTypes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Vertex : IEquatable<Vertex>
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public Vector3 Normal;
        [FieldOffset(24)] public Vector2 UV;
        [FieldOffset(32)] public Vector3 Tangent;
        [FieldOffset(44)] public Vector3 Bittangent;

        public static readonly int VERTEX_BYTE_SIZE = sizeof(float) * 14;

        public bool Equals(Vertex other)
        {
            return Vector3.Distance(Position, other.Position) < 0.001f;
        }
    }
}