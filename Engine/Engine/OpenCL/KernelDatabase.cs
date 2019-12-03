using System.Collections.Generic;
using System.IO;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;

namespace Engine.OpenCL
{
    /// <summary>
    /// A class used to store and manage Kernels
    /// </summary>
    public class KernelDatabase
    {
        /// <summary>
        /// The Folder that will get searched when initializing the database.
        /// </summary>
        private readonly string folderName;

        /// <summary>
        /// The currently loaded kernels
        /// </summary>
        public readonly Dictionary<string, CLKernel> LoadedKernels;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="folderName">Folder name where the kernels are located</param>
        /// <param name="genDataType">The DataTypes used to compile the FL Database</param>
        public KernelDatabase(Clapi instance, string folderName, TypeEnums.DataTypes genDataType)
        {
            GenDataType = KernelParameter.GetDataString(genDataType);
            if (!IOManager.FolderExists(folderName))
            {
                Logger.Crash(new InvalidFolderPathException(folderName), false);
            }

            this.folderName = folderName;
            LoadedKernels = new Dictionary<string, CLKernel>();
            Initialize(instance);
        }


        public string GenDataType { get; }


        /// <summary>
        /// Initializes the Kernel Database
        /// </summary>
        private void Initialize(Clapi instance)
        {
            string[] files = IOManager.GetFiles(folderName, "*.cl");

            foreach (string file in files)
            {
                AddProgram(instance, file);
            }
        }


        /// <summary>
        /// Manually adds a Program to the database
        /// </summary>
        /// <param name="file">Path fo the file</param>
        public void AddProgram(Clapi instance, string file)
        {
            if (!IOManager.Exists(file))
            {
                Logger.Crash(new InvalidFilePathException(file), true);
                return;
            }


            string path = Path.GetFullPath(file);

            Logger.Log("Creating CLProgram from file: " + file, DebugChannel.Log | DebugChannel.OpenCl, 7);
            ClProgram program = new ClProgram(instance, path, GenDataType);

            foreach (KeyValuePair<string, CLKernel> containedKernel in program.ContainedKernels)
            {
                if (!LoadedKernels.ContainsKey(containedKernel.Key))
                {
                    Logger.Log("Adding Kernel: " + containedKernel.Key, DebugChannel.Log | DebugChannel.OpenCl, 6);
                    LoadedKernels.Add(containedKernel.Key, containedKernel.Value);
                }
                else
                {
                    Logger.Log("Kernel with name: " + containedKernel.Key + " is already loaded. Skipping...",
                        DebugChannel.Log | DebugChannel.OpenCl, 7);
                }
            }
        }

        /// <summary>
        /// Tries to get the CLKernel by the specified name
        /// </summary>
        /// <param name="name">The name of the kernel</param>
        /// <param name="kernel">The kernel. Null if not found</param>
        /// <returns>Returns True if the kernel has been found</returns>
        public bool TryGetClKernel(string name, out CLKernel kernel)
        {
            if (LoadedKernels.ContainsKey(name))
            {
                kernel = LoadedKernels[name];
                return true;
            }

            kernel = null;
            return false;
        }
    }
}