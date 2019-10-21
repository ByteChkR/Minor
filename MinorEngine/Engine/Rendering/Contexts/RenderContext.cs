using System;
using OpenTK;

namespace Engine.Rendering.Contexts
{
    /// <summary>
    /// Defines a Render Context.
    /// </summary>
    public abstract class RenderContext : IComparable<RenderContext>
    {
        /// <summary>
        /// Constructor for a render context
        /// </summary>
        /// <param name="program">The shader program to use</param>
        /// <param name="modelMatrix">The model matrix</param>
        /// <param name="worldSpace">Is the model in Camera Space</param>
        /// <param name="renderType">The Type of the Render Process</param>
        /// <param name="renderQueue">The render queue that is used</param>
        protected RenderContext(ShaderProgram program, Matrix4 modelMatrix, bool worldSpace,
            Renderer.RenderType renderType, int renderQueue)
        {
            Program = program;
            ModelMat = modelMatrix;
            WorldSpace = worldSpace;
            RenderType = renderType;
            RenderQueue = renderQueue;
        }

        /// <summary>
        /// Abstract function that will be called when the object should be drawn
        /// </summary>
        /// <param name="viewMat">View Matrix</param>
        /// <param name="projMat">Projection Matrix</param>
        public abstract void Render(Matrix4 viewMat, Matrix4 projMat);

        /// <summary>
        /// CompareTo Implementation
        /// If both transparent and in camera space
        /// </summary>
        /// <param name="other">The Object to compare against</param>
        /// <returns></returns>
        public int CompareTo(RenderContext other)
        {
            return -CmpTo(other);
        }

        /// <summary>
        /// CompareTo Implementation
        /// If both transparent and in camera space
        /// </summary>
        /// <param name="other">The Object to compare against</param>
        /// <returns></returns>
        private int CmpTo(RenderContext other)
        {
            if (RenderType == Renderer.RenderType.Transparent && other.RenderType == Renderer.RenderType.Transparent)
            {
                if (WorldSpace && other.WorldSpace)
                {
                    float d = MVPosition.LengthSquared - other.MVPosition.LengthSquared;
                    if (d > 0)
                    {
                        return 1;
                    }

                    return -1;
                }

                if (WorldSpace) // && !other.WorldSpace
                {
                    return -1;
                }

                if (other.WorldSpace) // && !WorldSpace
                {
                    return 1;
                }

                //If !WorldSpace && !other.WorldSpace

                int ret = RenderQueue.CompareTo(other.RenderQueue);
                return ret;
            }

            if (other.RenderType == Renderer.RenderType.Transparent) //&& RenderType != Renderer.RenderType.Transparent
            {
                return 1;
            }

            if (RenderType == Renderer.RenderType.Transparent) //&& other.RenderType != Renderer.RenderType.Transparent
            {
                return -1;
            }

            //if other.RenderType != Renderer.RenderType.Transparent && RenderType != Renderer.RenderType.Transparent
            //Let z buffer do the ordering
            return 0;
        }

        /// <summary>
        /// the shader program that is used
        /// </summary>
        public ShaderProgram Program { get; set; }

        /// <summary>
        /// the render queue that determines the order in which the objects are drawn
        /// This only applies to Screen space Render Contexts. For World space the ordering is worked out by the distance to the camera.
        /// </summary>
        public int RenderQueue { get; set; }

        /// <summary>
        /// The Model matrix that is used to do transformations
        /// </summary>
        public Matrix4 ModelMat { get; set; }

        /// <summary>
        /// Cached version of the ModelView Matrix(is used for ordered rendering
        /// </summary>
        public Matrix4 MV { get; private set; }

        /// <summary>
        /// The position of the Object in ModelView Space
        /// </summary>
        public Vector3 MVPosition { get; private set; }

        /// <summary>
        /// The Render type of the context
        /// </summary>
        public Renderer.RenderType RenderType { get; set; }

        /// <summary>
        /// A flag that indicates if the object is meant to be drawn in screen space or world space
        /// </summary>
        public bool WorldSpace { get; set; }

        /// <summary>
        /// Precalculates the ModelView Matrix
        /// </summary>
        /// <param name="view">The view matrix</param>
        public void PrecalculateMV(Matrix4 view)
        {
            MV = ModelMat * view;
            MVPosition = new Vector3(new Vector4(Vector3.Zero, 1) * MV);
        }
    }
}