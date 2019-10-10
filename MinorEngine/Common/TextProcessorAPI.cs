using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ext_pp;
using ext_pp_base;
using ext_pp_base.settings;
using ext_pp_plugins;

namespace Common
{
    /// <summary>
    /// A static Wrapper class around the ext_pp project.
    /// </summary>
    public static class TextProcessorAPI
    {
        public class FileContent : IFileContent // For the commits on ext_pp repo that are not ready yet.
        {
            private readonly string[] _lines;
            private readonly string _incDir;
            private string Key => _incDir + "/memoryFile";
            private string Path => _incDir + "/memoryFile";

            public FileContent(string[] lines, string incDir)
            {
                _lines = lines;
                _incDir = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(incDir));
            }

            public bool TryGetLines(out string[] lines)
            {
                lines = _lines;
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

                string[] ret = { "FILE NOT FOUND" };
                try
                {
                    ret = pp.Run(new[] { filename }, new Settings(), definitions);
                }
                catch (ProcessorException ex)
                {
                    DebugHelper.Crash(new TextProcessingException("Could not preprocess file: " + filename.GetFilePath(), ex), true);
                }
                return ret;
            }
        }


        public class DefaultPreProcessorConfig : APreProcessorConfig
        {
            protected override Verbosity VerbosityLevel { get; } = Verbosity.SILENT;
            private static StringBuilder _sb = new StringBuilder();

            public override string GetGenericInclude(string filename, string[] genType)
            {
                _sb.Clear();
                foreach (string gt in genType)
                {
                    _sb.Append(gt);
                    _sb.Append(' ');
                }

                return "#include " + filename + " " + _sb;
            }

            protected override List<AbstractPlugin> Plugins =>
                new List<AbstractPlugin>
                {
                    new FakeGenericsPlugin(),
                    new IncludePlugin(),
                    new ConditionalPlugin(),
                    new ExceptionPlugin(),
                    new MultiLinePlugin()
                };
        }

        public class GLCLPreProcessorConfig : APreProcessorConfig
        {
            private static StringBuilder _sb = new StringBuilder();
            protected override Verbosity VerbosityLevel { get; } = Verbosity.SILENT;

            public override string GetGenericInclude(string filename, string[] genType)
            {
                _sb.Clear();
                foreach (string gt in genType)
                {
                    _sb.Append(gt);
                    _sb.Append(' ');
                }


                return "#include " + filename + " " + _sb;
            }

            protected override List<AbstractPlugin> Plugins =>
                new List<AbstractPlugin>
                {
                    new FakeGenericsPlugin(),
                    new IncludePlugin(),
                    new ConditionalPlugin(),
                    new ExceptionPlugin(),
                    new MultiLinePlugin()
                };
        }

        public class FLPreProcessorConfig : APreProcessorConfig
        {
            protected override Verbosity VerbosityLevel { get; } = Verbosity.SILENT;

            private static StringBuilder _sb = new StringBuilder();

            public override string GetGenericInclude(string filename, string[] genType)
            {
                _sb.Clear();
                foreach (string gt in genType)
                {
                    _sb.Append(gt);
                    _sb.Append(' ');
                }

                return "pp_include: " + filename + " " + _sb;
            }

            protected override List<AbstractPlugin> Plugins
            {
                get
                {
                    IncludePlugin inc = new IncludePlugin();
                    inc.IncludeInlineKeyword = "pp_includeinl:";
                    inc.IncludeKeyword = "pp_include:";
                    ConditionalPlugin cond = new ConditionalPlugin();
                    cond.StartCondition = "pp_if:";
                    cond.ElseIfCondition = "pp_elseif:";
                    cond.ElseCondition = "pp_else:";
                    cond.EndCondition = "pp_endif:";
                    cond.DefineKeyword = "pp_define:";
                    cond.UndefineKeyword = "pp_undefine:";

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
        }

        private static Dictionary<string, APreProcessorConfig> _configs = new Dictionary<string, APreProcessorConfig>
        {
            {".fl", new FLPreProcessorConfig()},
            {".vs", new GLCLPreProcessorConfig()},
            {".fs", new GLCLPreProcessorConfig()},
            {".cl", new GLCLPreProcessorConfig()},
            {"***", new DefaultPreProcessorConfig()}
        };

        public static string[] GenericIncludeToSource(string ext, string file, params string[] genType)
        {
            return new[] { _configs[ext].GetGenericInclude(file, genType) };
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
                DebugHelper.Log("Found Matching PreProcessor Config for: " + ext, 1);
                return _configs[ext].Preprocess(file, defs);
            }

            DebugHelper.Log("Loading File with Default PreProcessing", 1);
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
    }
}