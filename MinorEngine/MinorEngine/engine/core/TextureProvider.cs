using System;

using System.Collections.Generic;
using System.Text;
using GameEngine.engine.rendering;
using GameEngine.engine.ui.utils;

namespace GameEngine.engine.core
{
    public static class TextureProvider
    {
        private static Dictionary<int, Tuple<string, GameTexture, int>> _refs =
            new Dictionary<int, Tuple<string, GameTexture, int>>();

        private static StringBuilder _sb=new StringBuilder();

        private static string cmd_ListTex(string[] args)
        {
            _sb.Clear();
            _sb.Append("Loaded Textures:");
            foreach (var tuple in _refs)
            {
                _sb.Append("\n" + tuple.Value.Item1 + ": " + tuple.Value.Item3);
            }

            return _sb.ToString();
        }

        public static void AddConsoleCommands(DebugConsoleComponent comp)
        {
            comp.AddCommand("ltex", cmd_ListTex);
        }




        public static int IsLoaded(string path)
        {
            foreach (var tRef in _refs)
            {
                if (tRef.Value.Item1 == path)
                {
                    return tRef.Key;
                }
            }

            return -1;
        }

        private static int Create(string path)
        {
            GameTexture tex = GameTexture.Load(path);
            int id = tex.TextureId;

            _refs.Add(id, new Tuple<string, GameTexture, int>(path, tex, 1));
            return id;
        }

        internal static void AddToDb(GameTexture tex)
        {
            if (!_refs.ContainsKey(tex.TextureId))
            {

                _refs.Add(tex.TextureId, new Tuple<string, GameTexture, int>("Memory", tex, 1));

            }
            else
            {
                _refs[tex.TextureId] = new Tuple<string, GameTexture, int>("Memory", tex, 1);

            }
        }

        public static void GiveBack(GameTexture tex)
        {
            if (_refs.ContainsKey(tex.TextureId))
            {
                _refs[tex.TextureId] = new Tuple<string, GameTexture, int>(_refs[tex.TextureId].Item1,
                    _refs[tex.TextureId].Item2, _refs[tex.TextureId].Item3 - 1);
                if (_refs[tex.TextureId].Item3 == 0)
                {
                    _refs.Remove(tex.TextureId);
                    tex.Destroy();
                }
            }
        }

        public static GameTexture Load(string path)
        {
            int r = IsLoaded(path);
            if (r == -1)
            {
                r = Create(path);
            }

            return _refs[r].Item2;
        }

        public static void DestroyAll()
        {
            foreach (var tuple in _refs)
            {
                tuple.Value.Item2.Destroy();
            }
        }
    }
}