using System;
using System.Collections.Generic;
using System.Text;
using CommandRunner;

namespace Engine.BuildTools.Builder
{
    public class ModuleInfo : AbstractCmdModuleInfo
    {
        public override string[] Dependencies => new[]
        {
            "Engine.AssetPackaging.dll",
            "Engine.BuildTools.Common.dll",
            "Engine.BuildTools.PackageCreator.dll"
        };
        public override string ModuleName => "ebuild";
        public override void RunArgs(string[] args)
        {
            Builder.RunCommand(args);
        }
    }
}
