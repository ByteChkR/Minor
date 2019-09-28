using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLHelperLibrary.CLStructs;
using Common;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Programs;

namespace CLHelperLibrary
{
    /// <summary>
    /// A wrapper class for a OpenCL Program.
    /// </summary>
    public class CLProgram
    {
        /// <summary>
        /// The filepath of the program source
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The gentype the program source is compiled for
        /// </summary>
        private readonly string _genType;

        /// <summary>
        /// The kernels that are contained in the Program
        /// </summary>
        public Dictionary<string, CLKernel> ContainedKernels { get; }

        /// <summary>
        /// The Compiled OpenCL Program
        /// </summary>
        public Program ClProgramHandle { get; set; }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="FilePath">The FilePath where the source is located</param>
        public CLProgram(string FilePath, string genType)
        {
            this._filePath = FilePath;
            _genType = genType;

            ContainedKernels = new Dictionary<string, CLKernel>();

            Initialize();
        }

        private int GetVectorNum(string dtStr)
        {
            if (!char.IsNumber(dtStr.Last())) return 1;
            else if (dtStr.Last() == '2') return 2;
            else if (dtStr.Last() == '3') return 3;
            else if (dtStr.Last() == '4') return 4;
            else if (dtStr.Last() == '8') return 8;
            else if (dtStr.Last() == '6') return 16;
            return 0;
        }

        /// <summary>
        /// Loads the source and initializes the CLProgram
        /// </summary>
        private void Initialize()
        {



#if PP_CompileCheck

            string[] dt = Enum.GetNames(typeof(DataTypes));

            string[] kernelNames = new string[0];
            string source = "";
            for (int i = 0; i < dt.Length; i++)
            {
                string dtStr = KernelParameter.GetDataString(Enum.Parse<DataTypes>(dt[i]));
                if (dtStr == "UNKNOWN") continue;
                string s = (_filePath + "." + dt[i] + ".cl").Replace("kernel", "kernel_cache");
                int vnum = GetVectorNum(dtStr);
                string[] lines = TextProcessorAPI.GenericIncludeToSource(".cl", _filePath, dtStr, vnum == 0 || vnum == 1 ? "float" : "float" + vnum);
                Dictionary<string, bool> defs = new Dictionary<string, bool> { { "V_" + vnum, true } };
                source = TextProcessorAPI.PreprocessSource(lines, _filePath, defs);
                kernelNames = FindKernelNames(source);
                File.WriteAllText(s, source);

                ClProgramHandle = CL.CreateCLProgramFromSource(source);
            }
            Console.WriteLine("Finished file.");


#else

            int vnum = GetVectorNum(_genType);
            string[] lines = TextProcessorAPI.GenericIncludeToSource(".cl", _filePath, _genType, vnum == 0 || vnum == 1 ? "float" : "float" + vnum);
            Dictionary<string, bool> defs = new Dictionary<string, bool> { { "V_" + vnum, true } };
            string source = TextProcessorAPI.PreprocessSource(lines, _filePath, defs);
            string[] kernelNames = FindKernelNames(source);
            
            ClProgramHandle = CL.CreateCLProgramFromSource(source);
#endif


            foreach (string kernelName in kernelNames)
            {

                Kernel k = CL.CreateKernelFromName(ClProgramHandle, kernelName);
                int kernelNameIndex = source.IndexOf(" " + kernelName + " ", StringComparison.InvariantCulture);
                kernelNameIndex = (kernelNameIndex == -1) ? source.IndexOf(" " + kernelName + "(", StringComparison.InvariantCulture) : kernelNameIndex;
                KernelParameter[] parameter = KernelParameter.CreateKernelParametersFromKernelCode(source,
                    kernelNameIndex,
                    source.Substring(kernelNameIndex, source.Length - kernelNameIndex).IndexOf(')') + 1);

                ContainedKernels.Add(kernelName, new CLKernel(k, kernelName, parameter));
            }

        }


        /// <summary>
        /// Extracts the kernel names from the program source
        /// </summary>
        /// <param name="source">The complete source of the program</param>
        /// <returns>A list of kernel names</returns>
        private static string[] FindKernelNames(string source)
        {
            List<string> kernelNames = new List<string>();
            string[] s = source.Split(' ');
            List<string> parts = new List<string>();
            foreach (string part in s)
            {
                parts.AddRange(part.Split('\n'));
            }
            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] == "__kernel" || parts[i] == "kernel")
                {
                    if (i < parts.Count - 2 && parts[i + 1] == "void")
                    {
                        if (parts[i + 2].Contains('('))
                        {
                            kernelNames.Add(
                                parts[i + 2]. //The Kernel name
                                    Substring(0,
                                        parts[i + 2].IndexOf('(')
                                    )
                            );
                        }
                        else
                        {
                            kernelNames.Add(parts[i + 2]);
                        }
                    }
                }
            }
            return kernelNames.ToArray();

        }
    }
}