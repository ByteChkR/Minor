using System;
using Engine.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    /// <summary>
    /// Implements the Render Target
    /// </summary>
    public class RenderTarget : IComparable<RenderTarget>, IDisposable
    {
        /// <summary>
        /// The Merge type how the Framebuffer will get merged by the RenderTargetMergeStage
        /// </summary>
        public RenderTargetMergeType MergeType { get; set; } = RenderTargetMergeType.Additive;

        /// <summary>
        /// The Mask for the Target
        /// If an RenderingComponent has 1 | 2 as a render mask it will be drawn in RenderTarget 1 and 2
        /// </summary>
        public int PassMask { get; set; }

        /// <summary>
        /// The Color that is used to clear the buffer when reusing it
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        /// The Framebuffer Handle that is rendered to
        /// </summary>
        public int FrameBuffer { get; }

        /// <summary>
        /// The Texture Attachment
        /// </summary>
        public int RenderedTexture { get; }

        /// <summary>
        /// Depthbuffer attachment
        /// </summary>
        public int DepthBuffer { get; }

        /// <summary>
        /// The Camera associated with the rendering target(can be null. in this case the current scene camera will be taken)
        /// </summary>
        internal ICamera PassCamera { get; }

        /// <summary>
        /// The viewport of the Render Target
        /// </summary>
        public Rectangle ViewPort { get; set; } =
            new Rectangle(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);


        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="cam">The Camera associated with the Render Target</param>
        /// <param name="PassMask">The Mask for the Render Target</param>
        /// <param name="ClearColor">The Clear color that is used to clear the framebuffer</param>
        /// <param name="noDepth">if true there will be no depth attachment to the Framebuffer</param>
        public RenderTarget(ICamera cam, int PassMask, Color ClearColor, bool noDepth = false)
        {
            this.PassMask = PassMask;
            this.ClearColor = ClearColor;

            PassCamera = cam;


            FrameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);

            RenderedTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, RenderedTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16,
                GameEngine.Instance.Width, GameEngine.Instance.Height, 0, PixelFormat.Bgra,
                PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                RenderedTexture, 0);

            GL.DrawBuffers(1, new[] {DrawBuffersEnum.ColorAttachment0});

            if (noDepth)
            {
                DepthBuffer = -1;
            }
            else
            {
                DepthBuffer = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent,
                    GameEngine.Instance.Width, GameEngine.Instance.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer, DepthBuffer);
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.ReadLine();
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// Disposes the Framebuffer and all the associated GL objects
        /// </summary>
        public void Dispose()
        {
            GL.DeleteFramebuffer(FrameBuffer);
            GL.DeleteTexture(RenderedTexture);
            GL.DeleteRenderbuffer(DepthBuffer);
        }

        /// <summary>
        /// Compare to function.
        /// Compares PassMask 
        /// </summary>
        /// <param name="other">The instance to compare against</param>
        /// <returns></returns>
        public int CompareTo(RenderTarget other)
        {
            return PassMask.CompareTo(other.PassMask);
        }

        /// <summary>
        /// Simple XOr Hashcode
        /// </summary>
        /// <returns>a semi reliable hash code</returns>
        public override int GetHashCode()
        {
            return RenderedTexture.GetHashCode() ^ FrameBuffer.GetHashCode() ^ DepthBuffer.GetHashCode();
        }

        /// <summary>
        /// Equals Function
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the render targets have the same mask</returns>
        public override bool Equals(object obj)
        {
            RenderTarget other = obj as RenderTarget;
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Overrides for Operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(RenderTarget left, RenderTarget right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Overrides for Operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Overrides for Operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Overrides for Operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Overrides for Operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Overrides for Operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(RenderTarget left, RenderTarget right)
        {
            return !(left == right);
        }
    }
}