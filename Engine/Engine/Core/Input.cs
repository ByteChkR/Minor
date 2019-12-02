using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

namespace Engine.Core
{
    public static class Input
    {
        private static Dictionary<Key, bool> keymap = new Dictionary<Key, bool>();
        private static GameWindow wnd;
        public static bool RightMB { get; private set; }
        public static bool LeftMB { get; private set; }


        internal static void Initialize(GameWindow window)
        {
            window.KeyDown += Window_KeyDown;
            window.KeyUp += Window_KeyUp;
            window.MouseDown += WindowOnMouseInteraction;
            window.MouseUp += WindowOnMouseInteraction;
            wnd = window;
            string[] keynames = Enum.GetNames(typeof(Key));
            for (int i = 0; i < keynames.Length; i++)
            {
                Key k = Enum.Parse<Key>(keynames[i]);
                if (!keymap.ContainsKey(k))
                {
                    keymap.Add(k, false);
                }
            }
        }

        private static void WindowOnMouseInteraction(object sender, MouseButtonEventArgs e)
        {
            RightMB = e.Mouse.RightButton == ButtonState.Pressed;
            LeftMB = e.Mouse.LeftButton == ButtonState.Pressed;
        }


        private static void Window_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            keymap[e.Key] = false;
        }

        private static void Window_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            keymap[e.Key] = true;
        }

        public static bool GetKey(Key key)
        {
            return keymap[key];
        }
    }
}