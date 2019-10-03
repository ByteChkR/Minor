using System;
using System.Collections.Generic;
using System.Text;
using Common;
using MinorEngine.debug;
using MinorEngine.engine.rendering;
using MinorEngine.engine.ui.utils;

namespace MinorEngine.engine.core
{
    public static class TextureProvider
    {
        /// <summary>
        /// Dictionary of TextureID, Tuple of filename/name, GameTexture, RefCount
        /// </summary>
        private static Dictionary<int, Tuple<string, GameTexture, int>> _refs =
            new Dictionary<int, Tuple<string, GameTexture, int>>();

        private static StringBuilder _sb = new StringBuilder();

        private static string cmd_ListTex(string[] args)
        {
            _sb.Clear();
            _sb.Append("Loaded Textures:");
            foreach (var tuple in _refs) _sb.Append("\n" + tuple.Value.Item1 + ": " + tuple.Value.Item3);

            return _sb.ToString();
        }

        public static void AddConsoleCommands(DebugConsoleComponent comp)
        {
            comp.AddCommand("ltex", cmd_ListTex);
        }

        public static void Add(GameTexture tex, string name)
        {
            AddToDb(tex, name);
        }


        public static int IsLoaded(string path)
        {
            foreach (var tRef in _refs)
                if (tRef.Value.Item1 == path)
                    return tRef.Key;

            return -1;
        }

        private static int Create(string path)
        {
            GameTexture tex = GameTexture.Load(path);
            int id = tex.TextureId;


            _refs.Add(id, new Tuple<string, GameTexture, int>(path, tex, 1));

            return id;
        }

        private static void AddToDb(GameTexture tex, string name)
        {
            if (!_refs.ContainsKey(tex.TextureId))
                _refs.Add(tex.TextureId, new Tuple<string, GameTexture, int>(name, tex, 1));
        }

        public static void GiveBack(GameTexture tex)
        {
            if (_refs.ContainsKey(tex.TextureId))
            {
                tex.Log("Giving Back Texture ID: " + tex.TextureId, DebugChannel.Log);

                _refs[tex.TextureId] = new Tuple<string, GameTexture, int>(_refs[tex.TextureId].Item1,
                    _refs[tex.TextureId].Item2, _refs[tex.TextureId].Item3 - 1);
                if (_refs[tex.TextureId].Item3 == 0)
                {
                    _refs.Remove(tex.TextureId);
                    tex.Destroy();
                }
            }
            else
            {
                tex.Log("Handle of Unmanaged Texture " + tex.TextureId + " was destroyed.", DebugChannel.Log);
            }
        }


        public static GameTexture Load(string path)
        {
            int r = IsLoaded(path);
            if (r == -1)
                r = Create(path);
            else
                _refs[r] = new Tuple<string, GameTexture, int>(_refs[r].Item1, _refs[r].Item2, _refs[r].Item3 + 1);


            return _refs[r].Item2;
        }

        public static void DestroyAll()
        {
            foreach (var tuple in _refs) tuple.Value.Item2.Destroy();
        }
    }
}