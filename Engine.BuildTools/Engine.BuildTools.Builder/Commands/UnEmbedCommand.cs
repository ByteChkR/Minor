using System;
using System.IO;
using CommandRunner;
using Engine.BuildTools.Common;

namespace Engine.BuildTools.Builder.Commands
{
    public class UnEmbedCommand : AbstractCommand
    {
        private static void UnembedFiles(StartupInfo info, string[] args)
        {
            try
            {
                AssemblyEmbedder.UnEmbedFilesFromProject(Path.GetFullPath(args[0]));
            }
            catch (Exception e)
            {

                throw new ApplicationException("Input Error", e);
            }
        }

        public UnEmbedCommand() : base(UnembedFiles, new[] { "--unembed", "-u" }, "--unembed <Path/To/CSProj/File>\nUnembeds that were embedded into the .csproj file of the game project.", false)
        {

        }
    }
}