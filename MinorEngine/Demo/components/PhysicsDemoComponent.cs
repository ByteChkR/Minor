using System;
using System.Collections.Generic;
using MinorEngine.BEPUphysics.Entities.Prefabs;
using MinorEngine.components;
using MinorEngine.engine.components;
using MinorEngine.engine.core;
using MinorEngine.engine.physics;
using MinorEngine.engine.rendering;
using MinorEngine.engine.ui.utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Demo.components
{
    public class PhysicsDemoComponent : AbstractComponent
    {
        private static ShaderProgram _objShader;

        GameModel Box = new GameModel("models/cube_flat.obj", false);
        GameModel Sphere = new GameModel("models/sphere_smooth.obj", false);
        protected override void Awake()
        {
            //Physics.AddBoxStatic(System.Numerics.Vector3.UnitY * -4, new System.Numerics.Vector3(50, 10, 50), 1, 3);

            base.Awake();
            DebugConsoleComponent comp = Owner.World.GetChildWithName("Console")
                .GetComponent<DebugConsoleComponent>();

            GameTexture tex = TextureProvider.Load("textures/TEST.png");
            Box.SetTextureBuffer(0, new[] { tex });
            Sphere.SetTextureBuffer(0, new[] { tex });


            comp?.AddCommand("rain", cmd_SpawnColliders);
            comp?.AddCommand("gravity", cmd_SetGravity);
            comp?.AddCommand("reset", cmd_ResetCollider);
        }

        protected override void OnDestroy()
        {
        }

        public static string cmd_SetGravity(string[] args)
        {
            if (args.Length != 3) return "Expected 3 component vector";

            if (float.TryParse(args[0], out float x)) return "Wrong X Component";
            if (float.TryParse(args[1], out float y)) return "Wrong Y Component";
            if (float.TryParse(args[2], out float z)) return "Wrong Z Component";

            //Physics.Gravity = new System.Numerics.Vector3(x, y, z);

            return "Gravity Set to: "; // + Physics.Gravity;
        }
        private List<GameObject> Collider = new List<GameObject>();
        private string cmd_ResetCollider(string[] args)
        {
            int count = Collider.Count;
            foreach (var gameObject in Collider)
            {
                gameObject.Destroy();
            }
            Collider.Clear();
            string ret = "";
            return ret + "\nReloaded " + count + " Colliders";
        }
        public string cmd_SpawnColliders(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int nmbrs)) return "Not a number.";

            if (_objShader == null)
                ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
                {
                    {ShaderType.FragmentShader, "shader/texture.fs"},
                    {ShaderType.VertexShader, "shader/texture.vs"}
                }, out _objShader);

            Random rnd = new Random();
            for (int i = 0; i < nmbrs; i++)
            {
                Vector3 pos = new Vector3((float)rnd.NextDouble(), 3 + (float)rnd.NextDouble(),
                    (float)rnd.NextDouble());
                pos -= Vector3.One * 0.5f;
                pos *= 50;


                GameObject obj = new GameObject(pos, "Sphere");
                float radius = 0.3f + (float)rnd.NextDouble();
                obj.Scale(new Vector3(radius / 2));
                if (rnd.Next(0, 2) == 1)
                {
                    obj.AddComponent(new MeshRendererComponent(_objShader, Box, 1));


                    Collider coll = new Collider(new Box(Vector3.Zero, radius, radius, radius, 1));
                    obj.AddComponent(coll);
                }
                else
                {
                    obj.AddComponent(new MeshRendererComponent(_objShader, Sphere, 1));
                    Collider coll = new Collider(new Sphere(Vector3.Zero, radius, 1));
                    obj.AddComponent(coll);
                }
                Collider.Add(obj);
                Owner.World.Add(obj);
            }


            return nmbrs + " Objects Created.";
        }
    }
}