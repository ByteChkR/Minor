using System;
using Engine.Core;
using Engine.Rendering;
using Engine.Rendering.Contexts;
using Engine.UI;
using OpenTK;

namespace Engine.Debug
{
    /// <summary>
    /// Graph Drawing Component
    /// </summary>
    public class GraphDrawingComponent : UIElement
    {
        /// <summary>
        /// Backing field of the graph data
        /// </summary>
        private Vector2[] _points;

        /// <summary>
        /// The backing field of the graphics context
        /// </summary>
        private GraphDrawingContext _context;

        /// <summary>
        /// The context that renders the graph data
        /// </summary>
        public override RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new GraphDrawingContext(_points, Position, Scale, Owner._worldTransformCache, WorldSpace,
                        Alpha, Shader, RenderQueue);
                }
                else if (ContextInvalid)
                {
                    ContextInvalid = false;
                    _context.Points = ComputeUVPos();
                    _context.Position = Position;
                    _context.Scale = Scale;
                    _context.ModelMat = Owner._worldTransformCache;
                    _context.WorldSpace = WorldSpace;
                    _context.Alpha = Alpha;
                    _context.Program = Shader;
                    _context.RenderQueue = RenderQueue;
                }

                return _context;
            }
        }

        /// <summary>
        /// Computes the UVs of the Quad that will contain the graph data
        /// </summary>
        /// <returns></returns>
        private Vector2[] ComputeUVPos()
        {
            Vector2[] ret = new Vector2[_points.Length];
            float max = 16f;
            for (int i = 0; i < _points.Length; i++)
            {
                max = MathF.Max(_points[i].Y, max);
            }

            Vector2 scale = new Vector2(Scale.X, Scale.Y / max);
            for (int i = 0; i < _points.Length; i++)
            {
                ret[i] = (Position + _points[i] - Vector2.One * 0.5f) * 2 * scale;
            }

            return ret;
        }

        /// <summary>
        /// Array of points containing the data to be drawn
        /// </summary>
        public Vector2[] Points
        {
            get => _points;
            set
            {
                _points = value;

                ContextInvalid = true;
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="shader">The shader to be used</param>
        /// <param name="worldSpace">flag if the graph is in world space</param>
        /// <param name="alpha">the alpha value of the graph</param>
        public GraphDrawingComponent(ShaderProgram shader, bool worldSpace, float alpha) :
            base(shader, worldSpace, alpha)
        {
            _points = new Vector2[0];
        }
    }
}