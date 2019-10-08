using System.Collections.Generic;
using System.Linq;
using Engine.Core;
using Engine.Rendering.Contexts;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    public class Renderer
    {
        private readonly List<RenderTarget> Targets = new List<RenderTarget>();
        private int CurrentTarget { get; set; }
        private Color _clearColor = new Color(0, 0, 0, 0);

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

            var rt = new RenderTarget(null, 1, _clearColor);
            rt.MergeType = RenderTargetMergeType.Additive;
            AddRenderTarget(rt);
            var rt1 = new RenderTarget(new ScreenCamera(), 1 << 30, _clearColor);
            rt1.MergeType = RenderTargetMergeType.Additive;
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

        private static List<RenderContext> CreateRenderQueue(int renderTarget, Matrix4 view, RenderType type)
        {
            var Contexts = new List<RenderContext>();
            foreach (var renderer in GameObject.ObjsWithAttachedRenderers)
            {
                var context = renderer.RenderingComponent.Context;
                if (MaskHelper.IsContainedInMask(renderer.RenderingComponent.RenderMask, renderTarget, false) &&
                    context.RenderType == type)
                {
                    context.PrecalculateMV(view);
                    Contexts.Add(context);
                }
            }

            Contexts.Sort();
            return Contexts;
        }

        public void RenderAllTargets(AbstractScene scene)
        {
            scene.ComputeWorldTransformCache(Matrix4.Identity); //Compute all the World transforms and cache them


            //GL.Enable(EnableCap.ScissorTest);
            for (var i = 0; i < Targets.Count; i++)
            {
                CurrentTarget = i;
                var target = Targets[i];

                var c = target.PassCamera ?? scene.Camera;

                if (c != null)
                {
                    //GL.Scissor(target.ViewPort.X, target.ViewPort.Y, target.ViewPort.Width, target.ViewPort.Height);
                    GL.Viewport(target.ViewPort.X, target.ViewPort.Y, target.ViewPort.Width, target.ViewPort.Height);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, target.FrameBuffer);

                    GL.ClearColor(target.ClearColor);

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


                    var _opaque = CreateRenderQueue(target.PassMask, c.ViewMatrix, RenderType.Opaque);
                    Render(_opaque, c);
                    var _transparent =
                        CreateRenderQueue(target.PassMask, c.ViewMatrix, RenderType.Transparent);
                    Render(_transparent, c);


                    //Render(target.PassMask, c);


                    GL.Viewport(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                    //GL.Scissor(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                }
            }

            //GL.Disable(EnableCap.ScissorTest);
            RenderTargetMergeStage.MergeAndDisplayTargets(Targets.Where(x => x.MergeType != RenderTargetMergeType.None)
                .ToList());
            //TODO: WRITE RENDERING TO THE SCREEN(COMBINE THE RENDER TARGETS)
            //Maybe its useful to have a flag that defines if the rendertarget should be merged into the screen output
            //To Merge i need a vertex and fragment shader that can iterate over variable amounts of samplers(Can start out with 2)
        }

        public static void Render(List<RenderContext> contexts, ICamera cam)
        {
            foreach (var renderContext in contexts)
            {
                renderContext.Render(cam.ViewMatrix, cam.Projection);
            }
        }

        public static void Render(int PassMask, ICamera cam)
        {
            foreach (var renderer in GameObject.ObjsWithAttachedRenderers)
            {
                if (MaskHelper.IsContainedInMask(renderer.RenderingComponent.RenderMask, PassMask, false))
                {
                    RenderObject(renderer.RenderingComponent, renderer._worldTransformCache, cam.ViewMatrix,
                        cam.Projection);
                }
            }
        }


        public static void RenderObject(IRenderingComponent model, Matrix4 modelMat, Matrix4 viewMat,
            Matrix4 projMat)
        {
            model.Context.Render(viewMat, projMat);

            //RenderContext context = model.Context;
            //DrawObject(model.Context, modelMat, viewMat, projMat);
            //TODO: HERE GOES THE PASS MASK EVALUATION SO STUFF IGNORES IT(REQUIRES EVERY GAMEOBJECT TO HAVE A MASK AS WELL)
            //model?.RenderObject(modelMat, viewMat, projMat);
        }

        public enum RenderType
        {
            Opaque,
            Transparent
        }
    }
}