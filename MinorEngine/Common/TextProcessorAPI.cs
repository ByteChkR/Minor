using System.Collections.Generic;
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

        /// <summary>
        /// Loads and preprocesses the file specified
        /// </summary>
        /// <param name="filename">the filepath</param>
        /// <param name="defs">definitions</param>
        /// <returns>the source in lines</returns>
        public static string[] PreprocessLines(string filename, Dictionary<string, bool> defs)
        {

            Logger.VerbosityLevel = Verbosity.LEVEL2;
            
            

            PreProcessor pp = new PreProcessor();


            List<AbstractPlugin> plugins = new List<AbstractPlugin>
            {
                new FakeGenericsPlugin(),
                new IncludePlugin(),
                new ConditionalPlugin(),
                new ExceptionPlugin(),
                new MultiLinePlugin()
            };



            pp.SetFileProcessingChain(plugins);

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