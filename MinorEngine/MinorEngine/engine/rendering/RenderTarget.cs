using System;
using Common;
using MinorEngine.engine.core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.rendering
{
    public class RenderTarget : IComparable<RenderTarget>
    {
        public int PassMask { get; set; }
        public Color ClearColor { get; set; }
        public int FrameBuffer { get; }
        public int RenderedTexture { get; }
        public int DepthBuffer { get; }
        public bool MergeInScreenBuffer { get; set; }

        public RenderTarget(int PassMask, Color ClearColor)
        {
            this.PassMask = PassMask;
            this.ClearColor = ClearColor;

            FrameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);

            RenderedTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, RenderedTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRgb,
                AbstractGame.Instance.Settings.Width, AbstractGame.Instance.Settings.Height, 0, PixelFormat.Rgb,
                PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderedTexture, 0);

            GL.DrawBuffers(1, new[] { DrawBuffersEnum.ColorAttachment0 });

            DepthBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent,
                AbstractGame.Instance.Settings.Width, AbstractGame.Instance.Settings.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, DepthBuffer);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.ReadLine();
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        public int CompareTo(RenderTarget other)
        {
            return PassMask - other.PassMask;
        }

    }
}