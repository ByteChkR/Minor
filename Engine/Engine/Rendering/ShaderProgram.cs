using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Common;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    /// <summary>
    /// Implements a Wrapper for Loading Building and compiling OpenGL Shaders
    /// </summary>
    public class ShaderProgram : IDisposable
    {
        private static int _lastUsedPrgID = -1;


        /// <summary>
        /// The program id of the shader
        /// </summary>
        private readonly int _prgId;

        private readonly Dictionary<string, int> uniformCache = new Dictionary<string, int>();

        /// <summary>
        /// Private constructor
        /// </summary>
        private ShaderProgram()
        {
            _prgId = GL.CreateProgram();
        }

        /// <summary>
        /// Disposable Implementation that frees the GL Shader memory once the shader is not longer in use
        /// </summary>
        public void Dispose()
        {
            GL.DeleteProgram(_prgId);
        }

        internal static bool TryCreateFromSource(Dictionary<ShaderType, string> subshaders, out ShaderProgram program)
        {
            bool ret = true;
            program = new ShaderProgram();
            List<int> shaders = new List<int>();
            foreach (KeyValuePair<ShaderType, string> shader in subshaders)
            {
                Logger.Log("Compiling Shader: " + shader.Key, DebugChannel.Log | DebugChannel.EngineRendering, 5);

                bool r = TryCompileShader(shader.Key, shader.Value, out int id);
                ret &= r;
                if (r)
                {
                    shaders.Add(id);
                }
            }


            for (int i = 0; i < shaders.Count; i++)
            {
                Logger.Log("Attaching Shader to Program: " + subshaders.ElementAt(i).Key,
                    DebugChannel.Log | DebugChannel.EngineRendering, 6);
                GL.AttachShader(program._prgId, shaders[i]);
            }

            Logger.Log("Linking Program...", DebugChannel.Log | DebugChannel.EngineRendering, 5);
            GL.LinkProgram(program._prgId);

            GL.GetProgram(program._prgId, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                Logger.Crash(new OpenGLShaderException(GL.GetProgramInfoLog(program._prgId)), true);
                return false;
            }

            return ret;
        }

        /// <summary>
        /// Tries to Create a Shader from source
        /// </summary>
        /// <param name="subshaders">The source paths of the sub shader</param>
        /// <param name="program">The Program that will be created</param>
        /// <returns></returns>
        public static bool TryCreate(Dictionary<ShaderType, string> subshaders, out ShaderProgram program)
        {
            Dictionary<ShaderType, string> ret = new Dictionary<ShaderType, string>();
            foreach (KeyValuePair<ShaderType, string> subshader in subshaders)
            {
                Logger.Log("Loading Shader: " + subshader.Value, DebugChannel.Log | DebugChannel.EngineRendering, 7);
                Stream s = IOManager.GetStream(subshader.Value);
                TextReader tr = new StreamReader(s);
                string dirName = Path.GetDirectoryName(subshader.Value);
                string src = "";
                string[] lines = TextProcessorAPI.PreprocessLines(tr.ReadToEnd().Split('\n'), dirName, null);
                tr.Close();
                for (int i = 0; i < lines.Length; i++)
                {
                    src += lines[i] + '\n';
                }

                ret.Add(subshader.Key, src);
            }

            return TryCreateFromSource(ret, out program);
        }

        /// <summary>
        /// Sets the This Program as active
        /// </summary>
        public void Use()
        {
            if (_lastUsedPrgID == _prgId)
            {
                return;
            }

            _lastUsedPrgID = _prgId;
            GL.UseProgram(_prgId);
        }

        /// <summary>
        /// Returns the Attribute location by name
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <returns>Attribute Location</returns>
        public int GetAttribLocation(string name)
        {
            int loc = GL.GetAttribLocation(_prgId, name);
            return loc;
        }

        public void AddUniformCache(string name)
        {
            if (uniformCache.ContainsKey(name))
            {
                return;
            }

            int loc = GL.GetUniformLocation(_prgId, name);
            uniformCache.Add(name, loc);
        }

        /// <summary>
        /// Returns the Uniform location by name
        /// </summary>
        /// <param name="name">Name of the Uniform</param>
        /// <returns>Uniform Location</returns>
        public int GetUniformLocation(string name)
        {
            return uniformCache[name];
            //int loc = GL.GetUniformLocation(_prgId, name);
            //uniformCache.Add(name, loc);
            //return loc;
        }

        public int GetUniformLocationUncached(string name)

        {
            return GL.GetUniformLocation(_prgId, name);
        }

        /// <summary>
        /// Tries to compile a Shader from source
        /// </summary>
        /// <param name="type">The shader type</param>
        /// <param name="source">The source</param>
        /// <param name="shaderID">the Returned shader Handle</param>
        /// <returns>False if there were compile errors</returns>
        private static bool TryCompileShader(ShaderType type, string source, out int shaderID)
        {
            shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, source);
            GL.CompileShader(shaderID);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                Logger.Crash(new OpenGLShaderException(GL.GetShaderInfoLog(shaderID)), true);

                return false;
            }

            return true;
        }
    }
}