using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Engine.Core;
using Engine.Debug;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering
{
    /// <summary>
    /// Implements a Wrapper for Loading Building and compiling OpenGL Shaders
    /// </summary>
    public class ShaderProgram : IDisposable
    {
        /// <summary>
        /// The program id of the shader
        /// </summary>
        private readonly int _prgId;

        /// <summary>
        /// Private constructor
        /// </summary>
        private ShaderProgram()
        {
            _prgId = GL.CreateProgram();
        }

        /// <summary>
        /// Tries to Create a Shader from source
        /// </summary>
        /// <param name="subshaders">The source paths of the sub shader</param>
        /// <param name="program">The Program that will be created</param>
        /// <returns></returns>
        public static bool TryCreate(Dictionary<ShaderType, string> subshaders, out ShaderProgram program)
        {
            bool ret = true;
            program = new ShaderProgram();
            List<int> shaders = new List<int>();
            foreach (KeyValuePair<ShaderType, string> shader in subshaders)
            {
                Logger.Log("Compiling Shader: " + shader.Value, DebugChannel.Log);

                string code = TextProcessorAPI.PreprocessSource(shader.Value, null);
                bool r = TryCompileShader(shader.Key, code, out int id);
                ret &= r;
                if (r)
                {
                    shaders.Add(id);
                }
            }


            for (int i = 0; i < shaders.Count; i++)
            {
                Logger.Log("Attaching Shader to Program: " + subshaders.ElementAt(i), DebugChannel.Log);
                GL.AttachShader(program._prgId, shaders[i]);
            }

            Logger.Log("Linking Program...", DebugChannel.Log);
            GL.LinkProgram(program._prgId);

            GL.GetProgram(program._prgId, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                Logger.Log(GL.GetProgramInfoLog(program._prgId), DebugChannel.Error);
                return false;
            }

            return ret;
        }

        /// <summary>
        /// Disposable Implementation that frees the GL Shader memory once the shader is not longer in use
        /// </summary>
        public void Dispose()
        {
            GL.DeleteProgram(_prgId);
        }

        /// <summary>
        /// Sets the This Program as active
        /// </summary>
        public void Use()
        {
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

        /// <summary>
        /// Returns the Uniform location by name
        /// </summary>
        /// <param name="name">Name of the Uniform</param>
        /// <returns>Uniform Location</returns>
        public int GetUniformLocation(string name)
        {
            int loc = GL.GetUniformLocation(_prgId, name);
            return loc;
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
                Logger.Log(GL.GetShaderInfoLog(shaderID), DebugChannel.Error);

                return false;
            }

            return true;
        }
    }
}