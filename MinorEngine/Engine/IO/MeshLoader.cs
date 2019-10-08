using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Configs;
using Engine.DataTypes;
using Engine.Debug;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Mesh = Engine.DataTypes.Mesh;
using AssimpMesh = Assimp.Mesh;
using AssimpTextureType = Assimp.TextureType;
using TextureType = Engine.DataTypes.TextureType;
namespace Engine.IO
{
    public class MeshLoader
    {
        //Todo: Create Dummy texture that will be returned when not found
        public static Mesh FileToMesh(string filename)
        {
            var meshes = LoadModel(filename);
            if (meshes.Count > 0)
            {
                return meshes[0];
            }

            return null;
        }

        public static Mesh[] FileToMeshes(string filename)
        {
            return LoadModel(filename).ToArray();
        }

        private static List<Mesh> LoadModel(string path)
        {
            var context = new AssimpContext();
            context.SetConfig(new NormalSmoothingAngleConfig(66));
            var s = context.ImportFile(path);

            if (s == null || (s.SceneFlags & SceneFlags.Incomplete) != 0 || s.RootNode == null)
            {
                Logger.Log("Loading Model File failed.", DebugChannel.Error);
                return new List<Mesh>();
            }

            var directory = Path.GetDirectoryName(path);
            if (directory == string.Empty)
            {
                directory = ".";
            }


            Logger.Log("Loading Model Finished.", DebugChannel.Log);

            Logger.Log("Processing Nodes...", DebugChannel.Log);

            var ret = new List<Mesh>();

            processNode(s.RootNode, s, ret, directory, path);
            return ret;
        }

        private static void processNode(Node node, Scene s, List<Mesh> meshes, string dir, string DebugName)
        {
            Logger.Log("Processing Node: " + node.Name, DebugChannel.Log);
            if (node.HasMeshes)
            {
                Logger.Log("Adding " + node.MeshCount + " Meshes...", DebugChannel.Log);
                for (var i = 0; i < node.MeshCount; i++)
                {
                    meshes.Add(processMesh(s.Meshes[node.MeshIndices[i]], s, dir, DebugName));
                }
            }

            if (node.HasChildren)
            {
                for (var i = 0; i < node.Children.Count; i++)
                {
                    processNode(node.Children[i], s, meshes, dir, DebugName);
                }
            }
        }

        private static Mesh processMesh(AssimpMesh mesh, Scene s, string dir, string debugName)
        {
            var vertices = new List<Vertex>();
            var indices = new List<uint>();
            var textures = new List<Texture>();


            Logger.Log("Converting Imported Mesh File Structure to GameEngine Engine Structure", DebugChannel.Log);


            Logger.Log("Copying Vertex Data...", DebugChannel.Log);
            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var vert = mesh.Vertices[i];
                var norm = mesh.Normals[i];
                var tan = mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D(0);
                var bit = mesh.HasTangentBasis ? mesh.BiTangents[i] : new Vector3D(0);
                var uv = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0);

                var v = new Vertex
                {
                    Position = new Vector3(vert.X, vert.Y, vert.Z),
                    Normal = new Vector3(norm.X, norm.Y, norm.Z),
                    UV = new Vector2(uv.X, uv.Y),
                    Bittangent = new Vector3(bit.X, bit.Y, bit.Z),
                    Tangent = new Vector3(tan.X, tan.Y, tan.Z)
                };

                vertices.Add(v);
            }


            Logger.Log("Calculating Indices...", DebugChannel.Log);

            for (var i = 0; i < mesh.FaceCount; i++)
            {
                var f = mesh.Faces[i];
                indices.AddRange(f.Indices.Select(x => (uint)x));
            }


            var m = s.Materials[mesh.MaterialIndex];

            Logger.Log("Loading Baked Material: " + m.Name, DebugChannel.Log);

            textures.AddRange(loadMaterialTextures(m, TextureType.Diffuse, dir));
            textures.AddRange(loadMaterialTextures(m, TextureType.Specular, dir));
            textures.AddRange(loadMaterialTextures(m, TextureType.Normals, dir));
            textures.AddRange(loadMaterialTextures(m, TextureType.Height, dir));





            setupMesh(indices.ToArray(), vertices.ToArray(), out int vao, out int vbo, out int ebo);



            return new Mesh(ebo, vbo, vao, indices.Count);
        }


        private static IntPtr offsetOf(string name)
        {
            var off = Marshal.OffsetOf(typeof(Vertex), name);
            return off;
        }

        private static void setupMesh(uint[] indices, Vertex[] vertices, out int vao,out int vbo, out int ebo)
        {
            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);
            GL.GenBuffers(1, out ebo);

            //VAO
            GL.BindVertexArray(vao);

            //VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.VERTEX_BYTE_SIZE),
                vertices, BufferUsageHint.StaticDraw);

            //EBO

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices,
                BufferUsageHint.StaticDraw);

            //Attribute Pointers
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_SIZE,
                IntPtr.Zero);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_SIZE,
                offsetOf("Normal"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_SIZE,
                offsetOf("UV"));

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_SIZE,
                offsetOf("Tangent"));

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vertex.VERTEX_BYTE_SIZE,
                offsetOf("Bittangent"));


            GL.BindVertexArray(0);
        }


        private static List<Texture> loadMaterialTextures(Material m, TextureType texType, string dir)
        {
            var ret = new List<Texture>();

            Logger.Log("Loading Baked Material Textures of type: " + Enum.GetName(typeof(TextureType), texType),
                DebugChannel.Log);
            for (var i = 0; i < m.GetMaterialTextureCount((AssimpTextureType)texType); i++)
            {
                TextureSlot s;
                m.GetMaterialTexture((AssimpTextureType)texType, i, out s);
                var tx = TextureLoader.FileToTexture(dir + s.FilePath);
                tx.TexType = texType;
                ret.Add(tx);
            }

            return ret;
        }
    }
}