using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using Engine.AI;
using Engine.Core;
using Engine.Debug;
using Engine.Rendering.Contexts;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    /// <summary>
    /// Renderer that does all the drawing
    /// </summary>
    public class Renderer
    {
        /// <summary>
        ///  A list of render targets
        /// </summary>
        private readonly List<RenderTarget> Targets = new List<RenderTarget>();

        internal static List<LightComponent> Lights = new List<LightComponent>();

        /// <summary>
        /// Current target id
        /// </summary>
        private int CurrentTarget { get; set; }

        /// <summary>
        /// The Clear color of the two standard Render targets(World/UI)
        /// </summary>
        private Color _clearColor = new Color(0, 0, 0, 0);

        /// <summary>
        /// The Clear color of the two standard Render targets(World/UI)
        /// </summary>
        public Color ClearColor
        {
            set
            {
                _clearColor = value;
                rt1.ClearColor = rt.ClearColor = _clearColor;
            }
            get => _clearColor;
        }

        /// <summary>
        /// Default Render Target(Game World)
        /// </summary>
        private RenderTarget rt;

        /// <summary>
        /// Default Render Target(UI)
        /// </summary>
        private RenderTarget rt1;

        /// <summary>
        /// Internal Constructor
        /// </summary>
        internal Renderer()
        {
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            //GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            rt = new RenderTarget(null, 1, _clearColor);
            rt.MergeType = RenderTargetMergeType.Additive;
            AddRenderTarget(rt);
            rt1 = new RenderTarget(new ScreenCamera(), 1 << 30, _clearColor);
            rt1.MergeType = RenderTargetMergeType.Additive;
            AddRenderTarget(rt1);
        }

        /// <summary>
        /// Adds a render target to the Render Target List
        /// </summary>
        /// <param name="target">The Target to Add</param>
        public void AddRenderTarget(RenderTarget target)
        {
            Targets.Add(target);
            Targets.Sort();
        }

        /// <summary>
        /// Removes a render target from the Render Target List
        /// </summary>
        /// <param name="target">The Target to Remove</param>
        public void RemoveRenderTarget(RenderTarget target)
        {
            for (int i = Targets.Count - 1; i >= 0; i--)
            {
                RenderTarget renderTarget = Targets[i];
                if (renderTarget.FrameBuffer == target.FrameBuffer)
                {
                    Targets.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Creates a Render Queue that is ordered and is only containing objects of the specified types
        /// </summary>
        /// <param name="renderTarget">The Render Target ID</param>
        /// <param name="view">The View Matrix of the Camera Associated with the Render Target</param>
        /// <param name="type">The Render Type</param>
        /// <returns>A sorted list of renderer contexts</returns>
        private static List<RenderContext> CreateRenderQueue(int renderTarget, Matrix4 view, RenderType type)
        {
            List<RenderContext> Contexts = new List<RenderContext>();
            foreach (GameObject renderer in GameObject.ObjsWithAttachedRenderers)
            {
                RenderContext context = renderer.RenderingComponent.Context;
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

        /// <summary>
        /// Renders all targets and merges them into a single frame buffer
        /// </summary>
        /// <param name="scene">The Scene to be drawn</param>
        public void RenderAllTargets(AbstractScene scene)
        {
            scene.ComputeWorldTransformCache(Matrix4.Identity); //Compute all the World transforms and cache them


            MemoryTracer.AddSubStage("Render Target loop");
            //GL.Enable(EnableCap.ScissorTest);
            for (int i = 0; i < Targets.Count; i++)
            {
                MemoryTracer.NextStage("Rendering Render Target: " + i);
                CurrentTarget = i;
                RenderTarget target = Targets[i];

                ICamera c = target.PassCamera ?? scene.Camera;

                if (c != null)
                {
                    //GL.Scissor(target.ViewPort.X, target.ViewPort.Y, target.ViewPort.Width, target.ViewPort.Height);
                    GL.Viewport(target.ViewPort.X, target.ViewPort.Y, target.ViewPort.Width, target.ViewPort.Height);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, target.FrameBuffer);

                    GL.ClearColor(target.ClearColor);

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    Matrix4 vmat = c.ViewMatrix;

                    List<RenderContext> _opaque = CreateRenderQueue(target.PassMask, vmat, RenderType.Opaque);
                    Render(_opaque, vmat, c);
                    List<RenderContext> _transparent =
                        CreateRenderQueue(target.PassMask, vmat, RenderType.Transparent);
                    Render(_transparent, vmat, c);


                    //Render(target.PassMask, c);


                    GL.Viewport(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                    //GL.Scissor(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                }
            }
            MemoryTracer.ReturnFromSubStage();


            //GL.Disable(EnableCap.ScissorTest);
            RenderTargetMergeStage.MergeAndDisplayTargets(Targets.Where(x => x.MergeType != RenderTargetMergeType.None)
                .ToList());
            //TODO: WRITE RENDERING TO THE SCREEN(COMBINE THE RENDER TARGETS)
            //Maybe its useful to have a flag that defines if the rendertarget should be merged into the screen output
            //To Merge i need a vertex and fragment shader that can iterate over variable amounts of samplers(Can start out with 2)
        }

        /// <summary>
        /// Renders the Render Queue
        /// </summary>
        /// <param name="contexts">The Queue of Render Contexts</param>
        /// <param name="cam">The ICamera</param>
        public static void Render(List<RenderContext> contexts, Matrix4 viewM, ICamera cam)
        {

            for (int i = 0; i < contexts.Count; i++)
            {
                contexts[i].Render(viewM, cam.Projection);
                
            }
 
        }

        public enum RenderType
        {
            Opaque,
            Transparent
        }
    }
}