using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Configs;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Mesh = Engine.DataTypes.Mesh;
using AssimpMesh = Assimp.Mesh;
using AssimpTextureType = Assimp.TextureType;
using Bitmap = System.Drawing.Bitmap;
using TextureType = Engine.DataTypes.TextureType;

namespace Engine.IO
{
    /// <summary>
    /// Static class responsible to load Meshes from Disk
    /// </summary>
    public class MeshLoader
    {
        /// <summary>
        /// Loads a Mesh From File
        /// </summary>
        /// <param name="filename">The path to the file</param>
        /// <returns>The loaded Mesh</returns>
        public static Mesh FileToMesh(string filename)
        {
            List<Mesh> meshes = LoadModel(filename);
            if (meshes.Count > 0)
            {
                return meshes[0];
            }

            return null;
        }

        /// <summary>
        /// Loads Meshes From File
        /// </summary>
        /// <param name="filename">The path to the file</param>
        /// <returns>The loaded Meshes</returns>
        public static Mesh[] FileToMeshes(string filename)
        {
            return LoadModel(filename).ToArray();
        }


        /// <summary>
        /// Loads a model with assimp
        /// </summary>
        /// <param name="stream">the input stream for assimp</param>
        /// <returns></returns>
        internal static List<Mesh> LoadModel(Stream stream, string hint = "", string path = "")
        {
            AssimpContext context = new AssimpContext();
            context.SetConfig(new NormalSmoothingAngleConfig(66));
            return LoadAssimpScene(context.ImportFileFromStream(stream, hint), path);
        }

        /// <summary>
        /// Loads a Assimp Model From File
        /// </summary>
        /// <param name="filename">The path to the file</param>
        /// <returns>The loaded AssimpModel</returns>
        private static List<Mesh> LoadModel(string path)
        {
            if (!IOManager.Exists(path))
            {
                Logger.Crash(new InvalidFolderPathException(path), true);
                return new List<Mesh> {Mesh.DefaultMesh};
            }

            return LoadModel(IOManager.GetStream(path), Path.GetExtension(path), path);
        }

        /// <summary>
        /// Converts an assimp scene to a list of game meshes
        /// </summary>
        /// <param name="s">The scene</param>
        /// <param name="path">Path to the object that was loaded by assimp</param>
        /// <returns></returns>
        private static List<Mesh> LoadAssimpScene(Scene s, string path)
        {
            if (s == null || (s.SceneFlags & SceneFlags.Incomplete) != 0 || s.RootNode == null)
            {
                Logger.Crash(new InvalidFolderPathException(path), true);
                return new List<Mesh>();
            }

            string directory = Path.GetDirectoryName(path);
            if (directory == string.Empty)
            {
                directory = ".";
            }


            Logger.Log("Loading Assimp Scene Finished.", DebugChannel.Log | DebugChannel.IO, 5);

            Logger.Log("Processing Nodes...", DebugChannel.Log | DebugChannel.IO, 6);

            List<Mesh> ret = new List<Mesh>();

            processNode(s.RootNode, s, ret, directory);
            return ret;
        }


        /// <summary>
        /// Processes a node in an Assimp Scene
        /// </summary>
        /// <param name="node">The Current node to process</param>
        /// <param name="s">The root scene</param>
        /// <param name="meshes">The mesh list that will be filled</param>
        /// <param name="dir">The Relative directory of the Mesh File</param>
        private static void processNode(Node node, Scene s, List<Mesh> meshes, string dir)
        {
            Logger.Log("Processing Node: " + node.Name, DebugChannel.Log | DebugChannel.IO, 4);
            if (node.HasMeshes)
            {
                Logger.Log("Adding " + node.MeshCount + " Meshes...", DebugChannel.Log | DebugChannel.IO, 4);
                for (int i = 0; i < node.MeshCount; i++)
                {
                    meshes.Add(processMesh(s.Meshes[node.MeshIndices[i]], s, dir));
                }
            }

            if (node.HasChildren)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    processNode(node.Children[i], s, meshes, dir);
                }
            }
        }


        /// <summary>
        /// Processes a Mesh in an Assimp Scene
        /// </summary>
        /// <param name="mesh">The Current mesh to process</param>
        /// <param name="s">The root scene</param>
        /// <param name="dir">The Relative directory of the Mesh File</param>
        /// <returns>The mesh loaded.</returns>
        private static Mesh processMesh(AssimpMesh mesh, Scene s, string dir)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();


            Logger.Log("Converting Imported Mesh File Structure to GameEngine Engine Structure",
                DebugChannel.Log | DebugChannel.IO, 3);


            Logger.Log("Copying Vertex Data...", DebugChannel.Log | DebugChannel.IO, 2);
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vector3D vert = mesh.Vertices[i];
                Vector3D norm = mesh.Normals[i];
                Vector3D tan = mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D(0);
                Vector3D bit = mesh.HasTangentBasis ? mesh.BiTangents[i] : new Vector3D(0);
                Vector3D uv = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0);

                Vertex v = new Vertex
                {
                    Position = new Vector3(vert.X, vert.Y, vert.Z),
                    Normal = new Vector3(norm.X, norm.Y, norm.Z),
                    UV = new Vector2(uv.X, uv.Y),
                    Bittangent = new Vector3(bit.X, bit.Y, bit.Z),
                    Tangent = new Vector3(tan.X, tan.Y, tan.Z)
                };

                vertices.Add(v);
            }


            Logger.Log("Calculating Indices...", DebugChannel.Log | DebugChannel.IO, 2);

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face f = mesh.Faces[i];
                indices.AddRange(f.Indices.Select(x => (uint) x));
            }


            Material m = s.Materials[mesh.MaterialIndex];

            Logger.Log("Loading Baked Material: " + m.Name, DebugChannel.Log | DebugChannel.IO, 2);

            textures.AddRange(TextureLoader.LoadMaterialTextures(m, TextureType.Diffuse, dir));
            textures.AddRange(TextureLoader.LoadMaterialTextures(m, TextureType.Specular, dir));
            textures.AddRange(TextureLoader.LoadMaterialTextures(m, TextureType.Normals, dir));
            textures.AddRange(TextureLoader.LoadMaterialTextures(m, TextureType.Height, dir));

            setupMesh(indices.ToArray(), vertices.ToArray(), out int vao, out int vbo, out int ebo);


            return new Mesh(ebo, vbo, vao, indices.Count);
        }

        /// <summary>
        /// Little helper function that is the equivalent for C++ : offsetof keyword
        /// </summary>
        /// <param name="name">Name of the Field</param>
        /// <returns>The offset relative to the beginning of vertex the struct</returns>
        private static IntPtr offsetOf(string name)
        {
            IntPtr off = Marshal.OffsetOf(typeof(Vertex), name);
            return off;
        }

        /// <summary>
        /// Code that is setting up the mesh in OpenGL
        /// </summary>
        /// <param name="indices">Indices to Draw</param>
        /// <param name="vertices">Vertex Data</param>
        /// <param name="vao">vertex array object</param>
        /// <param name="vbo">vertex buffer object</param>
        /// <param name="ebo">element buffer object</param>
        private static void setupMesh(uint[] indices, Vertex[] vertices, out int vao, out int vbo, out int ebo)
        {
            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);
            GL.GenBuffers(1, out ebo);

            //VAO
            GL.BindVertexArray(vao);

            //VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (vertices.Length * Vertex.VERTEX_BYTE_SIZE),
                vertices, BufferUsageHint.StaticDraw);

            //EBO

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (indices.Length * sizeof(uint)), indices,
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
    }
}