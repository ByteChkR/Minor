using System;
using System.IO;
using CommandRunner;
using Engine.BuildTools.Common;

namespace Engine.BuildTools.Builder.Commands
{
    public class EmbedCommand : AbstractCommand
    {
        private static void EmbedFiles(StartupInfo info, string[] args)
        {
            try
            {
                string[] files = Directory.GetFiles(Path.GetFullPath(args[1]), "*", SearchOption.AllDirectories);
                AssemblyEmbedder.EmbedFilesIntoProject(Path.GetFullPath(args[0]), files);
            }
            catch (Exception e)
            {

                throw new ApplicationException("Input Error", e);
            }
        }

        public EmbedCommand() : base(EmbedFiles, new[] { "--embed", "-e" }, "--embed <Path/To/CSProj/File> <Folder/To/Embed>\nEmbeds the files in the specified folder into the .csproj file of the game project.", false)
        {

        }
    }
}