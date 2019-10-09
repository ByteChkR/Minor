using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Programs;

namespace Engine.OpenCL
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
            _filePath = FilePath;
            _genType = genType;

            ContainedKernels = new Dictionary<string, CLKernel>();

            Initialize();
        }

        /// <summary>
        /// Returns the N of the VectorN types
        /// </summary>
        /// <param name="dtStr">the cl type in use</param>
        /// <returns>the amount of dimensions in the vector type</returns>
        private static int GetVectorNum(string dtStr)
        {
            if (!char.IsNumber(dtStr.Last()))
            {
                return 1;
            }

            if (dtStr.Last() == '2')
            {
                return 2;
            }

            if (dtStr.Last() == '3')
            {
                return 3;
            }

            if (dtStr.Last() == '4')
            {
                return 4;
            }

            if (dtStr.Last() == '8')
            {
                return 8;
            }

            if (dtStr.Last() == '6')
            {
                return 16;
            }

            return 0;
        }

        /// <summary>
        /// Loads the source and initializes the CLProgram
        /// </summary>
        private void Initialize()
        {


            var vnum = GetVectorNum(_genType);
            var lines = TextProcessorAPI.GenericIncludeToSource(".cl", _filePath, _genType,
                vnum == 0 || vnum == 1 ? "float" : "float" + vnum);
            var defs = new Dictionary<string, bool> { { "V_" + vnum, true } };
            var source = TextProcessorAPI.PreprocessSource(lines, _filePath, defs);
            var kernelNames = FindKernelNames(source);

            ClProgramHandle = CLAPI.CreateCLProgramFromSource(source);



            foreach (var kernelName in kernelNames)
            {
                var k = CLAPI.CreateKernelFromName(ClProgramHandle, kernelName);
                var kernelNameIndex = source.IndexOf(" " + kernelName + " ", StringComparison.InvariantCulture);
                kernelNameIndex = kernelNameIndex == -1
                    ? source.IndexOf(" " + kernelName + "(", StringComparison.InvariantCulture)
                    : kernelNameIndex;
                var parameter = KernelParameter.CreateKernelParametersFromKernelCode(source,
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
            var kernelNames = new List<string>();
            var s = source.Split(' ');
            var parts = new List<string>();
            foreach (var part in s)
            {
                parts.AddRange(part.Split('\n'));
            }

            for (var i = 0; i < parts.Count; i++)
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