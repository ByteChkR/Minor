using System.Collections.Generic;
using System.Linq;
using Common;
using MinorEngine.debug;
using MinorEngine.engine.core;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering
{
    public class ShaderProgram : IDestroyable
    {
        private readonly int _prgId;

        private ShaderProgram()
        {
            _prgId = GL.CreateProgram();
        }

        public static bool TryCreate(Dictionary<ShaderType, string> subshaders, out ShaderProgram program)
        {
            var ret = true;
            program = new ShaderProgram();
            var shaders = new List<int>();
            foreach (var shader in subshaders)
            {
                Logger.Log("Compiling Shader: " + shader.Value, DebugChannel.Log);

                var code = TextProcessorAPI.PreprocessSource(shader.Value, null);
                var r = TryCompileShader(shader.Key, code, out var id);
                ret &= r;
                if (r)
                {
                    shaders.Add(id);
                }
            }


            for (var i = 0; i < shaders.Count; i++)
            {
                Logger.Log("Attaching Shader to Program: " + subshaders.ElementAt(i), DebugChannel.Log);
                GL.AttachShader(program._prgId, shaders[i]);
            }

            Logger.Log("Linking Program...", DebugChannel.Log);
            GL.LinkProgram(program._prgId);

            GL.GetProgram(program._prgId, GetProgramParameterName.LinkStatus, out var success);
            if (success == 0)
            {
                Logger.Log(GL.GetProgramInfoLog(program._prgId), DebugChannel.Error);
                return false;
            }

            return ret;
        }

        public void Destroy()
        {
            GL.DeleteProgram(_prgId);
        }

        public void Use()
        {
            GL.UseProgram(_prgId);
        }

        public int GetAttribLocation(string name)
        {
            var loc = GL.GetAttribLocation(_prgId, name);
            return loc;
        }

        public int GetUniformLocation(string name)
        {
            var loc = GL.GetUniformLocation(_prgId, name);
            return loc;
        }


        private static bool TryCompileShader(ShaderType type, string source, out int shaderID)
        {
            shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, source);
            GL.CompileShader(shaderID);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out var success);
            if (success == 0)
            {
                Logger.Log(GL.GetShaderInfoLog(shaderID), DebugChannel.Error);

                return false;
            }

            return true;
        }
    }
}