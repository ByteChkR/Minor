using System.Collections.Generic;
using System.IO;
using Common;
using Common.Exceptions;

namespace CLHelperLibrary
{
    public class KernelDatabase
    {
        private readonly string _folderName;
        private Dictionary<string, CLKernel> loadedKernels;
        public KernelDatabase(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                this.Crash(new InvalidFolderPathException(folderName));
                return;
            }

            this._folderName = folderName;
            loadedKernels = new Dictionary<string, CLKernel>();
            Initialize();
        }

        private void Initialize()
        {
            string[] files = Directory.GetFiles(_folderName, "*.cl");

            foreach (string file in files)
            {
                AddProgram(file);
            }
        }

        public void AddProgram(string file)
        {

            if (!File.Exists(file))
            {
                this.Crash(new InvalidFilePathException(file));
                return;
            }


            string path = Path.GetFullPath(file);

            this.Log("Creating CLProgram from file: " + file, DebugChannel.Warning);
            CLProgram program = new CLProgram(path);

            foreach (KeyValuePair<string, CLKernel> containedKernel in program.ContainedKernels)
            {
                if (!loadedKernels.ContainsKey(containedKernel.Key))
                {
                    loadedKernels.Add(containedKernel.Key, containedKernel.Value);
                }
                else
                {
                    this.Log("Kernel with name: " + containedKernel.Key + " is already loaded. Skipping...", DebugChannel.Warning);
                }
            }
        }

        public bool TryGetCLKernel(string name, out CLKernel kernel)
        {
            if (loadedKernels.ContainsKey(name))
            {
                kernel = loadedKernels[name];
                return true;
            }
            kernel = null;
            return false;

        }



    }
}