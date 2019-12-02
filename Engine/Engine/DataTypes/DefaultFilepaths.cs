using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using Engine.Rendering;
using Engine.UI;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    [Serializable]
    internal struct ShaderPath
    {
        public ShaderType Type;
        public string Path;
    }

    [Serializable]
    public class DefaultFilepaths
    {
        private static DefaultFilepaths FilePaths = new DefaultFilepaths();


        /// <summary>
        /// Backing field for the default font
        /// </summary>
        private static GameFont _defaultFont;

        /// <summary>
        /// The Backing field of the default mesh
        /// </summary>
        private static Mesh _defaultMesh;

        /// <summary>
        /// Backing field for the default texture
        /// </summary>
        private static Texture _defaultTexture;


        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultUnlitShader;

        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultLitShader;

        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultUITextShader;

        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultUIImageShader;

        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultUIGraphShader;

        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultMergeMulShader;

        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultMergeAddShader;

        /// <summary>
        /// Backing field of the Default shader
        /// </summary>
        private static ShaderProgram _defaultScreenShader;

        private string _DefaultFont = "assets/fonts/default_font.ttf";


        private List<ShaderPath> _DefaultLitShader = new List<ShaderPath>
        {
            new ShaderPath {Type = ShaderType.FragmentShader, Path = "assets/shader/lit/shader.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/lit/shader.vs"}
        };

        private List<ShaderPath> _DefaultMergeAddShader = new List<ShaderPath>
        {
            new ShaderPath
                {Type = ShaderType.FragmentShader, Path = "assets/shader/internal/merge_stage/merge_shader_add.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/internal/merge_stage/merge_shader.vs"}
        };

        private List<ShaderPath> _DefaultMergeMulShader = new List<ShaderPath>
        {
            new ShaderPath
                {Type = ShaderType.FragmentShader, Path = "assets/shader/internal/merge_stage/merge_shader_mul.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/internal/merge_stage/merge_shader.vs"}
        };

        private string _DefaultMesh = "assets/models/default_mesh.obj";

        private List<ShaderPath> _DefaultScreenShader = new List<ShaderPath>
        {
            new ShaderPath {Type = ShaderType.FragmentShader, Path = "assets/shader/internal/screen_stage/shader.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/internal/screen_stage/shader.vs"}
        };

        private string _DefaultTexture = "assets/textures/default_texture.bmp";

        private List<ShaderPath> _DefaultUIGraphShader = new List<ShaderPath>
        {
            new ShaderPath {Type = ShaderType.FragmentShader, Path = "assets/shader/ui/graph/shader.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/ui/graph/shader.vs"}
        };

        private List<ShaderPath> _DefaultUIImageShader = new List<ShaderPath>
        {
            new ShaderPath {Type = ShaderType.FragmentShader, Path = "assets/shader/ui/image/shader.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/ui/image/shader.vs"}
        };

        private List<ShaderPath> _DefaultUITextShader = new List<ShaderPath>
        {
            new ShaderPath {Type = ShaderType.FragmentShader, Path = "assets/shader/ui/text/shader.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/ui/text/shader.vs"}
        };

        private List<ShaderPath> _DefaultUnlitShader = new List<ShaderPath>
        {
            new ShaderPath {Type = ShaderType.FragmentShader, Path = "assets/shader/unlit/shader.fs"},
            new ShaderPath {Type = ShaderType.VertexShader, Path = "assets/shader/unlit/shader.vs"}
        };

        private static Dictionary<ShaderType, string> DefaultLitShaderPath =>
            GetDictionary(FilePaths._DefaultLitShader);

        private static Dictionary<ShaderType, string> DefaultUnlitShaderPath =>
            GetDictionary(FilePaths._DefaultUnlitShader);

        private static Dictionary<ShaderType, string> DefaultUITextShaderPath =>
            GetDictionary(FilePaths._DefaultUITextShader);

        private static Dictionary<ShaderType, string> DefaultUIImageShaderPath =>
            GetDictionary(FilePaths._DefaultUIImageShader);

        private static Dictionary<ShaderType, string> DefaultUIGraphShaderPath =>
            GetDictionary(FilePaths._DefaultUIGraphShader);

        private static Dictionary<ShaderType, string> DefaultMergeAddShaderPath =>
            GetDictionary(FilePaths._DefaultMergeAddShader);

        private static Dictionary<ShaderType, string> DefaultMergeMulShaderPath =>
            GetDictionary(FilePaths._DefaultMergeMulShader);

        private static Dictionary<ShaderType, string> DefaultScreenShaderPath =>
            GetDictionary(FilePaths._DefaultScreenShader);

        public static string DefaultMeshPath => FilePaths._DefaultMesh;
        public static string DefaultTexturePath => FilePaths._DefaultTexture;
        public static string DefaultFontPath => FilePaths._DefaultFont;

        /// <summary>
        /// The default font
        /// </summary>
        public static GameFont DefaultFont => _defaultFont ?? (_defaultFont = GetDefaultFont());

        /// <summary>
        /// The Default Mesh
        /// </summary>
        public static Mesh DefaultMesh => _defaultMesh ?? (_defaultMesh = GetDefaultMesh());

        public static Texture DefaultTexture => _defaultTexture ?? (_defaultTexture = GetDefaultTexture());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultUnlitShader =>
            _defaultUnlitShader ?? (_defaultUnlitShader = GetDefaultUnlitShader());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultLitShader =>
            _defaultLitShader ?? (_defaultLitShader = GetDefaultLitShader());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultUITextShader =>
            _defaultUITextShader ?? (_defaultUITextShader = GetDefaultUITextShader());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultUIImageShader =>
            _defaultUIImageShader ?? (_defaultUIImageShader = GetDefaultUIImageShader());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultUIGraphShader =>
            _defaultUIGraphShader ?? (_defaultUIGraphShader = GetDefaultUIGraphShader());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultMergeMulShader =>
            _defaultMergeMulShader ?? (_defaultMergeMulShader = GetDefaultMergeMulShader());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultMergeAddShader =>
            _defaultMergeAddShader ?? (_defaultMergeAddShader = GetDefaultMergeAddShader());

        /// <summary>
        /// The default shader
        /// </summary>
        public static ShaderProgram DefaultScreenShader =>
            _defaultScreenShader ?? (_defaultScreenShader = GetDefaultScreenShader());

        private static Dictionary<ShaderType, string> GetDictionary(List<ShaderPath> list)
        {
            Dictionary<ShaderType, string> ret = new Dictionary<ShaderType, string>();
            for (int i = 0; i < list.Count; i++)
            {
                ret.Add(list[i].Type, list[i].Path);
            }

            return ret;
        }

        public static void Load(string path)
        {
            if (!IOManager.Exists(path))
            {
                Logger.Crash(new EngineException("Could not load DefaultFilePaths: " + path), false);
                return;
            }

            XmlSerializer xs = new XmlSerializer(typeof(DefaultFilepaths));
            Stream s = IOManager.GetStream(path);
            FilePaths = (DefaultFilepaths) xs.Deserialize(s);
            s.Close();
        }

        public static void Save(string path)
        {
            if (File.Exists(path))
            {
                Logger.Log("Warning, overwriting file: " + path, DebugChannel.Warning, 10);
                File.Delete(path);
            }

            XmlSerializer xs = new XmlSerializer(typeof(DefaultFilepaths));
            Stream s = File.Open(path, FileMode.Create);
            xs.Serialize(s, FilePaths);
            s.Close();
        }

        /// <summary>
        /// Creates the default font from embedded program resources
        /// </summary>
        /// <returns>The Default font</returns>
        private static GameFont GetDefaultFont()
        {
            return FontLibrary.LoadFontInternal(IOManager.GetStream(DefaultFontPath), 32, out string name);
        }

        /// <summary>
        /// Creates the default mesh from embedded program resources
        /// </summary>
        /// <returns>The Default mesh</returns>
        private static Mesh GetDefaultMesh()
        {
            return MeshLoader.LoadModel(IOManager.GetStream(DefaultMeshPath), Path.GetExtension(DefaultMeshPath))[0];
        }

        /// <summary>
        /// Creates the default Texture from embedded program resources
        /// </summary>
        /// <returns>The Default Texture</returns>
        private static Texture GetDefaultTexture()
        {
            return TextureLoader.BitmapToTexture(new Bitmap(IOManager.GetStream(DefaultTexturePath)));
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultUnlitShader()
        {
            ShaderProgram.TryCreate(DefaultUnlitShaderPath, out ShaderProgram shader);
            return shader;
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultLitShader()
        {
            ShaderProgram.TryCreate(DefaultLitShaderPath, out ShaderProgram shader);
            return shader;
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultUITextShader()
        {
            ShaderProgram.TryCreate(DefaultUITextShaderPath, out ShaderProgram shader);
            return shader;
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultUIImageShader()
        {
            ShaderProgram.TryCreate(DefaultUIImageShaderPath, out ShaderProgram shader);
            return shader;
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultUIGraphShader()
        {
            ShaderProgram.TryCreate(DefaultUIGraphShaderPath, out ShaderProgram shader);
            return shader;
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultMergeMulShader()
        {
            ShaderProgram.TryCreate(DefaultMergeMulShaderPath, out ShaderProgram shader);
            return shader;
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultMergeAddShader()
        {
            ShaderProgram.TryCreate(DefaultMergeAddShaderPath, out ShaderProgram shader);
            return shader;
        }

        /// <summary>
        /// Creates the default Shader from embedded program resources
        /// </summary>
        /// <returns>The Default Shader</returns>
        private static ShaderProgram GetDefaultScreenShader()
        {
            ShaderProgram.TryCreate(DefaultScreenShaderPath, out ShaderProgram shader);
            return shader;
        }
    }
}