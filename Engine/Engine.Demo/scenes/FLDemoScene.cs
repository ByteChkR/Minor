using System.Collections.Generic;
using System.Drawing;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Demo.components;
using Engine.IO;
using Engine.OpenFL;
using Engine.Rendering;
using Engine.UI.Animations;
using Engine.UI.Animations.AnimationTypes;
using Engine.UI.Animations.Interpolators;
using Engine.UI.EventSystems;
using OpenTK;

namespace Engine.Demo.scenes
{
    public class FLDemoScene : AbstractScene
    {
        private RenderTarget splitCam;

        private static string cmd_ReLoadScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<FLDemoScene>();
            return "Reloaded";
        }

        private static string cmd_NextScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<AudioDemoScene>();
            return "Loading Audio Demo Scene";
        }

        private static string cmd_ChangeCameraPos(string[] args)
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

            Vector3 pos = new Vector3(x, y, z);
            GameEngine.Instance.CurrentScene.Camera.Translate(pos);
            pos = GameEngine.Instance.CurrentScene.Camera.GetLocalPosition();
            return "New LocalPosition: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }

        private static string cmd_ChangeCameraRot(string[] args)
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

            Vector3 pos = new Vector3(x, y, z);
            GameEngine.Instance.CurrentScene.Camera.Rotate(pos, MathHelper.DegreesToRadians(angle));

            return "Rotating " + angle + " degrees on Axis: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }


        protected override void InitializeScene()
        {
            Mesh sphere = MeshLoader.FileToMesh("assets/models/sphere_smooth.obj");
            Mesh plane = MeshLoader.FileToMesh("assets/models/plane.obj");
            Mesh bgBox = MeshLoader.FileToMesh("assets/models/cube_flat.obj");

            //MeshBuilder mb = new MeshBuilder();

            //mb.AddTriangle(
            //    new Vector3(-1, 0, -1),
            //    new Vector3(1, 0, 1),
            //    new Vector3(1, 0, -1));
            //mb.AddTriangle(
            //    new Vector3(-1, 0, 1),
            //    new Vector3(1, 0, 1),
            //    new Vector3(-1, 0, -1));
            //plane.Dispose();
            //plane = mb.ToMesh();


            GameObject objSphere = new GameObject(new Vector3(1, 1, 0), "SphereDisplay");
            objSphere.AddComponent(new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader, sphere,
                TextureLoader.FileToTexture("assets/textures/ground4k.png"), 1));
            objSphere.GetComponent<LitMeshRendererComponent>().Textures = new[]
                {objSphere.GetComponent<LitMeshRendererComponent>().Textures[0], DefaultFilepaths.DefaultTexture};

            objSphere.AddComponent(new RotatingComponent());

            GameObject objQuad = new GameObject(new Vector3(-1, 1, 0), "QuadDisplay");
            objQuad.AddComponent(new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader, plane,
                TextureLoader.FileToTexture("assets/textures/ground4k.png"), 1));
            objQuad.GetComponent<LitMeshRendererComponent>().Textures = new[]
                {objSphere.GetComponent<LitMeshRendererComponent>().Textures[0], DefaultFilepaths.DefaultTexture};

            objQuad.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(45));

            GameObject _sourceCube = new GameObject(new Vector3(0, 10, 10), "Light Source");

            Mesh sourceCube = MeshLoader.FileToMesh("assets/models/cube_flat.obj");
            _sourceCube.AddComponent(new LightComponent());
            _sourceCube.AddComponent(new RotateAroundComponent {Slow = 0.15f});
            _sourceCube.AddComponent(new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader, sourceCube,
                TextureLoader.ColorToTexture(Color.White), 1));

            GameObject uiText = new GameObject(new Vector3(0), "UIText");
            uiText.AddComponent(new FLGeneratorComponent(new List<LitMeshRendererComponent>
                {
                    objSphere.GetComponent<LitMeshRendererComponent>(), objQuad.GetComponent<LitMeshRendererComponent>()
                },
                512,
                512, true));

            Add(_sourceCube);
            GameEngine.Instance.CurrentScene.Add(uiText);
            DebugConsoleComponent dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
            dbg.AddCommand("mov", cmd_ChangeCameraPos);
            dbg.AddCommand("rot", cmd_ChangeCameraRot);
            dbg.AddCommand("reload", cmd_ReLoadScene);
            dbg.AddCommand("next", cmd_NextScene);
            GameEngine.Instance.CurrentScene.Add(dbg.Owner);
            GameEngine.Instance.CurrentScene.Add(objSphere);
            GameEngine.Instance.CurrentScene.Add(objQuad);

            GameObject bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale = new Vector3(25, 1, 25);

            Texture bgTex = TextureLoader.FileToTexture("assets/textures/ground4k.png");
            //BufferOperations.GetRegion<byte>(buf, new int3(), )


            bgObj.AddComponent(new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader, bgBox, bgTex, 1));
            GameEngine.Instance.CurrentScene.Add(bgObj);


            BasicCamera mainCamera =
                new BasicCamera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);

            object mc = mainCamera;

            EngineConfig.LoadConfig("assets/configs/camera_fldemo.xml", ref mc);


            GameEngine.Instance.CurrentScene.Add(mainCamera);
            GameEngine.Instance.CurrentScene.SetCamera(mainCamera);

            GameObject camContainer = new GameObject("CamContainer");

            BasicCamera inPicCam =
                new BasicCamera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            inPicCam.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(0));
            inPicCam.Translate(new Vector3(0, 2, 4));
            inPicCam.AddComponent(new RotateAroundComponent());
            GameObject zeroPoint = new GameObject("Zero");
            GameEngine.Instance.CurrentScene.Add(zeroPoint);
            LookAtComponent comp = new LookAtComponent();
            comp.SetTarget(zeroPoint);
            inPicCam.AddComponent(comp);
            GameEngine.Instance.CurrentScene.Add(inPicCam);


            splitCam = new RenderTarget(inPicCam, 1, Color.FromArgb(0, 0, 0, 0))
            {
                MergeType = RenderTargetMergeType.Additive,
                ViewPort = new Rectangle(0, 0, (int) (GameEngine.Instance.Width * 0.3f),
                    (int) (GameEngine.Instance.Height * 0.3f))
            };

            GameEngine.Instance.CurrentScene.Add(camContainer);
            GameEngine.Instance.AddRenderTarget(splitCam);

            GameObject obj = new GameObject("Button");
            Texture btnIdle = TextureLoader.ColorToTexture(Color.Green);
            Texture btnHover = TextureLoader.ColorToTexture(Color.Red);
            Texture btnClick = TextureLoader.ColorToTexture(Color.Blue);
            Button btn = new Button(btnIdle, DefaultFilepaths.DefaultUIImageShader, 1, btnClick, btnHover);
            LinearAnimation loadAnim = new LinearAnimation();
            loadAnim.Interpolator = new SmoothInterpolator();
            loadAnim.StartPos = Vector2.Zero + Vector2.UnitY * 1;
            loadAnim.EndPos = Vector2.Zero;
            loadAnim.MaxAnimationTime = 1;
            loadAnim.Trigger = AnimationTrigger.OnLoad;
            loadAnim.AnimationDelay = 1f;

            LinearAnimation clickAnim = new LinearAnimation();
            clickAnim.Interpolator = new Arc2Interpolator();
            clickAnim.StartPos = Vector2.Zero;
            clickAnim.EndPos = Vector2.Zero + Vector2.UnitY * 1;
            clickAnim.MaxAnimationTime = 1;
            clickAnim.Trigger = AnimationTrigger.OnClick;

            Animator anim = new Animator(new List<Animation> {loadAnim, clickAnim}, btn);

            obj.AddComponent(anim);
            obj.AddComponent(btn);
            Add(obj);
            btn.Scale = Vector2.One * 0.3f;
        }

        public override void OnDestroy()
        {
            GameEngine.Instance.RemoveRenderTarget(splitCam);
            splitCam.Dispose();
        }
    }
}