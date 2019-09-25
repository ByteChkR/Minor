using System.Collections.Generic;
using System.Linq;
using GameEngine.engine.rendering;
using MinorEngine.engine.core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering
{
    public class Renderer
    {
        private readonly List<RenderTarget> _targets = new List<RenderTarget>();
        private Color _clearColor = Color.Green;
        public Color ClearColor
        {
            set
            {
                _clearColor = value;
                GL.ClearColor(_clearColor);
            }
            get => _clearColor;
        }

        public Renderer()
        {
            
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            RenderTarget rt = new RenderTarget(0, _clearColor);
            rt.MergeInScreenBuffer = true;
            AddRenderTarget(rt);
        }

        public void AddRenderTarget(RenderTarget target)
        {
            _targets.Add(target);
            _targets.Sort();
        }

        public void RenderAllTargets(World world)
        {
            foreach (RenderTarget target in _targets)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, target.FrameBuffer);
                GL.Enable(EnableCap.DepthTest);
                GL.ClearColor(target.ClearColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                Render(target.PassMask, world);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(_clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            ErrorCode gle = GL.GetError();
            //Render(0, world);
            ScreenRenderer.MergeAndDisplayTargets(_targets.Where(x=>x.MergeInScreenBuffer).ToList());
            //TODO: WRITE RENDERING TO THE SCREEN(COMBINE THE RENDER TARGETS)
            //Maybe its useful to have a flag that defines if the rendertarget should be merged into the screen output
            //To Merge i need a vertex and fragment shader that can iterate over variable amounts of samplers(Can start out with 2)
        }

        public void Render(int PassMask, World world)
        {
            Render(PassMask, world, world, world.Camera, true);

        }

        public static void Render(int PassMask, World world, GameObject obj, Camera cam, bool recursive)
        {
            Render(PassMask, world, obj, obj.GetWorldTransform(), Matrix4.Invert(cam.GetWorldTransform()), cam.Projection, recursive);
        }

        public static void Render(int PassMask, World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat,
            bool recursive)
        {
            RenderSelf(PassMask, world, obj, modelMat, viewMat, projMat);
            if (recursive)
            {
                RenderChildren(PassMask, world, obj, modelMat, viewMat, projMat, recursive);
            }

        }

        public static void RenderSelf(int PassMask, World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            if (obj.RenderMask == PassMask)
            {
                Render(world, obj.Shader, obj.Model, modelMat, viewMat, projMat);
            }
        }


        public static void RenderChildren(int PassMask, World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat,
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
                Render(PassMask, world, child, modelMat * child.Transform, viewMat, projMat, recursive);
            }
        }

        public static void Render(World world, ShaderProgram prog, GameModel model, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            //TODO: HERE GOES THE PASS MASK EVALUATION SO STUFF IGNORES IT(REQUIRES EVERY GAMEOBJECT TO HAVE A MASK AS WELL)
            if (model != null && prog != null)
            {
                model.Render(prog, modelMat, viewMat, projMat);
            }
        }



    }
}