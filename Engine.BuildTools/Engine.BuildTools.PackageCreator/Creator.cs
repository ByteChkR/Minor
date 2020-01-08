using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using Engine.BuildTools.PackageCreator.Versions;
using Engine.BuildTools.PackageCreator.Versions.Legacy;
using Engine.BuildTools.PackageCreator.Versions.v1;
using Engine.BuildTools.PackageCreator.Versions.v2;

namespace Engine.BuildTools.PackageCreator
{
    /// <summary>
    /// Class used to Create a game or engine package
    /// </summary>
    public static class Creator
    {
        public const string DefaultVersion = "v1";

        private static Dictionary<string, IPackageVersion> _packageVersions = new Dictionary<string, IPackageVersion>
        {
            {"legacy", new LegacyVersion()},
            {"v1", new Version1()},
            {"v2", new Version2()}
        };

        public static bool HasManifest(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            foreach (KeyValuePair<string, IPackageVersion> packageVersion in _packageVersions)
            {
                if (packageVersion.Value.IsVersion(path))
                {
                    archive.Dispose();
                    return true;
                }
            }

            return false;
        }



        public static IPackageManifest ReadManifest(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            foreach (KeyValuePair<string, IPackageVersion> packageVersion in _packageVersions)
            {
                if (packageVersion.Value.IsVersion(path))
                {
                    archive.Dispose();
                    return packageVersion.Value.GetPackageManifest(path);
                }
            }

            if (path.EndsWith(".patch")) return null; //Patches CAN but DONT need a Manifest
            throw new IOException("The file is not a supported format.");
        }

        private static string GetPackageVersion(string path)
        {
            ZipArchive archive = ZipFile.OpenRead(path);
            foreach (KeyValuePair<string, IPackageVersion> packageVersion in _packageVersions)
            {
                if (packageVersion.Value.IsVersion(path))
                {
                    archive.Dispose();
                    return packageVersion.Key;
                }
            }

            return "unknown";
        }

        public static void WriteManifest(Stream s, IPackageManifest o)
        {
            _packageVersions[o.PackageVersion].WriteManifest(s, o);
        }

        public static void CreateGamePackage(string packageName, string executable, string outputFile,
            string workingDir, string[] files, string version, string packageVersion = DefaultVersion)
        {
            _packageVersions[packageVersion]
                .CreateGamePackage(packageName, executable, outputFile, workingDir, files, version);
        }

        public static void CreateEnginePackage(string outputFile, string workingDir, string[] files,
            string packageVersion = DefaultVersion)
        {
            _packageVersions[packageVersion].CreateEnginePackage(outputFile, workingDir, files);
        }

        public static void UnpackPackage(string file, string outPutDir)
        {
            _packageVersions[GetPackageVersion(file)].UnpackPackage(file, outPutDir);
        }

        public static string GetEngineVersion(string workingDir)
        {
            FileVersionInfo v = FileVersionInfo.GetVersionInfo(workingDir + "/Engine.dll");

            return v.FileVersion;
        }

        private static string UnpackForPatching(string mainFile)
        {


            string path = Path.GetTempPath() + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            Console.WriteLine("Creating Temp Folder: " + path);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            Console.WriteLine("Extracting Main File to " + path);
            ZipFile.ExtractToDirectory(mainFile, path);

            return path;

        }

        public static void ApplyPatches(string folder, string packageVersion)
        {
            Console.WriteLine("Applying Patches on folder " + folder);
            string[] patches = Directory.GetFiles(folder + "/patches", "*.patch");
            for (int i = 0; i < patches.Length; i++)
            {
                Console.WriteLine("Applying Patch:" + patches[i]);
                PatchFolder(folder, patches[i], packageVersion);
            }
        }

        private static string ComputeHash(Stream content)
        {
            MD5 _md5 = MD5.Create();
            return BitConverter.ToString(_md5.ComputeHash(content)).Replace("-", "");
        }
        private static bool IsFileDifferent(string file, string other)
        {
            Stream s1 = new FileStream(file, FileMode.Open);
            Stream s2 = new FileStream(other, FileMode.Open);
            bool ret = ComputeHash(s1) != ComputeHash(s2);
            s1.Close();
            s2.Close();
            return ret;
        }

        public static List<string> CreateChangedFiles(string oldFile, string newFile, out string newFilePath)
        {


            Console.WriteLine($"Detecting File Changes between {oldFile} <=> {newFile}");

            string dirOld = UnpackForPatching(oldFile);
            newFilePath = UnpackForPatching(newFile);
            List<string> fileList = new List<string>();

            string[] oldFiles = Directory.GetFiles(dirOld, "*", SearchOption.AllDirectories);
            for (int i = 0; i < oldFiles.Length; i++)
            {
                oldFiles[i] = oldFiles[i].Replace(dirOld, "");
            }

            string[] newFiles = Directory.GetFiles(newFilePath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < newFiles.Length; i++)
            {
                newFiles[i] = newFiles[i].Replace(newFilePath, "");
            }

            for (int i = 0; i < newFiles.Length; i++)
            {
                if (!oldFiles.Contains(newFiles[i]) || IsFileDifferent(newFilePath + newFiles[i], dirOld + newFiles[i]))
                {
                    Console.WriteLine($"Found File Change: {newFiles[i]}");
                    fileList.Add(newFilePath + newFiles[i]);
                }
            }

            return fileList;



        }

        public static void CreatePatchFromFolder(string folder, string output)
        {
            Console.WriteLine("Creating Patch from Folder: " + folder);
            ZipFile.CreateFromDirectory(folder, output);
        }

        private static void CreatePatchFromFileList(string[] fileList, string workingDir, string output)
        {
            Console.WriteLine("Creating Patch from File List...");
            for (int i = 0; i < fileList.Length; i++)
            {
                fileList[i] = fileList[i].Replace(workingDir + "\\", "");
            }
            if (File.Exists(output)) File.Delete(output);

            Console.WriteLine("Creating Patch File");
            ZipFile.CreateFromDirectory(workingDir, output);
            Stream packStream = new FileStream(output, FileMode.Open);
            ZipArchive za = new ZipArchive(packStream, ZipArchiveMode.Update);


            Console.WriteLine("Remove duplicate Files");
            for (int i = za.Entries.Count - 1; i >= 0; i--)
            {
                if (!fileList.Contains(za.Entries[i].FullName))
                {
                    Console.WriteLine("Removing File: " + za.Entries[i].FullName);
                    za.Entries[i].Delete();
                }
            }


            za.Dispose();
            packStream.Close();

        }

        public static void CreatePatchFromDelta(string oldFile, string newFile, string output)
        {
            List<string> newFiles = CreateChangedFiles(oldFile, newFile, out string newFilePath);
            CreatePatchFromFileList(newFiles.ToArray(), newFilePath, output);
        }

        public static void PatchPackagePermanent(string mainFile, string patchFile)
        {

            Console.WriteLine($"Permanently Applying {patchFile} to {mainFile}");
            string dirPath = UnpackForPatching(mainFile);
            IPackageManifest pm = ReadManifest(mainFile);
            PatchFolder(dirPath, Path.GetFullPath(patchFile), pm.PackageVersion);
            File.Delete(Path.GetFullPath(mainFile));
            Console.WriteLine($"Repackaging {mainFile}");
            ZipFile.CreateFromDirectory(dirPath, Path.GetFullPath(mainFile));
        }

        public static void PatchPackage(string mainFile, string patchFile)
        {
            Console.WriteLine($"Adding {patchFile} to {mainFile}");
            string dirPath = UnpackForPatching(mainFile);

            if (!Directory.Exists(dirPath + "/patches"))
            {
                Directory.CreateDirectory(dirPath + "/patches");
            }

            if (File.Exists(dirPath + "/patches/" + Path.GetFileName(patchFile)))
            {
                File.Delete(dirPath + "/patches/" + Path.GetFileName(patchFile));
            }
            File.Copy(patchFile, dirPath + "/patches/" + Path.GetFileName(patchFile));

            string p = Path.GetFullPath(mainFile);
            File.Delete(p);


            Console.WriteLine($"Repackaging {mainFile}");
            ZipFile.CreateFromDirectory(dirPath, mainFile);
        }

        public static void PatchFolder(string folder, string patchFile, string folderManifestVersion)
        {
            
            Console.WriteLine($"Patching Folder {patchFile} => {folder}");
            using (ZipArchive zip1 = ZipFile.OpenRead(patchFile))
            {
                for (int i = 0; i < zip1.Entries.Count; i++)
                {

                    Console.WriteLine($"Patching File {zip1.Entries[i].FullName}");
                    zip1.Entries[i].ExtractToFile(folder + "/" + zip1.Entries[i].FullName, true);
                    // here, we extract every entry, but we could extract conditionally
                    // based on entry name, size, date, checkbox status, etc.  

                }
            }

            IPackageManifest pm = ReadManifest(patchFile);
            if (pm != null) //New Package File included
            {
                if (_packageVersions[pm.PackageVersion].ManifestPath !=
                    _packageVersions[folderManifestVersion].ManifestPath)
                {
                    string oldFile = Path.Combine(folder, _packageVersions[folderManifestVersion].ManifestPath);
                    File.Delete(oldFile);
                }
            }
        }
    }
}