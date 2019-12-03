﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Engine.DataTypes;
using Engine.Debug;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    /// <summary>
    /// Implements the Merging of Framebuffers
    /// </summary>
    public static class RenderTargetMergeStage
    {
        /// <summary>
        /// Static Float Array that is used to create a screen space quad
        /// </summary>
        private static float[] _screenQuadVertexData =
        {
            // positions   // texCoords
            -1.0f, 1.0f, 0.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
            1.0f, -1.0f, 1.0f, 0.0f,

            -1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 1.0f, 1.0f
        };

        /// <summary>
        /// flag to indicate if the MergeStage has been initialized
        /// </summary>
        private static bool _init;

        /// <summary>
        /// The VAO of the Screen quad
        /// </summary>
        private static int _screenVAO;

        /// <summary>
        /// Render target 0 for the pingpong rendering
        /// </summary>
        private static RenderTarget _screenTarget0 =
            new RenderTarget(new ScreenCamera(), int.MaxValue, Color.FromArgb(0, 0, 0, 0));

        /// <summary>
        /// Render target 1 for the pingpong rendering
        /// </summary>
        private static RenderTarget _screenTarget1 =
            new RenderTarget(new ScreenCamera(), int.MaxValue, Color.FromArgb(0, 0, 0, 0));

        /// <summary>
        /// The shaders used for the different merge types
        /// </summary>
        private static Dictionary<RenderTargetMergeType, ShaderProgram> _mergeTypes =
            new Dictionary<RenderTargetMergeType, ShaderProgram>();

        /// <summary>
        /// Flag that indicates what is the next input buffer and what is the next output buffer
        /// </summary>
        private static bool _isOne;

        /// <summary>
        /// Initialization method
        /// </summary>
        private static void Init()
        {
            _init = true;
            _screenVAO = GL.GenVertexArray();
            int _screenVBO = GL.GenBuffer();
            GL.BindVertexArray(_screenVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _screenVBO);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (_screenQuadVertexData.Length * sizeof(float)),
                _screenQuadVertexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));


            DefaultFilepaths.DefaultMergeAddShader.AddUniformCache("destinationTexture");
            DefaultFilepaths.DefaultMergeAddShader.AddUniformCache("otherTexture");
            _mergeTypes.Add(RenderTargetMergeType.Additive, DefaultFilepaths.DefaultMergeAddShader);

            DefaultFilepaths.DefaultMergeMulShader.AddUniformCache("destinationTexture");
            DefaultFilepaths.DefaultMergeMulShader.AddUniformCache("otherTexture");

            _mergeTypes.Add(RenderTargetMergeType.Multiplikative, DefaultFilepaths.DefaultMergeMulShader);


            DefaultFilepaths.DefaultScreenShader.AddUniformCache("sourceTexture");
        }

        /// <summary>
        /// Flips input and output of GetTarget() and GetSource()
        /// </summary>
        private static void Ping()
        {
            _isOne = !_isOne;
        }

        /// <summary>
        /// Returns the Current Target that will be drawn onto
        /// </summary>
        /// <returns></returns>
        private static RenderTarget GetTarget()
        {
            return _isOne ? _screenTarget1 : _screenTarget0;
        }

        /// <summary>
        /// Returns the current Source that will be sampled
        /// </summary>
        /// <returns></returns>
        private static RenderTarget GetSource()
        {
            return _isOne ? _screenTarget0 : _screenTarget1;
        }

        /// <summary>
        /// merges the targets and draws the results on the back buffer of the OpenGL Window
        /// </summary>
        /// <param name="targets"></param>
        public static void MergeAndDisplayTargets(List<RenderTarget> targets)
        {
            if (!_init)
            {
                Init();
            }


            MemoryTracer.AddSubStage("Merge Framebuffers");
            
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            foreach (RenderTarget renderTarget in targets)
            {
                MemoryTracer.NextStage("Merge Framebuffer: " + renderTarget.PassMask);
                RenderTarget dst = GetTarget();
                RenderTarget src = GetSource();

                _mergeTypes[renderTarget.MergeType].Use();

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, dst.FrameBuffer);
                
                GL.ClearColor(dst.ClearColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //GL.Uniform1(_mergeShader.GetUniformLocation("divWeight"), 1 / (float)divideCount);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.Uniform1(_mergeTypes[renderTarget.MergeType].GetUniformLocation("destinationTexture"), 0);
                GL.BindTexture(TextureTarget.Texture2D, src.RenderedTexture);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.Uniform1(_mergeTypes[renderTarget.MergeType].GetUniformLocation("otherTexture"), 1);
                GL.BindTexture(TextureTarget.Texture2D, renderTarget.RenderedTexture);

                GL.BindVertexArray(_screenVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);


                Ping();

                
            }

            MemoryTracer.ReturnFromSubStage();

            MemoryTracer.NextStage("Rendering To Screen");
            GL.Disable(EnableCap.Blend);

            Ping();
            DefaultFilepaths.DefaultScreenShader.Use();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.ClearColor(Color.FromArgb(168, 143, 50, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Uniform1(DefaultFilepaths.DefaultScreenShader.GetUniformLocation("sourceTexture"), 0);
            GL.BindTexture(TextureTarget.Texture2D, GetTarget().RenderedTexture);
            

            GL.BindVertexArray(_screenVAO);
            
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);


            //Clear the ping pong buffers after rendering them to the screen
            //For whatever reason GL.Clear is not acting on the active framebuffer
            GL.ClearTexImage(_screenTarget1.RenderedTexture, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.ClearTexImage(_screenTarget0.RenderedTexture, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);


            GL.Enable(EnableCap.DepthTest);
        }
    }
}