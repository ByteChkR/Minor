namespace Engine.BuildTools.Builder.CLI
{
    /// <summary>
    /// CLI Wrapper for the Engine.BuildTools.Builder Library
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            Builder.RunCommand(args);
        }
    }
}