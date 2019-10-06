using System;
using OpenTK;

namespace MinorEngine.engine.rendering.contexts
{
    public abstract class RenderContext : IComparable<RenderContext>
    {
        protected RenderContext(ShaderProgram program, Matrix4 modelMatrix, bool worldSpace,
            Renderer.RenderType renderType, int renderQueue)
        {
            Program = program;
            ModelMat = modelMatrix;
            WorldSpace = worldSpace;
            RenderType = renderType;
            RenderQueue = renderQueue;
        }

        public abstract void Render(Matrix4 viewMat, Matrix4 projMat);

        public int CompareTo(RenderContext other)
        {
            if (RenderType == Renderer.RenderType.Transparent)
            {
                if (WorldSpace && other.WorldSpace)
                {
                    var d = MVPosition.LengthSquared - other.MVPosition.LengthSquared;
                    if (d > 0)
                    {
                        return 1;
                    }

                    return -1;
                }

                if (WorldSpace)
                {
                    return -1;
                }

                if (other.WorldSpace)
                {
                    return 1;
                }

                var ret = RenderQueue.CompareTo(other.RenderQueue);
                return ret;
            }

            return 0;
        }

        public ShaderProgram Program { get; }

        public int RenderQueue { get; set; }
        public Matrix4 ModelMat { get; set; }

        public Matrix4 MV { get; private set; }
        public Vector3 MVPosition { get; private set; }
        public Renderer.RenderType RenderType { get; set; }

        public bool WorldSpace { get; set; }

        public void PrecalculateMV(Matrix4 view)
        {
            MV = ModelMat * view;
            MVPosition = new Vector3(new Vector4(Vector3.Zero, 1) * MV);
        }
    }
}