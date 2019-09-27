using System.Collections.Generic;
using System.IO;
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

        public abstract class APreProcessorConfig
        {
            protected abstract Verbosity VerbosityLevel { get; }
            protected abstract List<AbstractPlugin> Plugins { get; }
            
            public string[] Preprocess(string filename, Dictionary<string, bool> defs)
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

                return pp.Run(new[] { filename }, new Settings(), definitions);
            }
        }

        public class GLCLPreProcessorConfig : TextProcessorAPI.APreProcessorConfig
        {
            protected override Verbosity VerbosityLevel { get; } = Verbosity.LEVEL2;

            protected override List<AbstractPlugin> Plugins
            {
                get
                {
                    return new List<AbstractPlugin>
                    {
                        new FakeGenericsPlugin(),
                        new IncludePlugin(),
                        new ConditionalPlugin(),
                        new ExceptionPlugin(),
                        new MultiLinePlugin()
                    };
                }
            }
        }

        public class FLPreProcessorConfig : TextProcessorAPI.APreProcessorConfig
        {
            protected override Verbosity VerbosityLevel { get; } = Verbosity.LEVEL2;


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
            {".fl", new FLPreProcessorConfig() },
            {".vs", new GLCLPreProcessorConfig() },
            {".fs", new GLCLPreProcessorConfig() },
            {".cl", new GLCLPreProcessorConfig() },

        };
        /// <summary>
        /// Loads and preprocesses the file specified
        /// </summary>
        /// <param name="filename">the filepath</param>
        /// <param name="defs">definitions</param>
        /// <returns>the source in lines</returns>
        public static string[] PreprocessLines(string filename, Dictionary<string, bool> defs)
        {
            string ext = new string(filename.TakeLast(3).ToArray());
            if (_configs.ContainsKey(ext))
            {
                filename.Log("Found Matching PreProcessor Config for: " + ext, DebugChannel.Log);
                return _configs[ext].Preprocess(filename, defs);
            }
            filename.Log("Loading File without PreProcessing", DebugChannel.Log);
            return File.ReadAllLines(filename);

        }

        /// <summary>
        /// Loads and preprocesses the file specified
        /// </summary>
        /// <param name="filename">the filepath</param>
        /// <param name="defs">definitions</param>
        /// <returns>the source as string</returns>
        public static string PreprocessSource(string filename, Dictionary<string, bool> defs)
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