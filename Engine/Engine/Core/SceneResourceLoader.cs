//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Net.Mime;
//using System.Threading;
//using System.Xml.Serialization;
//using Assimp;
//using Engine.DataTypes;
//using Engine.Debug;
//using Engine.Exceptions;
//using Engine.IO;
//using Engine.OpenCL.DotNetCore.Memory;
//using Mesh = Engine.DataTypes.Mesh;

//namespace Engine.Core
//{

//    public class TextFileLoader : FileTypeLoader
//    {
//        private ConcurrentDictionary<string, string> localLoaded = new ConcurrentDictionary<string, string>();
//        public bool Done => Interlocked.Read(ref localDoneCount) == localLoaded.Count;
//        public TextFileLoader(FileKeyMapItem[] items) : base(items) { }
//        protected override void RunMultithreaded(FileKeyMapItem item, int doneIndex)
//        {
//            localLoaded.TryAdd(item.Key, IOManager.ReadAllText(item.Path));

//            Interlocked.Increment(ref localDoneCount);
//        }

//        public override void RunDataMergingStage(SceneResources res)
//        {
//            foreach (var textFile in localLoaded)
//            {
//                if (!res.LoadedTextFiles.ContainsKey(textFile.Key)) res.LoadedTextFiles.Add(textFile.Key, textFile.Value);
//            }
//        }
//    }
//    public class BinaryFileLoader : FileTypeLoader
//    {
//        private ConcurrentDictionary<string, byte[]> localLoaded = new ConcurrentDictionary<string, byte[]>();
//        public BinaryFileLoader(FileKeyMapItem[] items) : base(items) { }
//        protected override void RunMultithreaded(FileKeyMapItem item, int doneIndex)
//        {
//            Stream s = IOManager.GetStream(item.Path);
//            byte[] b = new byte[s.Length];
//            s.Read(b, 0, b.Length);
//            s.Close();
//            localLoaded.TryAdd(item.Key, b);

//            Interlocked.Increment(ref localDoneCount);
//        }

//        public override void RunDataMergingStage(SceneResources res)
//        {
//            foreach (var binaryFile in localLoaded)
//            {
//                res.LoadedBinaryFiles.Add(binaryFile.Key, binaryFile.Value);
//            }
//        }
//    }
//    public class TextureFileLoader : FileTypeLoader
//    {
//        private ConcurrentDictionary<string, Tuple<byte[], int, int>> localLoaded = new ConcurrentDictionary<string, Tuple<byte[], int, int>>();
//        public TextureFileLoader(FileKeyMapItem[] items) : base(items) { }
//        protected override void RunMultithreaded(FileKeyMapItem item, int doneIndex)
//        {
//            Stream s = IOManager.GetStream(item.Path);
//            Bitmap bmp = new Bitmap(s);
//            s.Close();
//            localLoaded.TryAdd(item.Key, new Tuple<byte[], int, int>(TextureLoader.BitmapToBytes(bmp), bmp.Width, bmp.Height));

//            Interlocked.Increment(ref localDoneCount);
//        }

//        public override void RunDataMergingStage(SceneResources res)
//        {
//            foreach (var binaryFile in localLoaded)
//            {
//                res.LoadedTextures.Add(binaryFile.Key, TextureLoader.BytesToTexture(binaryFile.Value.Item1, binaryFile.Value.Item2, binaryFile.Value.Item3));
//            }
//        }
//    }

//    public class MeshFileLoader : FileTypeLoader
//    {
//        private ConcurrentDictionary<string, Scene> localLoaded = new ConcurrentDictionary<string, Scene>();
//        public MeshFileLoader(FileKeyMapItem[] items) : base(items) { }
//        protected override void RunMultithreaded(FileKeyMapItem item, int doneIndex)
//        {
//            Stream s = IOManager.GetStream(item.Path);

//            localLoaded.TryAdd(item.Key, MeshLoader.LoadInternalAssimpScene(s, Path.GetExtension(item.Path)));
//            s.Close();

//            Interlocked.Increment(ref localDoneCount);
//        }

//        public override void RunDataMergingStage(SceneResources res)
//        {
//            foreach (var binaryFile in localLoaded)
//            {
//                res.LoadedMeshes.Add(binaryFile.Key, MeshLoader.LoadAssimpScene(binaryFile.Value, "")[0]);
//            }
//        }
//    }
//    public abstract class FileTypeLoader
//    {
//        private FileKeyMapItem[] localCopy;
//        protected long localDoneCount;
//        protected FileTypeLoader(FileKeyMapItem[] items)
//        {
//            localDoneCount = items.Length;
//            localCopy = items;
//        }

//        protected virtual void RunMultithreaded(FileKeyMapItem items, int doneIndex)
//        {
//        }

//        public void RunMultithreaded()
//        {
//            for (int i = 0; i < localCopy.Length; i++)
//            {
//                ThreadPool.QueueUserWorkItem((x) => RunMultithreaded(x, i), localCopy[i], true);
//            }
//        }

//        public virtual void RunDataMergingStage(SceneResources res)
//        {

//        }
//    }
//    [Serializable]
//    public class SceneResources
//    {
//        private delegate void LoadTask(FileKeyMapItem item);
//        public string AssemblyQualifiedName;
//        public List<FileKeyMapItem> Assets;
//        [XmlIgnore] public Dictionary<string, string> LoadedTextFiles;
//        [XmlIgnore] public Dictionary<string, byte[]> LoadedBinaryFiles;
//        [XmlIgnore] public Dictionary<string, Texture> LoadedTextures;
//        [XmlIgnore] public Dictionary<string, AudioFile> LoadedAudioFiles;
//        [XmlIgnore] public Dictionary<string, GameFont> LoadedFonts;
//        [XmlIgnore] public Dictionary<string, MemoryBuffer> LoadedFLScripts;
//        [XmlIgnore] public Dictionary<string, Mesh> LoadedMeshes;
//        [XmlIgnore]
//        private static Dictionary<FileType, Type> _loadTasks = new Dictionary<FileType, Type>
//        {
//            {FileType.Text, typeof(TextFileLoader) }
//        };


//        public static SceneResources Load(string path)
//        {
//            XmlSerializer xs = new XmlSerializer(typeof(SceneResources));
//            Stream resourceFile = IOManager.GetStream(path);
//            SceneResources res = (SceneResources) xs.Deserialize(resourceFile);
//            resourceFile.Close();
//            Dictionary<FileType, List<FileKeyMapItem>> localCopies = new Dictionary<FileType, List<FileKeyMapItem>>();
//            for (int i = 0; i < res.Assets.Count; i++)
//            {
//                if (!localCopies.ContainsKey(res.Assets[i].FileType)) localCopies.Add(res.Assets[i].FileType, new List<FileKeyMapItem> { res.Assets[i] });
//                else localCopies[res.Assets[i].FileType].Add(res.Assets[i]);
//            }
//            FileTypeLoader[] loader = new FileTypeLoader[localCopies.Count];
//            int ii = 0;
//            foreach (var localCopy in localCopies)
//            {
//                loader[ii] = (FileTypeLoader)Activator.CreateInstance(_loadTasks[localCopy.Key], new object[] { localCopy.Value });
//                ii++;
//                loader[ii].RunMultithreaded();
//            }
//        }

//    }

//    public enum FileType
//    {
//        Text,
//        Binary,
//        Texture,
//        Audio,
//        Font,
//        FLScript,
//        Mesh
//    }
//    [Serializable]
//    public class FileKeyMapItem
//    {
//        public FileType FileType;
//        public string Key;
//        public string Path;
//    }
    
//}