using System.Drawing;
using Engine.IO;
using Xunit;

namespace Engine.Tests
{
    public class IOLoader
    {
        public IOLoader()
        {
            TestSetup.ApplyDebugSettings();
        }

        [Fact]
        public void LoadTexture()
        {
            byte[] buf = TextureLoader.BitmapToBytes((Bitmap) Image.FromFile("resources/WFCTiles/testtile.png"));

            for (int i = 0; i < buf.Length; i++)
            {
                System.Diagnostics.Debug.Assert(buf[i] == 255 || buf[i] == 0);
            }
        }
    }
}