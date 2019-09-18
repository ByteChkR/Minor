using System;
using System.Collections.Generic;
using Common;
using OpenCl.DotNetCore.Kernels;
using OpenCl.DotNetCore.Programs;

namespace CLHelperLibrary
{
    public class CLProgram
    {
        private string _filePath;
        public Dictionary<string, CLKernel> ContainedKernels;
        public Program CLProgramHandle = null;

        public CLProgram(string FilePath)
        {
            this._filePath = FilePath;
            Initialize();
        }


        private void Initialize()
        {
            string source = TextProcessorAPI.PreprocessSource(_filePath, null);
            string[] kernelNames = FindKernelNames(source);

            ContainedKernels = new Dictionary<string, CLKernel>();
#if NO_CL
#else
            CLProgramHandle = CL.CreateCLProgramFromSource(source);

#endif
            foreach (string kernelName in kernelNames)
            {
#if NO_CL
                Kernel k = null;
#else
                Kernel k = CLProgramHandle.CreateKernel(kernelName);
#endif
                int kernelNameIndex = source.IndexOf(" " + kernelName + " ", StringComparison.InvariantCulture);
                kernelNameIndex = (kernelNameIndex == -1) ? source.IndexOf(" " + kernelName + "(", StringComparison.InvariantCulture) : kernelNameIndex;
                KernelParameter[] parameter = KernelParameter.CreateKernelParametersFromKernelCode(source,
                    kernelNameIndex,
                    source.Substring(kernelNameIndex, source.Length - kernelNameIndex).IndexOf(')') + 1);
                
                ContainedKernels.Add(kernelName, new CLKernel(k, parameter));
            }

        }



        private string[] FindKernelNames(string source)
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
                        else kernelNames.Add(parts[i + 2]);
                    }
                }
            }
            return kernelNames.ToArray();

        }
    }
}