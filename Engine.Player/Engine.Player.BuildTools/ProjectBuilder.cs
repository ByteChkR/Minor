using System;
using System.IO;
using System.Threading;
using Engine.Player.Common;

namespace Engine.Player.BuildTools
{
    public class ProjectBuilder
    {

        public static int BuildProject(string buildFile, string csProjFile, Action waitAction)
        {
            return ProcessUtils.RunProcess(buildFile, csProjFile, waitAction);

        }

    }
}