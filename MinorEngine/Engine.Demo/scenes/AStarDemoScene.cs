using System.Collections.Generic;
using Engine.AI;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.IO;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Demo.scenes
{
    public class AStarDemoScene : AbstractScene
    {
        protected override void InitializeScene()
        {
            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/lit/point.fs"},
                {ShaderType.VertexShader, "shader/lit/point.vs"}
            }, out ShaderProgram litShader);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"}
            }, out ShaderProgram shader);

            BasicCamera mainCamera =
                new BasicCamera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float)GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);

            object mc = mainCamera;

            EngineConfig.LoadConfig("configs/camera_fldemo.xml", ref mc);



            GameObject bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale = new Vector3(25, 1, 25);

            Texture bgTex = TextureLoader.FileToTexture("textures/ground4k.png");

            Texture tex = TextureLoader.ColorToTexture(System.Drawing.Color.Green);
            

            Add(DebugConsoleComponent.CreateConsole());

            bgObj.AddComponent(new MeshRendererComponent(litShader, true, Prefabs.Cube, bgTex, 1));
            Add(bgObj);
            Add(mainCamera);
            SetCamera(mainCamera);


            AINode[,] nodes = GenerateNodeGraph(64, 64);
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    nodes[i, j].Owner.Scale *= 0.3f;
                    nodes[i, j].Owner.AddComponent(new MeshRendererComponent(shader, false, Prefabs.Cube, tex.Copy(), 1));
                    Add(nodes[i, j].Owner);
                }
            }
        }


        private AINode[,] GenerateNodeGraph(int width, int length)
        {
            AINode[,] nodes = new AINode[width, length];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    GameObject obj = new GameObject("NodeW" + i + "L:" + j);
                    obj.LocalPosition = new Physics.BEPUutilities.Vector3(i-(width/2), 0, -j);
                    AINode node = new AINode(true);
                    obj.AddComponent(node);
                    nodes[i, j] = node;
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    AINode current = nodes[i, j];
                    for (int k = -1; k < 1; k++)
                    {
                        for (int s = -1; s < 1; s++)
                        {
                            if (i + k < 0 || i + k >= width || j + s < 0 || j + s >= length || (k == 0 && s == 0)) continue;

                            current.AddConnection(nodes[i + k, j + s]);
                        }
                    }
                }
            }

            return nodes;
        }
    }
}