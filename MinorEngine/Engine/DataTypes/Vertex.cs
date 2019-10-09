using System;
using System.Runtime.InteropServices;
using OpenTK;

namespace Engine.DataTypes
{

    /// <summary>
    /// A Data Type that is containing the information that Represents a Vertex in OpenGL
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Vertex : IEquatable<Vertex>
    {
        /// <summary>
        /// Position of Vertex
        /// </summary>
        [FieldOffset(0)] public Vector3 Position;
        /// <summary>
        /// Normal of Vertex
        /// </summary>
        [FieldOffset(12)] public Vector3 Normal;
        /// <summary>
        /// UV Coords of Vertex
        /// </summary>
        [FieldOffset(24)] public Vector2 UV;
        /// <summary>
        /// Tangent of the Normal of the Vertex
        /// </summary>
        [FieldOffset(32)] public Vector3 Tangent;
        /// <summary>
        /// Bitangent of the Normal of the Vertex
        /// </summary>
        [FieldOffset(44)] public Vector3 Bittangent;

        /// <summary>
        /// A static value that is representing the size of the object( as a woorkaround for sizeof(T) beeing unsave
        /// </summary>
        public static readonly int VERTEX_BYTE_SIZE = sizeof(float) * 14;


        /// <summary>
        /// Equals implementation that checks for the Distance to the other vertex
        /// </summary>
        /// <param name="other">The other vertex to check against</param>
        /// <returns>True if the vertices are reasonably close together to merge them into a single vertex.</returns>
        public bool Equals(Vertex other)
        {
            return Vector3.Distance(Position, other.Position) < 0.001f;
        }
    }
}