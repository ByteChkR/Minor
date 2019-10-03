using System.Collections.Generic;
using System.Linq;
using MinorEngine.components;
using MinorEngine.engine.core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering
{
    public class Renderer
    {
        private readonly List<RenderTarget> Targets = new List<RenderTarget>();
        private int CurrentTarget { get; set; }
        private Color _clearColor = new Color(0,0,0,0);
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
            //GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            RenderTarget rt = new RenderTarget(null, 1, _clearColor);
            rt.MergeType = ScreenRenderer.MergeType.Additive;
            AddRenderTarget(rt);
            RenderTarget rt1 = new RenderTarget(new UICamera(), 1 << 30, _clearColor);
            rt1.MergeType = ScreenRenderer.MergeType.Additive;
            AddRenderTarget(rt1);
        }

        public void AddRenderTarget(RenderTarget target)
        {
            Targets.Add(target);
            Targets.Sort();
        }

        public void RemoveRenderTarget(RenderTarget target)
        {
            for (var i = Targets.Count - 1; i >= 0; i--)
            {
                var renderTarget = Targets[i];
                if (renderTarget.FrameBuffer == target.FrameBuffer)
                {
                    Targets.RemoveAt(i);
                }
            }
        }

        public void RenderAllTargets(World world)
        {


            world.ComputeWorldTransformCache(Matrix4.Identity); //Compute all the World transforms and cache them


            //GL.Enable(EnableCap.ScissorTest);
            for (var i = 0; i < Targets.Count; i++)
            {
                CurrentTarget = i;
                RenderTarget target = Targets[i];
                //GL.Scissor(target.ViewPort.X, target.ViewPort.Y, target.ViewPort.Width, target.ViewPort.Height);
                GL.Viewport(target.ViewPort.X, target.ViewPort.Y, target.ViewPort.Width, target.ViewPort.Height);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, target.FrameBuffer);

                GL.ClearColor(target.ClearColor);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (target.PassCamera != null)
                {
                    Render(target.PassMask, world, target.PassCamera);
                }
                else
                {
                    Render(target.PassMask, world, world.Camera);
                }

                GL.Viewport(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                //GL.Scissor(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                
            }
            //GL.Disable(EnableCap.ScissorTest);
            ScreenRenderer.MergeAndDisplayTargets(Targets.Where(x => x.MergeType != ScreenRenderer.MergeType.None).ToList());
            //TODO: WRITE RENDERING TO THE SCREEN(COMBINE THE RENDER TARGETS)
            //Maybe its useful to have a flag that defines if the rendertarget should be merged into the screen output
            //To Merge i need a vertex and fragment shader that can iterate over variable amounts of samplers(Can start out with 2)
        }

        public static void Render(int PassMask, World world, ICamera cam)
        {

            foreach (var renderer in GameObject.ObjsWithAttachedRenderers)
            {
                if (MaskHelper.IsContainedInMask(renderer.RenderingComponent.RenderMask, PassMask, false))
                {
                    Render(world, renderer.RenderingComponent, renderer._worldTransformCache, cam.ViewMatrix, cam.Projection);
                }

            }

            //Render(PassMask, world, world, cam, true);

        }

        public static void Render(int PassMask, World world, GameObject obj, ICamera cam, bool recursive)
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
            if (obj.RenderingComponent != null && MaskHelper.IsContainedInMask(obj.RenderingComponent.RenderMask, PassMask, false))
            {
                Render(world, obj.RenderingComponent, modelMat, viewMat, projMat);
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

        public static void Render(World world,IRenderingComponent model, Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
        {
            //TODO: HERE GOES THE PASS MASK EVALUATION SO STUFF IGNORES IT(REQUIRES EVERY GAMEOBJECT TO HAVE A MASK AS WELL)
            model?.Render(modelMat, viewMat, projMat);
        }



    }
}