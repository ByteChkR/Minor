using System.Collections.Generic;
using Demo.components;
using MinorEngine.components;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using MinorEngine.engine.ui.utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Demo.scenes
{
    public class FLDemoScene : AbstractScene
    {
        private RenderTarget splitCam;

        private string cmd_ReLoadScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<FLDemoScene>();
            return "Reloaded";
        }

        private string cmd_NextScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<AudioDemoScene>();
            return "Loading Audio Demo Scene";
        }

        private string cmd_ChangeCameraPos(string[] args)
        {
            if (args.Length != 3) return "Invalid Arguments";

            float x, y, z;
            if (!float.TryParse(args[0], out x)) return "Invalid Arguments";
            if (!float.TryParse(args[1], out y)) return "Invalid Arguments";
            if (!float.TryParse(args[2], out z)) return "Invalid Arguments";

            Vector3 pos = new Vector3(x, y, z);
            GameEngine.Instance.World.Camera.Translate(pos);
            pos = GameEngine.Instance.World.Camera.GetLocalPosition();
            return "New Position: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }

        private string cmd_ChangeCameraRot(string[] args)
        {
            if (args.Length != 4) return "Invalid Arguments";
            float x, y, z, angle;
            if (!float.TryParse(args[0], out x)) return "Invalid Arguments";
            if (!float.TryParse(args[1], out y)) return "Invalid Arguments";
            if (!float.TryParse(args[2], out z)) return "Invalid Arguments";
            if (!float.TryParse(args[3], out angle)) return "Invalid Arguments";

            Vector3 pos = new Vector3(x, y, z);
            GameEngine.Instance.World.Camera.Rotate(pos, MathHelper.DegreesToRadians(angle));

            return "Rotating " + angle + " degrees on Axis: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }


        protected override void InitializeScene()
        {
            GameModel sphere = new GameModel("models/sphere_smooth.obj");
            GameModel plane = new GameModel("models/plane.obj");


            GameModel bgBox = new GameModel("models/cube_flat.obj");

            bgBox.SetTextureBuffer(0, new[] {TextureProvider.Load("textures/ground4k.png")});


            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UITextRender.fs"},
                {ShaderType.VertexShader, "shader/UIRender.vs"}
            }, out ShaderProgram textShader);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"}
            }, out ShaderProgram shader);

            GameObject objSphere = new GameObject(new Vector3(1, 1, 0), "SphereDisplay");
            objSphere.AddComponent(new MeshRendererComponent(shader, sphere, 1));
            objSphere.AddComponent(new RotatingComponent());

            GameObject objQuad = new GameObject(new Vector3(-1, 1, 0), "QuadDisplay");
            objQuad.AddComponent(new MeshRendererComponent(shader, plane, 1));
            objQuad.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(90));

            GameObject uiText = new GameObject(new Vector3(0), "UIText");
            uiText.AddComponent(new FLGeneratorComponent(new List<MeshRendererComponent>
                    {objSphere.GetComponent<MeshRendererComponent>(), objQuad.GetComponent<MeshRendererComponent>()},
                512,
                512));


            GameEngine.Instance.World.Add(uiText);
            DebugConsoleComponent dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
            dbg.AddCommand("mov", cmd_ChangeCameraPos);
            dbg.AddCommand("rot", cmd_ChangeCameraRot);
            dbg.AddCommand("reload", cmd_ReLoadScene);
            dbg.AddCommand("next", cmd_NextScene);
            GameEngine.Instance.World.Add(dbg.Owner);
            GameEngine.Instance.World.Add(objSphere);
            GameEngine.Instance.World.Add(objQuad);

            GameObject bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale(new Vector3(25, 1, 25));
            bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, 1));
            GameEngine.Instance.World.Add(bgObj);


            Camera mainCamera =
                new Camera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            mainCamera.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-25));
            mainCamera.Translate(new Vector3(0, 2, 2));
            GameEngine.Instance.World.Add(mainCamera);
            GameEngine.Instance.World.SetCamera(mainCamera);

            GameObject camContainer = new GameObject("CamContainer");

            Camera inPicCam =
                new Camera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            inPicCam.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(0));
            inPicCam.Translate(new Vector3(0, 2, 4));
            inPicCam.AddComponent(new RotateAroundComponent());
            GameEngine.Instance.World.Add(inPicCam);


            splitCam = new RenderTarget(inPicCam, 1, new Color(0, 0, 0, 0))
            {
                MergeType = ScreenRenderer.MergeType.Additive,
                ViewPort = new Rectangle(0, 0, (int) (GameEngine.Instance.Width * 0.3f),
                    (int) (GameEngine.Instance.Height * 0.3f))
            };

            GameEngine.Instance.World.Add(camContainer);
            GameEngine.Instance.AddRenderTarget(splitCam);
        }

        public override void OnDestroy()
        {
            GameEngine.Instance.RemoveRenderTarget(splitCam);
            //splitCam.Destroy();
        }
    }
}