using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using Engine.AssetPackaging;
using ReleaseBuilder;

namespace Engine.ReleaseBuilder
{
    public partial class Form1 : Form
    {
        private static Form1 Instance;
        private BuildSettings Settings;
        public Form1(BuildSettings settings = null)
        {
            Settings = settings;
            Instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            WriteInfo();
            
        }

        private void InvalidateForm()
        {
            if (Settings != null)
            {
                tbAssetFolder.Text = Settings.AssetFolder;
                tbProject.Text = Settings.Project;
                tbPackagedFiles.Text = Settings.UnpackFiles;
                tbUnpackagedFiles.Text = Settings.MemoryFiles;
                tbOutputFolder.Text = Settings.OutputFolder;
                cbOnlyEmbed.Checked = Settings.BuildFlags == BuildType.Embed;
                nudPackSize.Value = Settings.PackSize;
            }
        }

        private void InvalidateSettings()
        {
            Settings = new BuildSettings();
            Settings.AssetFolder = tbAssetFolder.Text;
            Settings.Project = tbProject.Text;
            if (cbOnlyEmbed.Checked) Settings.BuildFlags = BuildType.Embed;
            else if (cbPacksOnDisk.Checked) Settings.BuildFlags = BuildType.PackOnly;
            else Settings.BuildFlags = BuildType.PackEmbed;
            Settings.UnpackFiles = tbPackagedFiles.Text;
            Settings.MemoryFiles = tbUnpackagedFiles.Text;
            Settings.OutputFolder = tbOutputFolder.Text;
            Settings.PackSize = (int)nudPackSize.Value;
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            if (ofdProject.ShowDialog() == DialogResult.OK)
            {
                tbProject.Text = ofdProject.FileName;
            }
        }

        private void btnOpenAssetFolder_Click(object sender, EventArgs e)
        {
            if (fbdAssetFolder.ShowDialog() == DialogResult.OK)
            {
                tbAssetFolder.Text = fbdAssetFolder.SelectedPath;
            }
        }

        private void btnOpenOutputFolder_Click(object sender, EventArgs e)
        {
            if (fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                tbOutputFolder.Text = fbdOutputFolder.SelectedPath;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            rtbBuildOutput.Text = "";
            if (!cbOnlyEmbed.Checked)
            {
                PackAssets(
                    Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text),
                    (int)nudPackSize.Value, tbPackagedFiles.Text, tbUnpackagedFiles.Text, tbAssetFolder.Text, cbCompression.Checked);
            }

            BuildProject("resources/Build.bat", tbAssetFolder.Text, tbPackagedFiles.Text, tbUnpackagedFiles.Text,
                tbProject.Text, tbOutputFolder.Text, cbOnlyEmbed.Checked);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            rtbBuildOutput.Text = "";
            if (!cbOnlyEmbed.Checked)
            {
                PackAssets(
                    Path.GetDirectoryName(tbProject.Text) + "\\" + Path.GetFileNameWithoutExtension(tbProject.Text),
                    (int)nudPackSize.Value, tbPackagedFiles.Text, tbUnpackagedFiles.Text, tbAssetFolder.Text, cbCompression.Checked);
            }

            BuildProject("resources/Build_NoDelete.bat", tbAssetFolder.Text, tbPackagedFiles.Text,
                tbUnpackagedFiles.Text, tbProject.Text, tbOutputFolder.Text, cbOnlyEmbed.Checked);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                string output = fbdOutputFolder.SelectedPath;
                if (Directory.Exists(output + "/packs"))
                {
                    Directory.Delete(output + "/packs", true);
                }

                PackAssets(output, (int)nudPackSize.Value, tbPackagedFiles.Text, tbUnpackagedFiles.Text,
                    tbAssetFolder.Text, cbCompression.Checked);
            }
        }

        public static void RunConfig(BuildSettings settings)
        {
            if (settings.BuildFlags == BuildType.PackOnly)
            {
                if (Directory.Exists(settings.OutputFolder + "/packs"))
                {
                    Directory.Delete(settings.OutputFolder + "/packs", true);
                }
                PackAssets(settings.OutputFolder, settings.PackSize, settings.UnpackFiles, settings.MemoryFiles,
                   settings.AssetFolder, false);
                //???
                BuildProject("resources/Build.bat", settings.AssetFolder, "", "",
                    settings.Project, settings.OutputFolder, false);
            }
            else if (settings.BuildFlags == BuildType.Embed)
            {
                BuildProject("resources/Build.bat", settings.AssetFolder, settings.UnpackFiles, settings.MemoryFiles,
                    settings.Project, settings.OutputFolder, true);
            }
            else if (settings.BuildFlags == BuildType.PackEmbed)
            {
                string dir = Path.GetDirectoryName(settings.Project);
                if (Directory.Exists(dir + "/packs"))
                {
                    Directory.Delete(dir + "/packs", true);
                }
                PackAssets(
                    dir + "\\" + Path.GetFileNameWithoutExtension(settings.Project),
                    settings.PackSize, settings.UnpackFiles, settings.MemoryFiles, settings.AssetFolder, false);
                BuildProject("resources/Build.bat", settings.AssetFolder, settings.MemoryFiles, settings.UnpackFiles,
                    settings.Project, settings.OutputFolder, settings.BuildFlags == BuildType.Embed);
            }
        }

        private static T GetAssemblyAttribute<T>(Assembly asm) where T : Attribute
        {
            object[] attributes =
                asm.GetCustomAttributes(typeof(T), true);

            if (attributes.Length == 0)
            {
                return null;
            }

            return (T)attributes[0];
        }

        private static void WriteInfo()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            AssemblyCopyrightAttribute attrib = GetAssemblyAttribute<AssemblyCopyrightAttribute>(asm);

            string info = asm.GetName().Name + "(" +
                          asm.GetName().Version + ")\n" +
                          attrib.Copyright;

            Instance?.WriteLine(info);
        }

        private static void PackAssets(string outputFolder, int packSize, string packedFileExts,
            string unpackedFileExts, string assetFolder, bool compression)
        {
            AssetPacker.MAXSIZE_KILOBYTES = packSize;

            Instance?.WriteLine("Parsing File info...");

            AssetPackageInfo info = new AssetPackageInfo();
            List<string> unpackExts = packedFileExts.Split('+').ToList();
            for (int i = 0; i < unpackExts.Count; i++)
            {
                info.FileInfos.Add(unpackExts[i], new AssetFileInfo() { packageType = AssetPackageType.Unpack });
            }

            List<string> packExts = unpackedFileExts.Split("+".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < packExts.Count; i++)
            {
                info.FileInfos.Add(packExts[i], new AssetFileInfo() { packageType = AssetPackageType.Memory });
            }

            Instance?.WriteLine("Creating Asset Pack(" + assetFolder + ")...");
            AssetResult ret = AssetPacker.PackAssets(assetFolder, info, compression);
            Instance?.WriteLine("Packaging " + ret.indexList.Count + " Assets in " + ret.packs.Count + " Packs.");

            Instance?.WriteLine("Saving Asset Pack to " + outputFolder);
            ret.Save(outputFolder);

            Instance?.WriteLine("Packaging Assets Finished.");
        }

        private static void BuildProject(string buildFile, string assetFolder, string packedFilesExt,
            string unpackedFilesExt, string projectFile, string outputFolder, bool onlyEmbed)
        {
            Instance?.WriteLine("Starting Build Process.");

            string incdir = onlyEmbed
                ? assetFolder + "\\+" + unpackedFilesExt
                : //Normal Asset Folder
                Path.GetDirectoryName(projectFile) + "\\" + Path.GetFileNameWithoutExtension(projectFile) +
                "\\packs\\+*.xml+*.pack";
            string publishDir = Path.GetDirectoryName(projectFile) + "\\bin\\Release\\netcoreapp2.1\\publish";
            string projectDir = Path.GetDirectoryName(projectFile);
            string plainProjectName = Path.GetFileNameWithoutExtension(projectFile);
            string command =
                $"{projectFile} {incdir} {publishDir} {outputFolder} {projectDir} {plainProjectName}";


            if (Instance != null)
            {

                CheckForIllegalCrossThreadCalls = false;

                ProcessStartInfo psi = new ProcessStartInfo(buildFile, command)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };


                Process p = new Process { StartInfo = psi };

                p.Start();
                ConsoleRedirector redir;
                redir = ConsoleRedirector.CreateRedirector(p.StandardOutput, p.StandardError, p, Instance.WriteLine);

                redir.StartThreads();

                while (!p.HasExited)
                {
                    lock (Instance?.rtbBuildOutput)
                    {
                        Application.DoEvents();
                    }
                }


                redir.StopThreads();

                Instance?.WriteLine(redir.GetRemainingLogs());

                CheckForIllegalCrossThreadCalls = true;
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo(buildFile, command)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };


                Process p = new Process { StartInfo = psi };

                p.Start();
                ConsoleRedirector redir;
                redir = ConsoleRedirector.CreateRedirector(p.StandardOutput, p.StandardError, p, Console.WriteLine);

                redir.StartThreads();

                while (!p.HasExited)
                {
                    
                }


                redir.StopThreads();

                Console.WriteLine(redir.GetRemainingLogs());
            }
        }

        private void WriteLine(string line)
        {
            lock (rtbBuildOutput)
            {
                if (line != null)
                {
                    rtbBuildOutput.AppendText(line + "\n");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PackageCreator c = new PackageCreator();
            if (c.ShowDialog() == DialogResult.OK)
            {
                Console.ReadLine();
                AssetPackageInfo info = new AssetPackageInfo();
                Dictionary<string, AssetFileInfo> fileinfos = new Dictionary<string, AssetFileInfo>();
                foreach (KeyValuePair<string, string> keyValuePair in c.fileMap)
                {
                    List<string> folders = new List<string>();
                    string curFolder = Path.GetDirectoryName(keyValuePair.Value);
                    folders.Add(curFolder);
                    while (curFolder.Trim() != "\\")
                    {
                        if (string.IsNullOrEmpty(curFolder))
                        {
                            break;
                        }

                        folders.Add(curFolder);
                        curFolder = Path.GetDirectoryName(curFolder);
                    }

                    for (int i = 0; i < folders.Count; i++)
                    {
                        if (!Directory.Exists(folders[i]))
                        {
                            Directory.CreateDirectory(".\\" + folders[i]);
                        }
                    }

                    if (!File.Exists(keyValuePair.Value))
                        File.Copy(keyValuePair.Key, keyValuePair.Value);
                    AssetFileInfo afi = new AssetFileInfo() { packageType = AssetPackageType.Memory };
                    info.FileInfos.Add(keyValuePair.Value, afi);
                }

                AssetResult s = AssetPacker.PackAssets(Path.GetFullPath(".\\"), info, cbCompression.Checked);
                s.Save("./packs");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (sfdProjectSettings.ShowDialog() == DialogResult.OK)
            {

                InvalidateSettings();
                XmlSerializer xs = new XmlSerializer(typeof(BuildSettings));
                if (File.Exists(sfdProjectSettings.FileName)) File.Delete(sfdProjectSettings.FileName);
                FileStream fs = new FileStream(sfdProjectSettings.FileName, FileMode.Create);
                xs.Serialize(fs, Settings);
                fs.Close();
            }
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {

            if (ofdProjectSettings.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(BuildSettings));
                FileStream fs = new FileStream(ofdProjectSettings.FileName, FileMode.Create);
                Settings = (BuildSettings)xs.Deserialize(fs);
                fs.Close();
                InvalidateForm();
            }
        }
    }
}