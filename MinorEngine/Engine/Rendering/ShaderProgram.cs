﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Common;
using Engine.Core;
using Engine.DataTypes;
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
        private static ShaderProgram _defaultShader;
        public static ShaderProgram DefaultShader => _defaultShader ?? (_defaultShader = GetDefaultShader());

        private static ShaderProgram GetDefaultShader()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] paths = {"._DefaultResources.DefaultShader.vs", "._DefaultResources.DefaultShader.fs"};
            Dictionary<ShaderType, string> shaderSource = new Dictionary<ShaderType, string>();
            for (int i = 0; i < paths.Length; i++)
            {
                using (Stream resourceStream = asm.GetManifestResourceStream(asm.GetName().Name + paths[i]))
                {
                    if (resourceStream == null)
                    {
                        Logger.Crash(new EngineException("Could not load default shader"), false);
                        return null;
                    }

                    TextReader tr = new StreamReader(resourceStream);
                    string source = tr.ReadToEnd();

                    resourceStream.Close();
                    shaderSource.Add(i==0?ShaderType.VertexShader: ShaderType.FragmentShader, source);

                }
            }

            if (TryCreateFromSource(shaderSource, out var shader))
            {
                return shader;
            }

            Logger.Crash(new EngineException("Could not compile default shader"), false);
            return null;

        }

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

        internal static bool TryCreateFromSource(Dictionary<ShaderType, string> subshaders, out ShaderProgram program)
        {
            bool ret = true;
            program = new ShaderProgram();
            List<int> shaders = new List<int>();
            foreach (KeyValuePair<ShaderType, string> shader in subshaders)
            {
                Logger.Log("Compiling Shader: " + shader.Key, DebugChannel.Log);
                
                bool r = TryCompileShader(shader.Key, shader.Value, out int id);
                ret &= r;
                if (r)
                {
                    shaders.Add(id);
                }
            }


            for (int i = 0; i < shaders.Count; i++)
            {
                Logger.Log("Attaching Shader to Program: " + subshaders.ElementAt(i).Key, DebugChannel.Log);
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
        /// Tries to Create a Shader from source
        /// </summary>
        /// <param name="subshaders">The source paths of the sub shader</param>
        /// <param name="program">The Program that will be created</param>
        /// <returns></returns>
        public static bool TryCreate(Dictionary<ShaderType, string> subshaders, out ShaderProgram program)
        {
            Dictionary<ShaderType, string> ret = new Dictionary<ShaderType, string>();
            foreach (var subshader in subshaders)
            {
                Logger.Log("Loading Shader: " + subshader.Value, DebugChannel.Log);
                ret.Add(subshader.Key, TextProcessorAPI.PreprocessSource(subshader.Value, null));
            }

            return TryCreateFromSource(ret, out program);
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