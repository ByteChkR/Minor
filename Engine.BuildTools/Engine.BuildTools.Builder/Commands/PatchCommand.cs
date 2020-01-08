using System;
using CommandRunner;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder.Commands
{
    public class PatchCommand : AbstractCommand
    {

        private static void PatchPackage(StartupInfo info, string[] args)
        {
            if (args.Length != 2)
            {
                throw new ApplicationException("Invalid Input");

            }

            try
            {
                Creator.PatchPackage(args[0], args[1]);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Input Error", e);
            }
        }
        public PatchCommand() : base(PatchPackage, new[] { "--patch", "-p" }, "--patch <targetFile> <patchFile>\nApplies the patch to the file.", false)
        {

        }
    }
}