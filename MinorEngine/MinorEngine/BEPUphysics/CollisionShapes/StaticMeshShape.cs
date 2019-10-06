using MinorEngine.BEPUphysics.DataStructures;
using MinorEngine.BEPUutilities;

namespace MinorEngine.BEPUphysics.CollisionShapes
{
    ///<summary>
    /// The local space information needed by a StaticMesh.
    /// Since the hierarchy is in world space and owned by the StaticMesh collidable,
    /// this is a pretty lightweight object.
    ///</summary>
    public class StaticMeshShape : CollisionShape
    {
        ///<summary>
        /// Gets the triangle mesh data composing the StaticMeshShape.
        ///</summary>
        public TransformableMeshData TriangleMeshData { get; }


        ///<summary>
        /// Constructs a new StaticMeshShape.
        ///</summary>
        ///<param name="vertices">Vertices of the mesh.</param>
        ///<param name="indices">Indices of the mesh.</param>
        ///<param name="worldTransform">World transform to use in the local space data.</param>
        public StaticMeshShape(Vector3[] vertices, int[] indices, AffineTransform worldTransform)
        {
            TriangleMeshData = new TransformableMeshData(vertices, indices, worldTransform);
        }


        ///<summary>
        /// Constructs a new StaticMeshShape.
        ///</summary>
        ///<param name="vertices">Vertices of the mesh.</param>
        ///<param name="indices">Indices of the mesh.</param>
        public StaticMeshShape(Vector3[] vertices, int[] indices)
        {
            TriangleMeshData = new TransformableMeshData(vertices, indices);
        }
    }
}