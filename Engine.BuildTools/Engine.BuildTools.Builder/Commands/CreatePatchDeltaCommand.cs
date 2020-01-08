using System;
using CommandRunner;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder.Commands
{
    public class CreatePatchDeltaCommand :AbstractCommand
    {
        private static void CreatePatchDelta(StartupInfo info, string[] args)
        {
            if (args.Length != 3)
            {
                throw new ApplicationException("Invalid Input");
            }

            try
            {
                Creator.CreatePatchFromDelta(args[0], args[1], args[2]);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Input Error", e);
            }
        }
        public CreatePatchDeltaCommand() : base(CreatePatchDelta, new[] { "--create-patch-delta", "-cdpatch" }, "--create-patch-delta <oldFile> <newFile> <destinationFile>", false)
        {

        }
    }
}