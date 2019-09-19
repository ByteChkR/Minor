using System;
using System.Collections.Generic;
using Common;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Programs;

namespace CLHelperLibrary
{
    public class CLProgram
    {
        private readonly string _filePath;
        public Dictionary<string, CLKernel> ContainedKernels { get; }
        public Program ClProgramHandle { get; set; }

        public CLProgram(string FilePath)
        {
            this._filePath = FilePath;

            ContainedKernels = new Dictionary<string, CLKernel>();

            Initialize();
        }


        private void Initialize()
        {
            string source = TextProcessorAPI.PreprocessSource(_filePath, null);
            string[] kernelNames = FindKernelNames(source);


            ClProgramHandle = CL.CreateCLProgramFromSource(source);

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