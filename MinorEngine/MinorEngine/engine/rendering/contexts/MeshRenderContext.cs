using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.rendering.contexts
{


    public class MeshRenderContext : RenderContext
    {
        public GameMesh[] Meshes { get; }
        public MeshRenderContext(ShaderProgram program, Matrix4 modelMatrix, GameMesh[] meshes, Renderer.RenderType renderType) : base(program, modelMatrix, true, renderType, 0)
        {
            Meshes = meshes;

        }

        public override void Render(Matrix4 viewMat, Matrix4 projMat)
        {
            Program.Use();
            Matrix4 mat = ModelMat;
            GL.UniformMatrix4(Program.GetUniformLocation("modelMatrix"), false, ref mat);
            GL.UniformMatrix4(Program.GetUniformLocation("viewMatrix"), false, ref viewMat);
            GL.UniformMatrix4(Program.GetUniformLocation("projectionMatrix"), false, ref projMat);
            Matrix4 mvp = ModelMat * viewMat * projMat;
            GL.UniformMatrix4(Program.GetUniformLocation("mvpMatrix"), false, ref mvp);

            foreach (GameMesh gameMesh in Meshes) gameMesh.Draw(Program);
        }
    }
}