using System.Drawing;
using Engine.DataTypes;
using Engine.IO;
using Engine.Rendering;
using Xunit;

namespace Engine.Tests
{
    public class IOLoader
    {

        [Fact]
        public void LoadTexture()
        {
            byte[] buf = TextureLoader.BitmapToBytes((Bitmap)Image.FromFile("resources/testtile.png"));

            for (int i = 0; i < buf.Length; i++)
            {
                System.Diagnostics.Debug.Assert(buf[i] == 255 || buf[i] == 0);
            }
        }
        

    }
}