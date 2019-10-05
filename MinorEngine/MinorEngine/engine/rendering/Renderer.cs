using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using Assimp;
using MinorEngine.components;
using MinorEngine.debug;
using MinorEngine.engine.core;
using MinorEngine.engine.ui;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace MinorEngine.engine.rendering
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
                if (renderTarget.FrameBuffer == target.FrameBuffer) Targets.RemoveAt(i);
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
                    Render(target.PassMask, world, target.PassCamera);
                else
                    Render(target.PassMask, world, world.Camera);

                GL.Viewport(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
                //GL.Scissor(0, 0, GameEngine.Instance.Width, GameEngine.Instance.Height);
            }

            //GL.Disable(EnableCap.ScissorTest);
            ScreenRenderer.MergeAndDisplayTargets(Targets.Where(x => x.MergeType != ScreenRenderer.MergeType.None)
                .ToList());
            //TODO: WRITE RENDERING TO THE SCREEN(COMBINE THE RENDER TARGETS)
            //Maybe its useful to have a flag that defines if the rendertarget should be merged into the screen output
            //To Merge i need a vertex and fragment shader that can iterate over variable amounts of samplers(Can start out with 2)
        }

        public static void Render(int PassMask, World world, ICamera cam)
        {
            foreach (var renderer in GameObject.ObjsWithAttachedRenderers)
                if (MaskHelper.IsContainedInMask(renderer.RenderingComponent.RenderMask, PassMask, false))
                    Render(world, renderer.RenderingComponent, renderer._worldTransformCache, cam.ViewMatrix,
                        cam.Projection);

            //Render(PassMask, world, world, cam, true);
        }

        public static void Render(int PassMask, World world, GameObject obj, ICamera cam, bool recursive)
        {
            Render(PassMask, world, obj, obj.GetWorldTransform(), Matrix4.Invert(cam.GetWorldTransform()),
                cam.Projection, recursive);
        }

        public static void Render(int PassMask, World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat,
            Matrix4 projMat,
            bool recursive)
        {
            RenderSelf(PassMask, world, obj, modelMat, viewMat, projMat);
            if (recursive) RenderChildren(PassMask, world, obj, modelMat, viewMat, projMat, recursive);
        }

        public static void RenderSelf(int PassMask, World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat,
            Matrix4 projMat)
        {
            if (obj.RenderingComponent != null &&
                MaskHelper.IsContainedInMask(obj.RenderingComponent.RenderMask, PassMask, false))
                Render(world, obj.RenderingComponent, modelMat, viewMat, projMat);
        }


        public static void RenderChildren(int PassMask, World world, GameObject obj, Matrix4 modelMat, Matrix4 viewMat,
            Matrix4 projMat,
            bool recursive)
        {
            if (obj.ChildCount < 1) return;
            GameObject child;
            for (int i = 0; i < obj.ChildCount; i++)
            {
                child = obj.GetChildAt(i);
                Render(PassMask, world, child, modelMat * child.Transform, viewMat, projMat, recursive);
            }
        }

        public static void Render(World world, IRenderingComponent model, Matrix4 modelMat, Matrix4 viewMat,
            Matrix4 projMat)
        {

            model.Context.Render(modelMat, viewMat, projMat);

            //RenderContext context = model.Context;
            //DrawObject(model.Context, modelMat, viewMat, projMat);
            //TODO: HERE GOES THE PASS MASK EVALUATION SO STUFF IGNORES IT(REQUIRES EVERY GAMEOBJECT TO HAVE A MASK AS WELL)
            //model?.Render(modelMat, viewMat, projMat);
        }

        public abstract class RenderContext
        {
            protected RenderContext(ShaderProgram program)
            {
                Program = program;
            }

            public abstract void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat);

            public ShaderProgram Program { get; }
        }


        public class UIRenderContext : RenderContext
        {
            public Vector2 Position { get; }
            public Vector2 Scale { get; }


            protected static int _vbo;
            protected static int _vao;
            protected static bool _init;

            public UIRenderContext(Vector2 position, Vector2 scale, ShaderProgram program) : base(program)
            {
                Position = position;
                Scale = scale;
            }

            private static void SetUpTextResources()
            {
                _init = true;
                _vao = GL.GenVertexArray();
                _vbo = GL.GenBuffer();
                GL.BindVertexArray(_vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            }

            public override void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
            {
                if (!_init) SetUpTextResources();
            }
        }

        public class TextRenderContext : UIRenderContext
        {
            public string DisplayText { get; }
            public GameFont FontFace { get; }





            public TextRenderContext(ShaderProgram program, Vector2 position, Vector2 scale, GameFont fontFace, string displayText) : base(position, scale, program)
            {
                FontFace = fontFace;
                DisplayText = displayText;
            }


            public override void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
            {
                base.Render(modelMat, viewMat, projMat);
                int scrW = GameEngine.Instance.Width;
                int scrH = GameEngine.Instance.Height;

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                Program.Use();

                Matrix4 trmat = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
                Matrix4 m = trmat;

                GL.UniformMatrix4(Program.GetUniformLocation("transform"), false, ref m);


                GL.Uniform3(Program.GetUniformLocation("textColor"), 1f, 0f, 0f);
                GL.Uniform1(Program.GetUniformLocation("sourceTexture"), 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindVertexArray(_vao);
                float x = Position.X;
                float y = Position.Y;
                for (int i = 0; i < DisplayText.Length; i++)
                {
                    if (DisplayText[i] == '\n')
                    {
                        FaceMetrics fm = FontFace.Metrics;
                        x = Position.X;
                        y -= fm.LineHeight / scrH * Scale.Y;
                        continue;
                    }

                    if (!FontFace.TryGetCharacter(DisplayText[i], out Character chr)) FontFace.TryGetCharacter('?', out chr);

                    float xpos = x + chr.BearingX / scrW * Scale.X;
                    float ypos = y - (chr.Height - chr.BearingY) / scrH * Scale.Y;

                    float w = chr.Width / (float)scrW * Scale.X;
                    float h = chr.Height / (float)scrH * Scale.Y;
                    float[] verts =
                    {
                        xpos, ypos + h, 0.0f, 1.0f,
                        xpos, ypos, 0.0f, 0.0f,
                        xpos + w, ypos, 1.0f, 0.0f,

                        xpos, ypos + h, 0.0f, 1.0f,
                        xpos + w, ypos, 1.0f, 0.0f,
                        xpos + w, ypos + h, 1.0f, 1.0f
                    };

                    if (chr.GlTexture != null)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, chr.GlTexture.TextureId);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(sizeof(float) * verts.Length),
                            verts);

                        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                    }

                    x += chr.Advance / scrW * Scale.X;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.Disable(EnableCap.Blend);
            }
        }


        public class MeshRenderContext : RenderContext
        {
            public GameMesh[] Meshes { get; }
            public MeshRenderContext(ShaderProgram program, GameMesh[] meshes) : base(program)
            {
                Meshes = meshes;

            }

            public override void Render(Matrix4 modelMat, Matrix4 viewMat, Matrix4 projMat)
            {
                Program.Use();

                GL.UniformMatrix4(Program.GetUniformLocation("modelMatrix"), false, ref modelMat);
                GL.UniformMatrix4(Program.GetUniformLocation("viewMatrix"), false, ref viewMat);
                GL.UniformMatrix4(Program.GetUniformLocation("projectionMatrix"), false, ref projMat);
                Matrix4 mvp = modelMat * viewMat * projMat;
                GL.UniformMatrix4(Program.GetUniformLocation("mvpMatrix"), false, ref mvp);

                foreach (GameMesh gameMesh in Meshes) gameMesh.Draw(Program);
            }
        }
    }
}