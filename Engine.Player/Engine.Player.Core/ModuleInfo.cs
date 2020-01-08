using CommandRunner;

namespace Engine.Player.Core
{
    public class ModuleInfo : AbstractCmdModuleInfo
    {
        public override string ModuleName => "eplay";

        public override string[] Dependencies => new[]
        {
            "Engine.BuildTools.Common.dll",
            "Engine.BuildTools.PackageCreator.dll"
        };

        public override void RunArgs(string[] args)
        {
            EnginePlayer.RunCommands(args);
        }
    }
}