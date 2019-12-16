using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Engine.Core;
using Engine.Debug;
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
        /// Different Render Types
        /// </summary>
        public enum RenderType
        {
            Opaque,
            Transparent
        }

        /// <summary>
        /// List of Light Components in the Scene
        /// </summary>
        internal static List<LightComponent> Lights = new List<LightComponent>();

        /// <summary>
        ///  A list of render targets
        /// </summary>
        private readonly List<RenderTarget> targets = new List<RenderTarget>();

        /// <summary>
        /// The Clear color of the two standard Render targets(World/UI)
        /// </summary>
        private Color clearColor = Color.FromArgb(0, 0, 0, 0);

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
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            rt = new RenderTarget(null, 1, clearColor) {MergeType = RenderTargetMergeType.Additive};
            AddRenderTarget(rt);
            rt1 = new RenderTarget(new ScreenCamera(), 1 << 30, clearColor)
            {
                MergeType = RenderTargetMergeType.Additive
            };
            AddRenderTarget(rt1);
        }

        /// <summary>
        /// Current target id
        /// </summary>
        private int CurrentTarget { get; set; }

        /// <summary>
        /// The Clear color of the two standard Render targets(World/UI)
        /// </summary>
        public Color ClearColor
        {
            set
            {
                clearColor = value;
                rt1.ClearColor = rt.ClearColor = clearColor;
            }
            get => clearColor;
        }

        /// <summary>
        /// Adds a render target to the Render Target List
        /// </summary>
        /// <param name="target">The Target to Add</param>
        public void AddRenderTarget(RenderTarget target)
        {
            targets.Add(target);
            targets.Sort();
        }

        /// <summary>
        /// Removes a render target from the Render Target List
        /// </summary>
        /// <param name="target">The Target to Remove</param>
        public void RemoveRenderTarget(RenderTarget target)
        {
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                RenderTarget renderTarget = targets[i];
                if (renderTarget.FrameBuffer == target.FrameBuffer)
                {
                    targets.RemoveAt(i);
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
        private static List<RenderingComponent> CreateRenderQueue(int renderTarget, Matrix4 view, RenderType type)
        {
            List<RenderingComponent> contexts = new List<RenderingComponent>();
            foreach (GameObject renderer in GameObject.ObjsWithAttachedRenderers)
            {
                RenderingComponent context = renderer.RenderingComponent;
                if (MaskHelper.IsContainedInMask(renderer.RenderingComponent.RenderQueue, renderTarget, false) &&
                    context.RenderType == type)
                {
                    context.PrecalculateMv(view);
                    contexts.Add(context);
                }
            }

            contexts.Sort();
            return contexts;
        }

        /// <summary>
        /// Renders all targets and merges them into a single frame buffer
        /// </summary>
        /// <param name="scene">The Scene to be drawn</param>
        public void RenderAllTargets(AbstractScene scene)
        {
            scene.ComputeWorldTransformCache(Matrix4.Identity); //Compute all the World transforms and cache them


            MemoryTracer.AddSubStage("Render Target loop");
            for (int i = 0; i < targets.Count; i++)
            {
                MemoryTracer.NextStage("Rendering Render Target: " + i);
                CurrentTarget = i;
                RenderTarget target = targets[i];

                ICamera c = target.PassCamera ?? scene.Camera;

                if (c != null)
                {
                    GL.Viewport(target.ViewPort.X, target.ViewPort.Y, target.ViewPort.Width, target.ViewPort.Height);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, target.FrameBuffer);

                    GL.ClearColor(target.ClearColor);

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    Matrix4 vmat = c.ViewMatrix;

                    List<RenderingComponent> opaque = CreateRenderQueue(target.PassMask, vmat, RenderType.Opaque);
                    Render(opaque, vmat, c);
                    List<RenderingComponent> transparent =
                        CreateRenderQueue(target.PassMask, vmat, RenderType.Transparent);
                    Render(transparent, vmat, c);


                    GL.Viewport(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                }
            }

            MemoryTracer.ReturnFromSubStage();

            
            RenderTargetMergeStage.MergeAndDisplayTargets(targets.Where(x => x.MergeType != RenderTargetMergeType.None)
                .ToList());
        }

        /// <summary>
        /// Renders the Render Queue
        /// </summary>
        /// <param name="contexts">The Queue of Render Contexts</param>
        /// <param name="viewM">the Viewing Matrix</param>
        /// <param name="cam">The ICamera</param>
        public static void Render(List<RenderingComponent> contexts, Matrix4 viewM, ICamera cam)
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                contexts[i].Render(viewM, cam.Projection);
            }
        }
    }
}