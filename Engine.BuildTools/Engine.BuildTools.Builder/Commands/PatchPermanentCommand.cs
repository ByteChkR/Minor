using System;
using CommandRunner;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder.Commands
{
    public class PatchPermanentCommand : AbstractCommand
    {

        private static void PatchPackagePermanent(StartupInfo info, string[] args)
        {
            if (args.Length != 2)
            {
                throw new ApplicationException("Invalid Input");
            }

            try
            {
                Creator.PatchPackagePermanent(args[0], args[1]);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Input Error", e);
            }
        }

        public PatchPermanentCommand() : base(PatchPackagePermanent, new[] { "--patch-permanent", "-pp" }, "--patch-permanent <targetFile> <patchFile>\nApplies the patch to the file permanently.", false)
        {

        }
    }
}