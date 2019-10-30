using System;
using System.Collections.Generic;
using Engine.AI;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Demo.components;
using Engine.IO;
using Engine.Physics;
using Engine.Physics.BEPUphysics.Entities.Prefabs;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Demo.scenes
{
    public class AStarDemoScene : AbstractScene
    {
        private ShaderProgram litShader;
        private AINode[,] nodes;
        private Texture tex;
        private Texture beginTex;
        private Texture endTex;
        private Texture blockTex;
        private List<AINode> path;

        protected override void InitializeScene()
        {

            int rayLayer = LayerManager.RegisterLayer("raycast", new Layer(1, 2));
            int hybLayer = LayerManager.RegisterLayer("hybrid", new Layer(1, 1 | 2));
            int physicsLayer = LayerManager.RegisterLayer("physics", new Layer(1, 1));
            LayerManager.DisableCollisions(rayLayer, physicsLayer);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "assets/shader/lit/point.fs"},
                {ShaderType.VertexShader, "assets/shader/lit/point.vs"}
            }, out litShader);

            BasicCamera mainCamera =
                new BasicCamera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float)GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);

            object mc = mainCamera;

            EngineConfig.LoadConfig("assets/configs/camera_astardemo.xml", ref mc);

            GameObject _sourceCube = new GameObject(new Vector3(0, 0,0), "Light Source");
            GameObject _hackCube = new GameObject(new Vector3(0, 8, -50),"Workaround");
            Add(_sourceCube);
            Add(_hackCube);
            Mesh sourceCube = MeshLoader.FileToMesh("assets/models/cube_flat.obj");
            _sourceCube.AddComponent(new LightComponent());


            mainCamera.AddComponent(new CameraRaycaster(_sourceCube, 3, _hackCube));

            GameObject bgObj = new GameObject(new Vector3(0, -3, -32), "BG");
            bgObj.Scale = new Vector3(32, 1, 32);

            Collider groundCol = new Collider(new Box(Vector3.Zero, 64, 1, 64), hybLayer);
            Texture bgTex = TextureLoader.FileToTexture("assets/textures/ground4k.png");
            bgTex.TexType = TextureType.Diffuse;
            bgObj.AddComponent(groundCol);

            tex = TextureLoader.ColorToTexture(System.Drawing.Color.Green);
            beginTex = TextureLoader.ColorToTexture(System.Drawing.Color.Blue);
            endTex = TextureLoader.ColorToTexture(System.Drawing.Color.Red);
            blockTex = TextureLoader.ColorToTexture(System.Drawing.Color.DarkMagenta);
            tex.TexType = beginTex.TexType = endTex.TexType = blockTex.TexType = TextureType.Diffuse;

            DebugConsoleComponent c = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();

            c.AddCommand("repath", ResetPaths);
            c.AddCommand("rp", ResetPaths);

            Add(c.Owner);

            bgObj.AddComponent(new LitMeshRendererComponent(litShader,  Prefabs.Cube, bgTex, 1));
            Add(bgObj);
            Add(mainCamera);
            SetCamera(mainCamera);

            Random rnd = new Random();
            nodes = GenerateNodeGraph(64, 64);
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {

                    if (rnd.Next(0, 6) == 0)
                    {
                        nodes[i, j].Walkable = false;
                        nodes[i, j].Owner.AddComponent(new LitMeshRendererComponent(litShader,  Prefabs.Sphere, blockTex, 1));
                    }
                    nodes[i, j].Owner.Scale *= 0.3f;

                    Add(nodes[i, j].Owner);
                }
            }


        }


        private string ResetPaths(string[] args)
        {
            if (path != null)
            {
                foreach (AINode aiNode in path)
                {
                    if (aiNode.Walkable)
                        aiNode.Owner.RemoveComponent<MeshRendererComponent>();
                }
                path.Clear();
            }
            Random rnd = new Random();
            AINode startNode = nodes[rnd.Next(0, 64), rnd.Next(0, 64)];
            AINode endNode = nodes[rnd.Next(0, 64), rnd.Next(0, 64)];
            while (!startNode.Walkable)
            {
                startNode = nodes[rnd.Next(0, 64), rnd.Next(0, 64)];
            }

            while (!endNode.Walkable)
            {
                endNode= nodes[rnd.Next(0, 64), rnd.Next(0, 64)];
                
            }
            path = AStarResolver.FindPath(startNode, endNode, out bool found);

            for (int i = 0; i < path.Count; i++)
            {
                path[i].Owner.AddComponent(new LitMeshRendererComponent(litShader,  Prefabs.Cube, tex, 1));
            }

            startNode.Owner.GetComponent<LitMeshRendererComponent>().Textures[0] = beginTex;
            endNode.Owner.GetComponent<LitMeshRendererComponent>().Textures[0] = endTex;
            return "Success: " + found;
        }

        private AINode[,] GenerateNodeGraph(int width, int length)
        {
            AINode[,] nodes = new AINode[width, length];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    GameObject obj = new GameObject("NodeW" + i + "L:" + j);
                    obj.LocalPosition = new Physics.BEPUutilities.Vector3(i - (width / 2), 0, -j);
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