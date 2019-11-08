﻿using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.DataTypes;
using Engine.Demo.components;
using Engine.IO;
using Engine.Physics;
using Engine.Physics.BEPUphysics.Entities.Prefabs;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;
using MathHelper = Engine.Physics.BEPUutilities.MathHelper;
using Vector3 = Engine.Physics.BEPUutilities.Vector3;

namespace Engine.Demo.scenes.testing
{
    public class PhysicsScene : AbstractScene
    {
        private Texture Tex;
        private Mesh Sphere;
        private Mesh Box;
        private Random Rnd;
        private int ObjCount = 1000;
        private int StaticObjCount = 7500;
        protected override void InitializeScene()
        {
            Tex = TextureLoader.ColorToTexture(Color.Red);
            Sphere = MeshLoader.FileToMesh("assets/models/sphere_smooth.obj");
            Box = MeshLoader.FileToMesh("assets/models/cube_flat.obj");
            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "assets/shader/texture.fs"},
                {ShaderType.VertexShader, "assets/shader/texture.vs"}
            }, out ShaderProgram shader);
            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "assets/shader/lit/point.fs"},
                {ShaderType.VertexShader, "assets/shader/lit/point.vs"}
            }, out ShaderProgram litShader);

            Rnd = new Random();


            BasicCamera mainCamera =
                new BasicCamera(
                    Matrix4.CreatePerspectiveFieldOfView(OpenTK.MathHelper.DegreesToRadians(75f),
                        GameEngine.Instance.Width / (float)GameEngine.Instance.Height, 0.01f, 1000f), OpenTK.Vector3.Zero);

            object mc = mainCamera;

            EngineConfig.LoadConfig("assets/configs/camera_physics_test.xml", ref mc);

            Add(mainCamera);
            SetCamera(mainCamera);
            PhysicsEngine.Gravity = -OpenTK.Vector3.UnitY*9.81f;
            GameObject ground = new GameObject("Ground");
            Collider c = new Collider(new Box(Vector3.Zero, 1000, 100, 1000), "physics");
            ground.AddComponent(c);
            MeshRendererComponent mrc = new MeshRendererComponent(shader, Box, TextureLoader.ColorToTexture(Color.Blue), 1);
            ground.AddComponent(mrc);
            Add(ground);
            ground.Scale = new Vector3(500, 50, 500);
            ground.LocalPosition = new Vector3(0, -150, 0);

            for (int i = 0; i < StaticObjCount; i++)
            {
                GameObject obj = new GameObject("StaticColl");
                Collider objColl = new Collider(new Sphere(Vector3.Zero, 1), "physics");
                obj.AddComponent(objColl);
                LitMeshRendererComponent objMrc = new LitMeshRendererComponent(litShader, Sphere, TextureLoader.ColorToTexture(Color.Green), 1);
                obj.AddComponent(objMrc);
                Add(obj);
                obj.LocalPosition = new Vector3((float)((Rnd.NextDouble() * 100) - 50), (float)((Rnd.NextDouble() * 25) + 1), (float)((Rnd.NextDouble() * 100) - 50));
            }


            for (int i = 0; i < ObjCount; i++)
            {
                GameObject obj = new GameObject("DynamicColl");
                Collider objColl = new Collider(new Box(Vector3.Zero, 2, 2, 2, (float)Rnd.NextDouble()*2+1), "physics");
                obj.AddComponent(objColl);
                LitMeshRendererComponent objMrc = new LitMeshRendererComponent(litShader, Box, TextureLoader.ColorToTexture(Color.Red), 1);
                obj.AddComponent(objMrc);
                Add(obj);
                obj.LocalPosition = new Vector3((float)((Rnd.NextDouble() * 100) - 50), (float)((Rnd.NextDouble() * 10) + 25), (float)((Rnd.NextDouble() * 100) - 50));
            }

            GameObject helper = new GameObject("SceneHelper");
            Add(helper);
            helper.AddComponent(new GeneralTimer(30, SceneRunner.Instance.NextScene));
            helper.AddComponent(new LightComponent());
            helper.LocalPosition = new Vector3(50, 150, 50);
        }
    }
}