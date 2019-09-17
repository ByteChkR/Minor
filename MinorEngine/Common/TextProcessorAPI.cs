using System.Collections.Generic;
using System.Text;
using ext_pp;
using ext_pp_base;
using ext_pp_base.settings;
using ext_pp_plugins;

namespace Common
{
    public static class TextProcessorAPI
    {
        public static string[] PreprocessLines(string filename, Dictionary<string, bool> defs)
        {
            PreProcessor pp = new PreProcessor();


            List<AbstractPlugin> plugins = new List<AbstractPlugin>()
            {
                new FakeGenericsPlugin(),
                new IncludePlugin(),
                new ConditionalPlugin(),
                new ExceptionPlugin(),
                new MultiLinePlugin()
            };




            pp.SetFileProcessingChain(plugins);


            if (defs == null) defs = new Dictionary<string, bool>();

            return pp.Run(new[] { filename }, new Settings(), new Definitions(defs));
        }

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