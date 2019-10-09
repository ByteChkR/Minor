using System.Collections.Generic;
using System.Resources;
using Assimp;
using Demo.components;
using Engine.Core;
using Engine.Debug;
using Engine.IO;
using Engine.Physics;
using Engine.Physics.BEPUphysics.Entities.Prefabs;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Demo.scenes
{
    public class PhysicsDemoScene : AbstractScene
    {
        private string cmd_ReLoadScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<PhysicsDemoScene>();
            return "Reloaded";
        }

        private string cmd_NextScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<FLDemoScene>();
            return "Loading FL Demo Scene";
        }

        private string cmd_ChangeCameraPos(string[] args)
        {
            if (args.Length != 3)
            {
                return "Invalid Arguments";
            }

            float x, y, z;
            if (!float.TryParse(args[0], out x))
            {
                return "Invalid Arguments";
            }

            if (!float.TryParse(args[1], out y))
            {
                return "Invalid Arguments";
            }

            if (!float.TryParse(args[2], out z))
            {
                return "Invalid Arguments";
            }

            var pos = new Vector3(x, y, z);
            GameEngine.Instance.CurrentScene.Camera.Translate(pos);
            pos = GameEngine.Instance.CurrentScene.Camera.GetLocalPosition();
            return "New LocalPosition: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }

        private string cmd_ChangeCameraRot(string[] args)
        {
            if (args.Length != 4)
            {
                return "Invalid Arguments";
            }

            float x, y, z, angle;
            if (!float.TryParse(args[0], out x))
            {
                return "Invalid Arguments";
            }

            if (!float.TryParse(args[1], out y))
            {
                return "Invalid Arguments";
            }

            if (!float.TryParse(args[2], out z))
            {
                return "Invalid Arguments";
            }

            if (!float.TryParse(args[3], out angle))
            {
                return "Invalid Arguments";
            }

            var pos = new Vector3(x, y, z);
            GameEngine.Instance.CurrentScene.Camera.Rotate(pos, MathHelper.DegreesToRadians(angle));

            return "Rotating " + angle + " degrees on Axis: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }


        protected override void InitializeScene()
        {
            var test = TextureLoader.FileToTexture("textures/ground4k.png");


            var rayLayer = LayerManager.RegisterLayer("raycast", new Layer(1, 2));
            var hybLayer = LayerManager.RegisterLayer("hybrid", new Layer(1, 1 | 2));
            var physicsLayer = LayerManager.RegisterLayer("physics", new Layer(1, 1));
            LayerManager.DisableCollisions(rayLayer, physicsLayer);

            var bgBox = MeshLoader.FileToMesh("models/cube_flat.obj");
            var box = MeshLoader.FileToMesh("models/cube_flat.obj");
            var sphere = MeshLoader.FileToMesh("models/sphere_smooth.obj");
            


            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UITextRender.fs"},
                {ShaderType.VertexShader, "shader/UIRender.vs"}
            }, out var textShader);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"}
            }, out var shader);

            var phys = new PhysicsDemoComponent();

            GameEngine.Instance.CurrentScene.AddComponent(phys); //Adding Physics Component to world.


            var dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
            dbg.AddCommand("mov", cmd_ChangeCameraPos);
            dbg.AddCommand("rot", cmd_ChangeCameraRot);
            dbg.AddCommand("reload", cmd_ReLoadScene);
            dbg.AddCommand("next", cmd_NextScene);
            GameEngine.Instance.CurrentScene.Add(dbg.Owner);

            var bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale = new Vector3(250, 1, 250);
            bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, TextureLoader.FileToTexture("textures/ground4k.png"), 1));
            var groundCol = new Collider(new Box(Vector3.Zero, 500, 1, 500), hybLayer);
            bgObj.AddComponent(groundCol);
            GameEngine.Instance.CurrentScene.Add(bgObj);

            var boxO = new GameObject(Vector3.UnitY * 3, "Box");
            boxO.AddComponent(new MeshRendererComponent(shader, bgBox, TextureLoader.FileToTexture("textures/ground4k.png"), 1));
            boxO.AddComponent(new Collider(new Box(Vector3.Zero, 1, 1, 1), physicsLayer));
            boxO.Translate(new Vector3(55, 0, 35));
            GameEngine.Instance.CurrentScene.Add(boxO);


            var mouseTarget = new GameObject(Vector3.UnitY * -3, "BG");
            mouseTarget.Scale = new Vector3(1, 1, 1);
            mouseTarget.AddComponent(new MeshRendererComponent(shader, sphere, TextureLoader.FileToTexture("textures/ground4k.png"), 1));

            GameEngine.Instance.CurrentScene.Add(mouseTarget);


            var c = new BasicCamera(
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                    GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            c.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-25));
            c.Translate(new Vector3(55, 10, 45));
            c.AddComponent(new CameraRaycaster(mouseTarget, 3, boxO));
            GameEngine.Instance.CurrentScene.Add(c);
            GameEngine.Instance.CurrentScene.SetCamera(c);
        }
    }
}