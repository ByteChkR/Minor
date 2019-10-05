using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Configs;
using MinorEngine.CLHelperLibrary;
using MinorEngine.debug;
using MinorEngine.engine.rendering;
using MinorEngine.engine.ui.utils;
using OpenCl.DotNetCore.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Bitmap = System.Drawing.Bitmap;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

namespace MinorEngine.engine.core
{



    public static class ResourceManager
    {

        #region Texture IO

        public static class TextureIO
        {


            public static GameTexture BytesToTexture(IntPtr ptr, int width, int height, string cacheName = "")
            {
                return BytesToTexture(ptr, width, height, true, cacheName);
            }

            internal static GameTexture BytesToTexture(IntPtr ptr, int width, int height, bool writeLog, string cacheName = "")
            {
                //cacheName = SanitizeName(cacheName);

                GameTexture ret = new GameTexture(cacheName);
                //AddReference(ret, cacheName); //Adding it to the cache

                if (writeLog)
                    ret.Log($"Loading Texture with Name: {cacheName}... Width: {width} Height: {height}", DebugChannel.Log);

                GL.BindTexture(TextureTarget.Texture2D, ret.TextureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra,
                    PixelType.UnsignedByte, ptr);


                DefaultTexParameter();
                GL.BindTexture(TextureTarget.Texture2D, 0);
                return ret;
            }

            public static GameTexture BytesToTexture(byte[] buffer, int width, int height, string cacheName = "")
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                GameTexture ret = BytesToTexture(handle.AddrOfPinnedObject(), width, height, cacheName);

                handle.Free();
                return ret;
            }

            internal static GameTexture ParameterToTexture(int width, int height, bool writeLog, string cacheName = "")
            {

                return BytesToTexture(IntPtr.Zero, width, height, writeLog, cacheName);
            }

            public static GameTexture ParameterToTexture(int width, int height, string cacheName = "")
            {
                return ParameterToTexture(width, height, true, cacheName);

            }

            //public static int GetRefCount(GameTexture tex)
            //{
            //    return _GameTextureRefTable[tex.TextureId];
            //}

            private static byte[] TextureToByteArray(GameTexture tex)
            {
                byte[] buffer = new byte[(int)(tex.Width * tex.Height * 4)];
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);

                GL.GetTextureSubImage(tex.TextureId, 0, 0, 0, 0, (int)tex.Width, (int)tex.Height, 1, PixelFormat.Bgra,
                    PixelType.UnsignedByte, buffer.Length, handle.AddrOfPinnedObject());

                handle.Free();
                return buffer;
            }

            public static MemoryBuffer TextureToMemoryBuffer(GameTexture tex)
            {
                byte[] buffer = TextureToByteArray(tex);
                return CL.CreateBuffer(buffer, MemoryFlag.CopyHostPointer | MemoryFlag.ReadWrite);
            }

            public static void Update(GameTexture tex, byte[] data, int width, int height)
            {
                GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra,
                    PixelType.UnsignedByte, data);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            public static void UpdateTexture(GameTexture tex, MemoryBuffer data)
            {
                Update(tex, TextureToByteArray(tex), (int)tex.Width, (int)tex.Height);
            }

            public static GameTexture BitmapToTexture(Bitmap bmp, string cacheName = "")
            {

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GameTexture tex = BytesToTexture(data.Scan0, bmp.Width, bmp.Height, cacheName);

                bmp.UnlockBits(data);
                return tex;
            }


            public static GameTexture[] AssimpSceneToTextures(Scene scene)
            {
                if (!scene.HasTextures) return new GameTexture[0];
                List<GameTexture> list = new List<GameTexture>();
                foreach (var x in scene.Textures) list.Add(AssimpEmbeddedToTexture(x));

                return list.ToArray();
            }

            private static byte[] flattenImageData(Texel[] imageData)
            {
                byte[] ret = new byte[imageData.Length * 4];
                for (int i = 0; i < imageData.Length; i++) TexelToByteSequence(i * 4, ret, imageData[i]);

                return ret;
            }

            private static void TexelToByteSequence(int startidx, byte[] arr, Texel txl)
            {
                arr[startidx] = txl.R;
                arr[startidx + 1] = txl.G;
                arr[startidx + 2] = txl.B;
                arr[startidx + 3] = txl.A;
            }

            //Maybe create key for textures loaded with assimp and then use them as cache name
            public static GameTexture AssimpEmbeddedToTexture(EmbeddedTexture tex, string cacheName = "")
            {
                return BytesToTexture(flattenImageData(tex.NonCompressedData), tex.Width, tex.Height, cacheName);
            }

            public static GameTexture FileToTexture(string file)
            {
                //if (IsContained(file)) return KeyToTexture(file);
                return BitmapToTexture(new Bitmap(file), file);
            }

            private static void DefaultTexParameter()
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            }

            private static GameTexture Copy(GameTexture other, string cacheName = "")
            {
                return BytesToTexture(TextureToByteArray(other), (int)other.Width, (int)other.Height, cacheName);
            }

            internal static void AddConsoleCommands(DebugConsoleComponent console)
            {
                console.AddCommand("ltexall", cmd_ListTextures);
                console.AddCommand("ltex", cmd_TextureInfo);
                console.AddCommand("ltex2c", cmd_LtexToConsole);
            }

        }
        #endregion


        #region MeshIO
        public static class MeshIO
        {

            //Todo: Create Dummy texture that will be returned when not found
            public static GameMesh FileToMesh(string filename)
            {
                List<GameMesh> meshes = LoadModel(filename);
                if (meshes.Count > 0) return meshes[0];
                return null;
            }

            public static GameMesh[] FileToMeshes(string filename)
            {
                return LoadModel(filename).ToArray();
            }

            private static List<GameMesh> LoadModel(string path)
            {
                AssimpContext context = new AssimpContext();
                context.SetConfig(new NormalSmoothingAngleConfig(66));
                Scene s = context.ImportFile(path);

                if (s == null || (s.SceneFlags & SceneFlags.Incomplete) != 0 || s.RootNode == null)
                {
                    s.Log("Loading Model File failed.", DebugChannel.Error);
                    return new List<GameMesh>();
                }

                string directory = Path.GetDirectoryName(path);
                if (directory == string.Empty) directory = ".";


                s.Log("Loading Model Finished.", DebugChannel.Log);

                s.Log("Processing Nodes...", DebugChannel.Log);

                List<GameMesh> ret = new List<GameMesh>();

                processNode(s.RootNode, s, ret, directory, path);
                return ret;
            }

            private static void processNode(Node node, Scene s, List<GameMesh> meshes, string dir, string DebugName)
            {
                node.Log("Processing Node: " + node.Name, DebugChannel.Log);
                if (node.HasMeshes)
                {
                    node.Log("Adding " + node.MeshCount + " Meshes...", DebugChannel.Log);
                    for (int i = 0; i < node.MeshCount; i++) meshes.Add(processMesh(s.Meshes[node.MeshIndices[i]], s, dir, DebugName));
                }

                if (node.HasChildren)
                    for (int i = 0; i < node.Children.Count; i++)
                        processNode(node.Children[i], s, meshes, dir, DebugName);
            }

            private static GameMesh processMesh(Mesh mesh, Scene s, string dir, string debugName)
            {
                List<GameVertex> vertices = new List<GameVertex>();
                List<uint> indices = new List<uint>();
                List<GameTexture> textures = new List<GameTexture>();


                mesh.Log("Converting Imported Mesh File Structure to GameEngine Engine Structure", DebugChannel.Log);


                mesh.Log("Copying Vertex Data...", DebugChannel.Log);
                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    Vector3D vert = mesh.Vertices[i];
                    Vector3D norm = mesh.Normals[i];
                    Vector3D tan = mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D(0);
                    Vector3D bit = mesh.HasTangentBasis ? mesh.BiTangents[i] : new Vector3D(0);
                    Vector3D uv = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0);

                    GameVertex v = new GameVertex
                    {
                        Position = new Vector3(vert.X, vert.Y, vert.Z),
                        Normal = new Vector3(norm.X, norm.Y, norm.Z),
                        UV = new Vector2(uv.X, uv.Y),
                        Bittangent = new Vector3(bit.X, bit.Y, bit.Z),
                        Tangent = new Vector3(tan.X, tan.Y, tan.Z)
                    };

                    vertices.Add(v);
                }


                mesh.Log("Calculating Indices...", DebugChannel.Log);

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    Face f = mesh.Faces[i];
                    indices.AddRange(f.Indices.Select(x => (uint)x));
                }


                Material m = s.Materials[mesh.MaterialIndex];

                mesh.Log("Loading Baked Material: " + m.Name, DebugChannel.Log);

                textures.AddRange(loadMaterialTextures(m, TextureType.Diffuse, dir));
                textures.AddRange(loadMaterialTextures(m, TextureType.Specular, dir));
                textures.AddRange(loadMaterialTextures(m, TextureType.Normals, dir));
                textures.AddRange(loadMaterialTextures(m, TextureType.Height, dir));
                return new GameMesh(vertices, indices, textures, debugName);
            }


            private static List<GameTexture> loadMaterialTextures(Material m, TextureType texType, string dir)
            {
                List<GameTexture> ret = new List<GameTexture>();

                m.Log("Loading Baked Material Textures of type: " + Enum.GetName(typeof(TextureType), texType),
                    DebugChannel.Log);
                for (int i = 0; i < m.GetMaterialTextureCount(texType); i++)
                {
                    TextureSlot s;
                    m.GetMaterialTexture(texType, i, out s);
                    GameTexture tx = TextureIO.FileToTexture(dir + s.FilePath);
                    tx.TexType = texType;
                    tx.Path = s.FilePath;
                    ret.Add(tx);
                }

                return ret;
            }


        }


        #endregion

        #region Console Commands

        private static string cmd_TextureInfo(string[] args)
        {
            return "Loaded Textures: ";// + _GameTextureCache.Count;
        }

        private static string cmd_LtexToConsole(string[] args)
        {
            args.Log(cmd_ListTextures(args), DebugChannel.Log);

            return "Sucess";
        }

        private static string cmd_ListTextures(string[] args)
        {
            string s = "Textures:";
            //foreach (var i1 in _GameTextureCache)
            //{
            //    if (!i1.Key.StartsWith("FONT_"))
            //        s += "\n" + i1.Key + "(" + i1.Value + "):" + _GameTextureRefTable[i1.Value];
            //}

            return s;
        }


        public static void AddConsoleCommands(DebugConsoleComponent console)
        {
            console.AddCommand("ltex", cmd_TextureInfo);
            console.AddCommand("ltexall", cmd_ListTextures);
            console.AddCommand("ltex2c", cmd_LtexToConsole);
        }

        #endregion

        //Stores only the raw handles of textures

        //Functions(Texture):
        //LoadTexture
        //CreateTexture
        //LoadTextureFromFile
        //LoadTextureFromBitmap
        //LoadTextureFromByteArray
        //LoadTexturesFromAssimpScene
        //LoadTextureFromAssimpEmbeddedTexture
        //Turn Game Texture into Memory Buffer and the other way around
        //Textures are returned when the destructor of GameTexture is called(

        //Functions(Mesh):
        //LoadMeshFromFile
        //LoadMeshesFromFile

        //Functions(Shader)
        //LoadShaderFromSources
        //LoadShaderFromFiles

        //Functions(CL)
        //LoadKernelFromSource
        //LoadKernelFromFile
        //LoadImageDataIntoMemoryBuffer

        //Functions(Material)
        //LoadMaterialFromFile

        //Material
        //Contains uniform values in xml/json/lua? format

    }
}