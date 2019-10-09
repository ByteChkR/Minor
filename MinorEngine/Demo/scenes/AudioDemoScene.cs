using System;
using System.Collections.Generic;
using System.Resources;
using Demo.components;
using Engine.Audio;
using Engine.Core;
using Engine.Debug;
using Engine.IO;
using Engine.Rendering;
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
            {
                _camLookCommandComponent.SetTarget(null);
            }
            else
            {
                _camLookCommandComponent.SetTarget(_sourceCube);
            }

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
            var bgBox = MeshLoader.FileToMesh("models/cube_flat.obj");
            

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

            var dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
            dbg.AddCommand("mov", cmd_ChangeCameraPos);
            dbg.AddCommand("rot", cmd_ChangeCameraRot);
            dbg.AddCommand("reload", cmd_ReLoadScene);
            dbg.AddCommand("next", cmd_NextScene);
            dbg.AddCommand("lookat", cmd_LookAtAudioSource);
            GameEngine.Instance.CurrentScene.Add(dbg.Owner);

            var bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale = new Vector3(25, 1, 25);
            bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, TextureLoader.FileToTexture("textures/ground4k.png"), 1));
            GameEngine.Instance.CurrentScene.Add(bgObj);

            var c = new BasicCamera(
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                    GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            //c.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-25));
            c.Translate(new Vector3(0, 20, 50));
            _camLookCommandComponent = new LookAtComponent();

            c.AddComponent(_camLookCommandComponent);

            _sourceCube = new GameObject(Vector3.UnitZ * -5, "Audio Source");

            var sourceCube = MeshLoader.FileToMesh("models/cube_flat.obj");
            var source = new AudioSourceComponent();
            _sourceCube.AddComponent(source);
            _sourceCube.AddComponent(new RotateAroundComponent());
            _sourceCube.AddComponent(new MeshRendererComponent(shader, sourceCube, TextureLoader.FileToTexture("textures/ground4k.png"), 1));
            if (!AudioLoader.TryLoad("sounds/test_mono_16.wav", out var clip))
            {
                Console.ReadLine();
            }

            source.SetClip(clip);
            source.Looping = true;
            //source.Play();
            GameEngine.Instance.CurrentScene.Add(_sourceCube);

            var listener = new AudioListener();
            c.AddComponent(listener);
            GameEngine.Instance.CurrentScene.Add(c);
            GameEngine.Instance.CurrentScene.SetCamera(c);
        }
    }
}