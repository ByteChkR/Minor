using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ext_pp;
using ext_pp_base;
using ext_pp_base.settings;
using ext_pp_plugins;

namespace Engine.Common
{
    /// <summary>
    /// IO Callbacks for the IO Operations of the Text Processor
    /// </summary>
    public class PpCallbacks : IOCallbacks
    {
        public override bool FileExists(string file)
        {
            if (TextProcessorApi.PpCallback == null)
            {
                return base.FileExists(file);
            }

            string p = Path.GetRelativePath(Directory.GetCurrentDirectory(), file);
            return TextProcessorApi.PpCallback.FileExists(p);
        }

        public override string[] ReadAllLines(string file)
        {
            if (TextProcessorApi.PpCallback == null)
            {
                return base.ReadAllLines(file);
            }

            string p = Path.GetRelativePath(Directory.GetCurrentDirectory(), file);
            return TextProcessorApi.PpCallback.ReadAllLines(p);
        }

        public override string[] GetFiles(string path, string searchPattern = "*")
        {
            if (TextProcessorApi.PpCallback == null)
            {
                return base.GetFiles(path, searchPattern);
            }

            string p = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);
            return TextProcessorApi.PpCallback.GetFiles(p, searchPattern);
        }
    }

    /// <summary>
    /// A static Wrapper class around the ext_pp project.
    /// </summary>
    public static class TextProcessorApi
    {
        public static IIoCallback PpCallback = null;

        private static Dictionary<string, APreProcessorConfig> _configs = new Dictionary<string, APreProcessorConfig>
        {
            {".fl", new FlPreProcessorConfig()},
            {".vs", new GlclPreProcessorConfig()},
            {".fs", new GlclPreProcessorConfig()},
            {".cl", new GlclPreProcessorConfig()},
            {"***", new DefaultPreProcessorConfig()}
        };

        public static string[] GenericIncludeToSource(string ext, string file, params string[] genType)
        {
            return new[] {_configs[ext].GetGenericInclude(file, genType)};
        }

        public static string[] PreprocessLines(string filename, Dictionary<string, bool> defs)
        {
            return PreprocessLines(new FilePathContent(filename), defs);
        }

        public static string[] PreprocessLines(string[] lines, string incDir, Dictionary<string, bool> defs)
        {
            return PreprocessLines(new FileContent(lines, incDir), defs);
        }

        internal static string[] PreprocessLines(IFileContent file, Dictionary<string, bool> defs)
        {
            string ext = new string(file.GetFilePath().TakeLast(3).ToArray());
            if (_configs.ContainsKey(ext))
            {
                DebugHelper.Log("Found Matching PreProcessor Config for: " + ext, 1 | (1 << 21));
                return _configs[ext].Preprocess(file, defs);
            }

            DebugHelper.Log("Loading File with Default PreProcessing", 1 | (1 << 21));
            return _configs["***"].Preprocess(file, defs);
        }


        public static string PreprocessSource(string filename, Dictionary<string, bool> defs)
        {
            return PreprocessSource(new FilePathContent(filename), defs);
        }

        public static string PreprocessSource(string[] lines, string incDir, Dictionary<string, bool> defs)
        {
            return PreprocessSource(new FileContent(lines, incDir), defs);
        }


        /// <summary>
        /// Loads and preprocesses the file specified
        /// </summary>
        /// <param name="filename">the filepath</param>
        /// <param name="defs">definitions</param>
        /// <returns>the source as string</returns>
        internal static string PreprocessSource(IFileContent filename, Dictionary<string, bool> defs)
        {
            StringBuilder sb = new StringBuilder();
            string[] src = PreprocessLines(filename, defs);
            for (int i = 0; i < src.Length; i++)
            {
                sb.Append(src[i] + "\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// File Content that is used as an abstraction to files
        /// </summary>
        public class FileContent : IFileContent
        {
            private readonly string incDir;
            private readonly string[] lines;

            public FileContent(string[] lines, string incDir)
            {
                this.lines = lines;
                this.incDir = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(incDir));
            }

            private string Key => incDir + "/memoryFile";
            private string Path => incDir + "/memoryFile";
            public bool HasValidFilepath => false;

            public bool TryGetLines(out string[] lines)
            {
                lines = this.lines;
                return true;
            }

            public string GetKey()
            {
                return Key;
            }

            public void SetKey(string key)
            {
                //Nothing
            }

            public string GetFilePath()
            {
                return Path;
            }

            public override string ToString()
            {
                return Key;
            }
        }

        /// <summary>
        /// Abstract PreProcessor Configuration
        /// </summary>
        public abstract class APreProcessorConfig
        {
            protected abstract Verbosity VerbosityLevel { get; }
            protected abstract List<AbstractPlugin> Plugins { get; }
            public abstract string GetGenericInclude(string filename, string[] genType);

            public string[] Preprocess(IFileContent filename, Dictionary<string, bool> defs)
            {
                PreProcessor pp = new PreProcessor();

                Logger.VerbosityLevel = VerbosityLevel;


                pp.SetFileProcessingChain(Plugins);

                Definitions definitions;
                if (defs == null)
                {
                    definitions = new Definitions();
                }
                else
                {
                    definitions = new Definitions(defs);
                }

                string[] ret = {"FILE NOT FOUND"};
                try
                {
                    ret = pp.Run(new[] {filename}, new Settings(), definitions);
                }
                catch (ProcessorException ex)
                {
                    DebugHelper.Crash(
                        new TextProcessingException("Could not preprocess file: " + filename.GetFilePath(), ex), true);
                }

                return ret;
            }
        }

        /// <summary>
        /// The Default PreProcessor Configuration
        /// </summary>
        public class DefaultPreProcessorConfig : APreProcessorConfig
        {
            private static StringBuilder _sb = new StringBuilder();
            protected override Verbosity VerbosityLevel { get; } = Verbosity.SILENT;

            protected override List<AbstractPlugin> Plugins =>
                new List<AbstractPlugin>
                {
                    new FakeGenericsPlugin(),
                    new IncludePlugin(),
                    new ConditionalPlugin(),
                    new ExceptionPlugin(),
                    new MultiLinePlugin()
                };

            public override string GetGenericInclude(string filename, string[] genType)
            {
                _sb.Append(" ");
                foreach (string gt in genType)
                {
                    _sb.Append(gt);
                    _sb.Append(' ');
                }

                string gens = _sb.Length == 0 ? "" : _sb.ToString();
                return "#include " + filename + " " + gens;
            }
        }
        /// <summary>
        /// The PreProcessor Configuration used for OpenGL and OpenCL files
        /// </summary>
        public class GlclPreProcessorConfig : APreProcessorConfig
        {
            private static StringBuilder _sb = new StringBuilder();
            protected override Verbosity VerbosityLevel { get; } = Verbosity.SILENT;

            protected override List<AbstractPlugin> Plugins =>
                new List<AbstractPlugin>
                {
                    new FakeGenericsPlugin(),
                    new IncludePlugin(),
                    new ConditionalPlugin(),
                    new ExceptionPlugin(),
                    new MultiLinePlugin()
                };

            public override string GetGenericInclude(string filename, string[] genType)
            {
                _sb.Clear();
                foreach (string gt in genType)
                {
                    _sb.Append(gt);
                    _sb.Append(' ');
                }


                string gens = _sb.Length == 0 ? "" : _sb.ToString();
                return "#include " + filename + " " + gens;
            }
        }

        /// <summary>
        /// The PreProcessor Configuration used for OpenFL files
        /// </summary>
        public class FlPreProcessorConfig : APreProcessorConfig
        {
            private static StringBuilder _sb = new StringBuilder();
            protected override Verbosity VerbosityLevel { get; } = Verbosity.SILENT;

            protected override List<AbstractPlugin> Plugins
            {
                get
                {
                    IncludePlugin inc = new IncludePlugin
                    {
                        IncludeInlineKeyword = "pp_includeinl:", IncludeKeyword = "pp_include:"
                    };
                    ConditionalPlugin cond = new ConditionalPlugin
                    {
                        StartCondition = "pp_if:",
                        ElseIfCondition = "pp_elseif:",
                        ElseCondition = "pp_else:",
                        EndCondition = "pp_endif:",
                        DefineKeyword = "pp_define:",
                        UndefineKeyword = "pp_undefine:"
                    };

                    return new List<AbstractPlugin>
                    {
                        new FakeGenericsPlugin(),
                        inc,
                        cond,
                        new ExceptionPlugin(),
                        new MultiLinePlugin()
                    };
                }
            }

            public override string GetGenericInclude(string filename, string[] genType)
            {
                _sb.Clear();
                foreach (string gt in genType)
                {
                    _sb.Append(gt);
                    _sb.Append(' ');
                }

                string gens = _sb.Length == 0 ? "" : _sb.ToString();
                return "#pp_include: " + filename + " " + gens;
            }
        }
    }
}