using System.Drawing;
using Engine.IO;
using Xunit;

namespace Engine.Tests
{
    public class IoLoader
    {
        public IoLoader()
        {
            TestSetup.ApplyDebugSettings();
        }

        [Fact]
        public void LoadTexture()
        {
            byte[] buf = TextureLoader.BitmapToBytes((Bitmap) Image.FromFile("resources/WFCTiles/testtile.png"));

            for (int i = 0; i < buf.Length; i++)
            {
                Assert.True(buf[i] == 255 || buf[i] == 0);
            }
        }
    }
}