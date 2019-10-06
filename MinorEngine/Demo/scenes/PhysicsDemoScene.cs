﻿using System.Collections.Generic;
using Demo.components;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.components;
using MinorEngine.engine.components;
using MinorEngine.engine.core;
using MinorEngine.engine.physics;
using MinorEngine.engine.rendering;
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
            GameEngine.Instance.World.Camera.Translate(pos);
            pos = GameEngine.Instance.World.Camera.GetLocalPosition();
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
            GameEngine.Instance.World.Camera.Rotate(pos, MathHelper.DegreesToRadians(angle));

            return "Rotating " + angle + " degrees on Axis: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }


        protected override void InitializeScene()
        {
            var test = ResourceManager.TextureIO.FileToTexture("textures/ground4k.png");


            var rayLayer = LayerManager.RegisterLayer("raycast", new Layer(1, 2));
            var hybLayer = LayerManager.RegisterLayer("hybrid", new Layer(1, 1 | 2));
            var physicsLayer = LayerManager.RegisterLayer("physics", new Layer(1, 1));
            LayerManager.DisableCollisions(rayLayer, physicsLayer);

            var bgBox = ResourceManager.MeshIO.FileToMesh("models/cube_flat.obj");
            var box = ResourceManager.MeshIO.FileToMesh("models/cube_flat.obj");
            var sphere = ResourceManager.MeshIO.FileToMesh("models/sphere_smooth.obj");

            bgBox.SetTextureBuffer(new[] {ResourceManager.TextureIO.FileToTexture("textures/ground4k.png")});
            box.SetTextureBuffer(new[] {ResourceManager.TextureIO.FileToTexture("textures/ground4k.png")});


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

            GameEngine.Instance.World.AddComponent(phys); //Adding Physics Component to world.


            var dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
            dbg.AddCommand("mov", cmd_ChangeCameraPos);
            dbg.AddCommand("rot", cmd_ChangeCameraRot);
            dbg.AddCommand("reload", cmd_ReLoadScene);
            dbg.AddCommand("next", cmd_NextScene);
            GameEngine.Instance.World.Add(dbg.Owner);

            var bgObj = new GameObject(Vector3.UnitY * -3, "BG");
            bgObj.Scale = new Vector3(250, 1, 250);
            bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, 1));
            var groundCol = new Collider(new Box(Vector3.Zero, 500, 1, 500), hybLayer);
            bgObj.AddComponent(groundCol);
            GameEngine.Instance.World.Add(bgObj);

            var boxO = new GameObject(Vector3.UnitY * 3, "Box");
            boxO.AddComponent(new MeshRendererComponent(shader, bgBox, 1));
            boxO.AddComponent(new Collider(new Box(Vector3.Zero, 1, 1, 1), physicsLayer));
            boxO.Translate(new Vector3(55, 0, 35));
            GameEngine.Instance.World.Add(boxO);


            var mouseTarget = new GameObject(Vector3.UnitY * -3, "BG");
            mouseTarget.Scale = new Vector3(1, 1, 1);
            mouseTarget.AddComponent(new MeshRendererComponent(shader, sphere, 1));

            GameEngine.Instance.World.Add(mouseTarget);


            var c = new Camera(
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f),
                    GameEngine.Instance.Width / (float) GameEngine.Instance.Height, 0.01f, 1000f), Vector3.Zero);
            c.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-25));
            c.Translate(new Vector3(55, 10, 45));
            c.AddComponent(new CameraRaycaster(mouseTarget, 3, boxO));
            GameEngine.Instance.World.Add(c);
            GameEngine.Instance.World.SetCamera(c);
        }
    }
}