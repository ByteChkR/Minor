using System;
using System.Drawing;
using Engine.Audio;
using Engine.Core;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Demo.components;
using Engine.IO;
using Engine.Rendering;
using OpenTK;

namespace Engine.Demo.scenes
{
    public class AudioDemoScene : AbstractScene
    {
        private LookAtComponent camLookCommandComponent;
        private GameObject sourceCube;



        protected override void InitializeScene()
        {


            Add(DebugConsoleComponent.CreateConsole());

            GameObject bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale = new Vector3(25, 1, 25);
            bgObj.AddComponent(new MeshRendererComponent(DefaultFilepaths.DefaultUnlitShader, Prefabs.Cube,
                TextureLoader.ColorToTexture(Color.MediumPurple), 1));
            Add(bgObj);

            BasicCamera c = new BasicCamera(
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                    GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            c.Translate(new Vector3(0, 4, 0));
            camLookCommandComponent = new LookAtComponent();

            c.AddComponent(camLookCommandComponent);

            sourceCube = new GameObject(Vector3.UnitZ * -5, "Audio Source");
            
            AudioSourceComponent source = new AudioSourceComponent();
            sourceCube.AddComponent(source);
            sourceCube.AddComponent(new RotateAroundComponent());
            sourceCube.AddComponent(new MeshRendererComponent(DefaultFilepaths.DefaultUnlitShader, Prefabs.Cube,
                DefaultFilepaths.DefaultTexture, 1));
            if (!AudioLoader.TryLoad("assets/sounds/test_mono_16.wav", out AudioFile clip))
            {
                Console.ReadLine();
            }

            source.Clip = clip;
            source.Looping = true;
            source.Play();
            Add(sourceCube);

            AudioListener listener = new AudioListener();
            c.AddComponent(listener);
            Add(c);
            SetCamera(c);
        }
    }
}