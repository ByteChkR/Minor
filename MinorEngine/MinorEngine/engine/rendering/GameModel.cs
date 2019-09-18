using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assimp;
using Assimp.Configs;
using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.rendering
{
    public class GameModel
    {
        public List<GameMesh> meshes = new List<GameMesh>();
        private string directory;

        public GameModel(string file)
        {
            this.Log("Loading Model File: " + file, DebugChannel.Log);
            LoadModel(file);
        }


        public void Render(ShaderProgram shader, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {


            shader.Use();

            GL.UniformMatrix4(shader.GetUniformLocation("modelMatrix"), false, ref modelMat);
            GL.UniformMatrix4(shader.GetUniformLocation("viewMatrix"), false, ref viewMat);
            GL.UniformMatrix4(shader.GetUniformLocation("projectionMatrix"), false, ref projMat);
            Matrix4 mvp = modelMat * viewMat * projMat;
            GL.UniformMatrix4(shader.GetUniformLocation("mvpMatrix"), false, ref mvp);

            foreach (GameMesh gameMesh in meshes)
            {
                gameMesh.Draw(shader);
            }

        }


        private void LoadModel(string path)
        {

            AssimpContext context = new AssimpContext();
            context.SetConfig(new NormalSmoothingAngleConfig(66));
            Scene s = context.ImportFile(path);

            if (s == null || (s.SceneFlags & SceneFlags.Incomplete) != 0 || s.RootNode == null)
            {
                this.Log("Loading Model File failed.", DebugChannel.Error);
                return;
            }

            directory = Path.GetDirectoryName(path);
            if (directory == string.Empty)
            {
                directory = ".";
            }


            this.Log("Loading Model Finished.", DebugChannel.Log);

            this.Log("Processing Nodes...", DebugChannel.Log);
            processNode(s.RootNode, s);
        }

        public void processNode(Node node, Scene s)
        {

            this.Log("Processing Node: " + node.Name, DebugChannel.Log);
            if (node.HasMeshes)
            {
                this.Log("Adding " + node.MeshCount + " Meshes...", DebugChannel.Log);
                for (int i = 0; i < node.MeshCount; i++)
                {
                    meshes.Add(processMesh(s.Meshes[node.MeshIndices[i]], s));
                }
            }

            if (node.HasChildren)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    processNode(node.Children[i], s);
                }
            }
        }

        private GameMesh processMesh(Mesh mesh, Scene s)
        {
            List<GameVertex> vertices = new List<GameVertex>();
            List<uint> indices = new List<uint>();
            List<GameTexture> textures = new List<GameTexture>();


            this.Log("Converting Imported Mesh File Structure to Game Engine Structure", DebugChannel.Log);


            this.Log("Copying Vertex Data...", DebugChannel.Log);
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vector3D vert = mesh.Vertices[i];
                Vector3D norm = mesh.Normals[i];
                Vector3D tan = mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D(0);
                Vector3D bit = mesh.HasTangentBasis ? mesh.BiTangents[i] : new Vector3D(0);
                Vector3D uv = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0);

                GameVertex v = new GameVertex()
                {
                    Position = new Vector3(vert.X, vert.Y, vert.Z),
                    Normal = new Vector3(norm.X, norm.Y, norm.Z),
                    UV = new Vector2(uv.X, uv.Y),
                    Bittangent = new Vector3(bit.X, bit.Y, bit.Z),
                    Tangent = new Vector3(tan.X, tan.Y, tan.Z)
                };

                vertices.Add(v);
            }


            this.Log("Calculating Indices...", DebugChannel.Log);

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face f = mesh.Faces[i];
                indices.AddRange(f.Indices.Select(x => (uint)x));
            }



            Material m = s.Materials[mesh.MaterialIndex];

            this.Log("Loading Baked Material: " + m.Name, DebugChannel.Log);

            textures.AddRange(loadMaterialTextures(m, TextureType.Diffuse, "texture_diffuse"));
            textures.AddRange(loadMaterialTextures(m, TextureType.Specular, "texture_specular"));
            textures.AddRange(loadMaterialTextures(m, TextureType.Normals, "texture_normal"));
            textures.AddRange(loadMaterialTextures(m, TextureType.Height, "texture_height"));
            return new GameMesh(vertices, indices, textures);
        }


        private List<GameTexture> loadMaterialTextures(Material m, TextureType texType, string typeName)
        {
            List<GameTexture> ret = new List<GameTexture>();

            this.Log("Loading Baked Material Textures of type: " + Enum.GetName(typeof(TextureType), texType), DebugChannel.Log);
            for (int i = 0; i < m.GetMaterialTextureCount(texType); i++)
            {
                TextureSlot s;
                m.GetMaterialTexture(texType, i, out s);
                GameTexture tx = GameTexture.Load(directory + s.FilePath);
                tx.TexType = texType;
                tx.Path = s.FilePath;
                ret.Add(tx);
            }

            return ret;
        }
    }
}
