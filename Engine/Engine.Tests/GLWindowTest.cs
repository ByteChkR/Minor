using System;
using Engine.Audio;
using Engine.OpenCL;
using OpenTK;
using OpenTK.Graphics;
using Xunit;

namespace Engine.Tests
{
    public static class GLWindowTest
    {
        [Fact]
        public static void WindowTest()
        {
            GameWindow wnd = new GameWindow(100, 100, GraphicsMode.Default, "TEST");
            wnd.MakeCurrent();
            wnd.Dispose();
        }
    }
}