using System;
using CommandRunner;
using Engine.BuildTools.PackageCreator;

namespace Engine.BuildTools.Builder.Commands
{
    public class CreatePatchCommand : AbstractCommand
    {
        private static void CreatePatch(StartupInfo info, string[] args)
        {
            if (args.Length != 2)
            {
                throw new ApplicationException("Invalid Input");
            }

            try
            {
                Creator.CreatePatchFromFolder(args[0], args[1]);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Input Error", e);
            }
        }
        public CreatePatchCommand() : base(CreatePatch, new[] { "--create-patch" , "-cpatch" }, "--create-patch <folder> <destinationFile>", false)
        {

        }
    }
}