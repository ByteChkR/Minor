using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Demo.components;
using Engine.IO;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL;
using Engine.Rendering;
using OpenTK;

namespace Engine.Demo.scenes
{
    public class FlDemoScene : AbstractScene
    {
        private RenderTarget splitCam;

        private Texture GenerateGroundTexture()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int texWidth = 1024;
            int texHeight = 1024;
            Interpreter i = new Interpreter(Clapi.MainThread, "assets/filter/fldemo/grass.fl", OpenCL.TypeEnums.DataTypes.Uchar1,
                Clapi.CreateEmpty<byte>(Clapi.MainThread, texWidth * texHeight * 4, MemoryFlag.ReadWrite), texWidth,
                texHeight, 1, 4, "assets/kernel/", true);

            do
            {
                i.Step();
            } while (!i.Terminated);

            Texture tex = TextureLoader.ParameterToTexture(texWidth, texHeight);
            TextureLoader.Update(tex, i.GetResult<byte>(), (int)tex.Width, (int)tex.Height);
            Logger.Log("Time for Ground Texture(ms): " + sw.ElapsedMilliseconds, DebugChannel.Log, 10);
            sw.Stop();
            return tex;
        }

        protected override void InitializeScene()
        {
            Mesh plane = MeshLoader.FileToMesh("assets/models/plane.obj");

            Texture texQuad = TextureLoader.ParameterToTexture(1024, 1024);
            Texture texSphere = TextureLoader.ParameterToTexture(1024, 1024);

            GameObject objSphere = new GameObject(new Vector3(1, 1, 0), "SphereDisplay");

            LitMeshRendererComponent sphereLmr = new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader,
                Prefabs.Sphere,
                texSphere, 1);
            objSphere.AddComponent(sphereLmr);
            sphereLmr.Textures = new[]
                {sphereLmr.Textures[0], DefaultFilepaths.DefaultTexture};

            objSphere.AddComponent(new RotatingComponent());

            GameObject objQuad = new GameObject(new Vector3(-1, 1, 0), "QuadDisplay");
            LitMeshRendererComponent quadLmr = new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader, plane,
                texQuad, 1);
            objQuad.AddComponent(quadLmr);
            quadLmr.Textures = new[]
                {quadLmr.Textures[0], DefaultFilepaths.DefaultTexture};

            objQuad.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(45));

            GameObject sourceCube = new GameObject(new Vector3(0, 10, 10), "Light Source");
            
            sourceCube.AddComponent(new LightComponent());
            sourceCube.AddComponent(new RotateAroundComponent { Slow = 0.15f });
            sourceCube.AddComponent(new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader, Prefabs.Cube,
                TextureLoader.ColorToTexture(Color.White), 1));

            GameObject uiText = new GameObject(new Vector3(0), "UIText");
            uiText.AddComponent(new FlGeneratorComponent(new List<LitMeshRendererComponent>
                {
                    sphereLmr, quadLmr
                },
                512,
                512, true));

            Add(sourceCube);
            Add(uiText);
            Add(DebugConsoleComponent.CreateConsole());
            Add(objSphere);
            Add(objQuad);

            GameObject bgObj = new GameObject(Vector3.UnitY * -3, "BG") { Scale = new Vector3(25, 1, 25) };
            


            bgObj.AddComponent(new LitMeshRendererComponent(DefaultFilepaths.DefaultLitShader, Prefabs.Cube, GenerateGroundTexture(), 1));
            Add(bgObj);


            BasicCamera mainCamera =
                new BasicCamera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float)GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);

            object mc = mainCamera;

            EngineConfig.LoadConfig("assets/configs/camera_fldemo.xml", ref mc);


            Add(mainCamera);
            SetCamera(mainCamera);

            GameObject camContainer = new GameObject("CamContainer");

            BasicCamera inPicCam =
                new BasicCamera(
                    Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float)GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            inPicCam.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(0));
            inPicCam.Translate(new Vector3(0, 2, 4));
            inPicCam.AddComponent(new RotateAroundComponent());
            GameObject zeroPoint = new GameObject("Zero");
            Add(zeroPoint);
            LookAtComponent comp = new LookAtComponent();
            comp.SetTarget(zeroPoint);
            inPicCam.AddComponent(comp);
            Add(inPicCam);


            splitCam = new RenderTarget(inPicCam, 1, Color.FromArgb(0, 0, 0, 0))
            {
                MergeType = RenderTargetMergeType.Additive,
                ViewPort = new Rectangle(0, 0, (int)(GameEngine.Instance.Width * 0.3f),
                    (int)(GameEngine.Instance.Height * 0.3f))
            };

            Add(camContainer);
            GameEngine.Instance.AddRenderTarget(splitCam);
        }

        public override void OnDestroy()
        {
            GameEngine.Instance.RemoveRenderTarget(splitCam);
            splitCam.Dispose();
        }
    }
}