using System;
using MinorEngine.engine.core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering
{
    public class RenderTarget : IComparable<RenderTarget>, IDisposable
    {
        public ScreenRenderer.MergeType MergeType { get; set; } = ScreenRenderer.MergeType.Additive;
        public int PassMask { get; set; }
        public Color ClearColor { get; set; }
        public int FrameBuffer { get; }
        public int RenderedTexture { get; }
        public int DepthBuffer { get; }
        internal ICamera PassCamera { get; }

        public Rectangle ViewPort { get; set; } =
            new Rectangle(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);


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
                Console.ReadLine();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(FrameBuffer);
            GL.DeleteTexture(RenderedTexture);
            GL.DeleteRenderbuffer(DepthBuffer);
        }


        public int CompareTo(RenderTarget other)
        {
            return PassMask - other.PassMask;
        }

        public override int GetHashCode()
        {
            return RenderedTexture.GetHashCode() ^ FrameBuffer.GetHashCode() ^ DepthBuffer.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as RenderTarget;
            if (ReferenceEquals(other, null)) return false;
            return CompareTo(other) == 0;
        }

        public static bool operator ==(RenderTarget left, RenderTarget right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator >(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(RenderTarget left, RenderTarget right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator !=(RenderTarget left, RenderTarget right)
        {
            return !(left == right);
        }
    }
}