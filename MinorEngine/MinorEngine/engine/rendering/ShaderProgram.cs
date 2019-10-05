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
            bool ret = true;
            program = new ShaderProgram();
            List<int> shaders = new List<int>();
            foreach (KeyValuePair<ShaderType, string> shader in subshaders)
            {
                Logger.Log("Compiling Shader: " + shader.Value, DebugChannel.Log);

                string code = TextProcessorAPI.PreprocessSource(shader.Value, null);
                bool r = TryCompileShader(shader.Key, code, out int id);
                ret &= r;
                if (r) shaders.Add(id);
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
            int loc = GL.GetAttribLocation(_prgId, name);
            return loc;
        }

        public int GetUniformLocation(string name)
        {
            int loc = GL.GetUniformLocation(_prgId, name);
            return loc;
        }


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