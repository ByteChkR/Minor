using System;
using System.Collections.Generic;
using Demo.components;
using MinorEngine.components;
using MinorEngine.engine.audio;
using MinorEngine.engine.audio.sources;
using MinorEngine.engine.components;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Demo.scenes
{
    public class AudioDemoScene : AbstractScene
    {
        private LookAtComponent _camLookCommandComponent;
        private GameObject _sourceCube;

        private string cmd_LookAtAudioSource(string[] args)
        {
            if (_camLookCommandComponent.IsLooking)
                _camLookCommandComponent.SetTarget(null);
            else
                _camLookCommandComponent.SetTarget(_sourceCube);

            return "Changed Look behaviour to: " + _camLookCommandComponent.IsLooking;
        }


        private string cmd_ReLoadScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<AudioDemoScene>();
            return "Reloaded";
        }

        private string cmd_NextScene(string[] args)
        {
            GameEngine.Instance.InitializeScene<PhysicsDemoScene>();
            return "Loading Physics Demo Scene";
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
            return "New LocalPosition: " + pos.X + ":" + pos.Z + ":" + pos.Y;
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
            GameMesh bgBox = ResourceManager.MeshIO.FileToMesh("models/cube_flat.obj");

            bgBox.SetTextureBuffer(new[] { ResourceManager.TextureIO.FileToTexture("textures/ground4k.png") });


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

            DebugConsoleComponent dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
            dbg.AddCommand("mov", cmd_ChangeCameraPos);
            dbg.AddCommand("rot", cmd_ChangeCameraRot);
            dbg.AddCommand("reload", cmd_ReLoadScene);
            dbg.AddCommand("next", cmd_NextScene);
            dbg.AddCommand("lookat", cmd_LookAtAudioSource);
            GameEngine.Instance.World.Add(dbg.Owner);

            GameObject bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale = new Vector3(25, 1, 25);
            bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, 1));
            GameEngine.Instance.World.Add(bgObj);

            Camera c = new Camera(
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                    GameEngine.Instance.Width / (float)GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            //c.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-25));
            c.Translate(new Vector3(0, 20, 50));
            _camLookCommandComponent = new LookAtComponent();

            c.AddComponent(_camLookCommandComponent);

            _sourceCube = new GameObject(Vector3.UnitZ * -5, "Audio Source");

            GameMesh sourceCube = ResourceManager.MeshIO.FileToMesh("models/cube_flat.obj");
            sourceCube.SetTextureBuffer(new[] { ResourceManager.TextureIO.FileToTexture("textures/ground4k.png") });
            AudioSourceComponent source = new AudioSourceComponent();
            _sourceCube.AddComponent(source);
            _sourceCube.AddComponent(new RotateAroundComponent());
            _sourceCube.AddComponent(new MeshRendererComponent(shader, sourceCube, 1));
            if (!AudioManager.TryLoad("sounds/test_mono_16.wav", out AudioClip clip)) Console.ReadLine();

            source.SetClip(clip);
            source.Looping = true;
            //source.Play();
            GameEngine.Instance.World.Add(_sourceCube);

            AudioListener listener = new AudioListener();
            c.AddComponent(listener);
            GameEngine.Instance.World.Add(c);
            GameEngine.Instance.World.SetCamera(c);
        }
    }
}