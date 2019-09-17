using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.rendering
{
    public class ShaderProgram
    {
        private int _prgID;
        private ShaderProgram()
        {
            _prgID = GL.CreateProgram();
        }

        public static bool TryCreate(Dictionary<ShaderType, string> subshaders, out ShaderProgram program)
        {
            bool ret = true;
            program = new ShaderProgram();
            List<int> shaders = new List<int>();
            foreach (KeyValuePair<ShaderType, string> shader in subshaders)
            {

                program.Log("Compiling Shader: " + shader.Value, DebugChannel.Log);
                string code = File.ReadAllText(shader.Value);
                bool r = TryCompileShader(shader.Key, code, out int id);
                ret &= r;
                if (r)
                    shaders.Add(id);
            }



            for (int i = 0; i < shaders.Count; i++)
            {
                program.Log("Attaching Shader to Program: " + subshaders.ElementAt(i), DebugChannel.Log);
                GL.AttachShader(program._prgID, shaders[i]);
            }

            program.Log("Linking Program...", DebugChannel.Log);
            GL.LinkProgram(program._prgID);

            GL.GetProgram(program._prgID, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                program.Log(GL.GetProgramInfoLog(program._prgID), DebugChannel.Error);
                return false;
            }

            return ret;


        }

        public void Use()
        {
            GL.UseProgram(_prgID);
        }

        public int GetAttribLocation(string name)
        {
            int loc = GL.GetAttribLocation(_prgID, name);
            return loc;
        }

        public int GetUniformLocation(string name)
        {
            int loc = GL.GetUniformLocation(_prgID, name);
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

                shaderID.Log(GL.GetShaderInfoLog(shaderID), DebugChannel.Error);

                return false;
            }

            return true;
        }
    }
}