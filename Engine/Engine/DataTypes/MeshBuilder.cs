using System.Collections.Generic;
using Engine.IO;
using OpenTK;

namespace Engine.DataTypes
{
    public class MeshBuilder
    {
        private List<Vertex> Vertices = new List<Vertex>();
        private List<uint> indices=new List<uint>();


        public void AddVertex(Vertex v)
        {
            Vertices.Add(v);
            indices.Add((uint)indices.Count);
        }

        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            AddVertex(new Vertex() { Position = v1 });
            AddVertex(new Vertex() { Position = v2 });
            AddVertex(new Vertex() { Position = v3 });
        }


        public Mesh ToMesh()
        {
            MeshLoader.setupMesh(indices.ToArray(), Vertices.ToArray(), out int vao, out int vbo, out int ebo);
            return new Mesh(ebo, vbo,vao, indices.Count);
        }
    }
}