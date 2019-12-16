using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

namespace Engine.Core
{
    /// <summary>
    /// Input Handler that is Used by the UI System can can be used to check what key is pressed.
    /// </summary>
    public static class Input
    {
        private static Dictionary<Key, bool> _keymap = new Dictionary<Key, bool>();
        private static GameWindow _wnd;
        /// <summary>
        /// Flag for the Right Mouse Button
        /// </summary>
        public static bool RightMb { get; private set; }
        /// <summary>
        /// Flag for the Left Mouse Button
        /// </summary>
        public static bool LeftMb { get; private set; }

        /// <summary>
        /// Initializes the Input Class with the Game Window to subscribe to all needed events
        /// </summary>
        /// <param name="window">window to initialize with</param>
        internal static void Initialize(GameWindow window)
        {
            window.KeyDown += Window_KeyDown;
            window.KeyUp += Window_KeyUp;
            window.MouseDown += WindowOnMouseInteraction;
            window.MouseUp += WindowOnMouseInteraction;
            _wnd = window;
            string[] keynames = Enum.GetNames(typeof(Key));
            for (int i = 0; i < keynames.Length; i++)
            {
                Key k = Enum.Parse<Key>(keynames[i]);
                if (!_keymap.ContainsKey(k))
                {
                    _keymap.Add(k, false);
                }
            }
        }

        private static void WindowOnMouseInteraction(object sender, MouseButtonEventArgs e)
        {
            RightMb = e.Mouse.RightButton == ButtonState.Pressed;
            LeftMb = e.Mouse.LeftButton == ButtonState.Pressed;
        }


        private static void Window_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            _keymap[e.Key] = false;
        }

        private static void Window_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            _keymap[e.Key] = true;
        }

        /// <summary>
        /// Returns if a specific key has been pressed
        /// </summary>
        /// <param name="key">the Key to be checked</param>
        /// <returns></returns>
        public static bool GetKey(Key key)
        {
            return _keymap[key];
        }
    }
}