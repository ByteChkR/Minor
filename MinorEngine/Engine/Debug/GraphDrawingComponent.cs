using System;
using Engine.Core;
using Engine.Rendering;
using Engine.Rendering.Contexts;
using Engine.UI;
using OpenTK;

namespace Engine.Debug
{
    public class GraphDrawingComponent : UIElement
    {
        private GraphDrawingContext _context;
        public override RenderContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new GraphDrawingContext(_points, Position, Scale, Owner._worldTransformCache, WorldSpace, Alpha, Shader, RenderQueue);
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

        private Vector2[] _points;
        public Vector2[] Points
        {
            get => _points;
            set
            {
                _points = value;

                ContextInvalid = true;
            }
        }

        public GraphDrawingComponent(ShaderProgram shader, bool worldSpace, float alpha) :
            base(shader, worldSpace, alpha)
        {
            _points = new Vector2[0];
        }


    }
}