using System.Drawing;
using Engine.DataTypes;

namespace Engine.IO
{
    /// <summary>
    /// static Class providing frequently used objects
    /// </summary>
    public static class Prefabs
    {
        private static Texture _white;
        private static Texture _black;

        /// <summary>
        /// Backing field of the Public Property Cube
        /// </summary>
        private static Mesh _cube = null;

        /// <summary>
        /// Backing field of the Public Property Sphere
        /// </summary>
        private static Mesh _sphere = null;

        /// <summary>
        /// Returns an instance of a Cube
        /// </summary>
        public static Mesh Cube
        {
            get
            {
                if (_cube == null)
                {
                    _cube = MeshLoader.FileToMesh("assets/models/cube_flat.obj");
                }

                return _cube.Copy();
            }
        }

        /// <summary>
        /// Returns an instance of a Sphere
        /// </summary>
        public static Mesh Sphere
        {
            get
            {
                if (_sphere == null)
                {
                    _sphere = MeshLoader.FileToMesh("assets/models/sphere_smooth.obj");
                }

                return _sphere.Copy();
            }
        }

        /// <summary>
        /// Returns an instance of a Sphere
        /// </summary>
        public static Texture White
        {
            get
            {
                if (_white == null)
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    bmp.SetPixel(0, 0, Color.White);
                    _white = TextureLoader.BitmapToTexture(bmp);
                }

                return _white.Copy();
            }
        }
        /// <summary>
        /// Returns an instance of a Sphere
        /// </summary>
        public static Texture Black
        {
            get
            {
                if (_black == null)
                {
                    Bitmap bmp = new Bitmap(1,1);
                    _black = TextureLoader.BitmapToTexture(bmp);
                }

                return _black.Copy();
            }
        }

    }
}