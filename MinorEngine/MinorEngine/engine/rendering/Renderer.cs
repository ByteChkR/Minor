using GameEngine.engine.core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.rendering
{
    public class Renderer
    {
        private Color _clearColor = Color.Green;
        public Color ClearColor
        {
            set
            {
                _clearColor = value;
                GL.ClearColor(_clearColor);
            }
        }

        public Renderer()
        {

            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        }

        public void Render(World world)
        {
            GL.ClearColor(_clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Render(world, world, world.Camera, true);

        }

        public static void Render(World world, GameObject obj, Camera cam, bool recursive)
        {
            Render(world, obj, obj.GetWorldTransform(), Matrix4.Invert(cam.GetWorldTransform()), cam.Projection, recursive);
        }

        public static void Render(World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat,
            bool recursive)
        {
            RenderSelf(world, obj, modelMat, viewMat, projMat);
            if (recursive)
            {
                RenderChildren(world, obj, modelMat, viewMat, projMat, recursive);
            }

        }

        public static void RenderSelf(World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            Render(world, obj.Shader, obj.Model, modelMat, viewMat, projMat);
        }


        public static void RenderChildren(World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat,
            bool recursive)
        {
            if (obj.ChildCount < 1)
            {
                return;
            }
            GameObject child;
            for (int i = 0; i < obj.ChildCount; i++)
            {
                child = obj.GetChildAt(i);
                Render(world, child, modelMat * child.Transform, viewMat, projMat, recursive);
            }
        }

        public static void Render(World world, ShaderProgram prog, GameModel model, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            if (model != null && prog != null)
            {
                model.Render(prog, modelMat, viewMat, projMat);
            }
        }



    }
}