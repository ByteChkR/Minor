using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Debug
{
    /// <summary>
    /// Engine Stage information that holds all informations regarding a Stage in the engine
    /// </summary>
    public class EngineStageInformation
    {
        /// <summary>
        /// Name of the Stage
        /// </summary>
        public string Name;

        /// <summary>
        /// A property that indicates if the Current Stage info was finalized(e.g. it will not be updated again since the stage is over)
        /// </summary>
        public bool Finalized { get; private set; }

        /// <summary>
        /// The GC Collection at the start of the stage
        /// </summary>
        public long Before;

        /// <summary>
        /// The GC Collection at the end of the stage
        /// </summary>
        public long After;

        /// <summary>
        /// The GC Collection at the end of the stage after GC.Collect has been called.
        /// </summary>
        public long AfterGarbageCollection;

        /// <summary>
        /// The Timing information about the current stage
        /// </summary>
        public TimeSpan TimeSpentInStage;

        /// <summary>
        /// The Parent Stage(null if its A root stage
        /// </summary>
        public EngineStageInformation Parent;

        /// <summary>
        /// List of substages
        /// </summary>
        public List<EngineStageInformation> SubStages;

        /// <summary>
        /// Timer that is used to keep track on how long the stage has been running
        /// </summary>
        public Stopwatch Timer;

        /// <summary>
        /// Internal Constructor to create a Stage Information object.
        /// </summary>
        /// <param name="name"></param>
        internal EngineStageInformation(string name)
        {
            Name = name;
#if !TRACE_TIME_ONLY
            Before = GC.GetTotalMemory(false) / 1024;
#endif
            SubStages = new List<EngineStageInformation>();
            Timer = new Stopwatch();
            Timer.Start();
        }

        /// <summary>
        /// Function called when this stage has ended
        /// </summary>
        internal void FinalizeStage()
        {
            Finalized = true;
#if !TRACE_TIME_ONLY
            After = GC.GetTotalMemory(false) / 1024;
            AfterGarbageCollection = GC.GetTotalMemory(true) / 1024;
#endif
            Timer.Stop();
            TimeSpentInStage = Timer.Elapsed;
        }

        /// <summary>
        /// Creates a "console-friendly" view of the Engine Stage by utilizing tabulators
        /// </summary>
        /// <param name="info">The info to represent</param>
        /// <param name="depth">The depth of the Engine Stage(Gets used for placing Tabulator characters)</param>
        /// <returns></returns>
        public static string ToConsoleText(EngineStageInformation info, int depth)
        {
            string ind = "\t";
            for (int i = 0; i < depth; i++)
            {
                ind += "\t";
            }


            string ret = info.Name + "\n";
            ret += ind + "KB Used Before: " + info.Before + "\n";
            ret += ind + "KB Used After: " + info.After + "\n";
            ret += ind + "KB Used After(post GC): " + info.AfterGarbageCollection + "\n";
            ret += ind + "Elapsed Time(MS): " + info.TimeSpentInStage.TotalMilliseconds + "\n";
            ret += ind + "Substep Count: " + info.SubStages.Count + "\n";
            ret += ind + "Substeps: \n";


            foreach (EngineStageInformation stepMemoryInformation in info.SubStages)
            {
                ret += ToConsoleText(stepMemoryInformation, depth + 1);
            }

            return ret;
        }

        /// <summary>
        /// To String override that is returning a Stage as it would look if it is the root object.(e.g. Depth 0)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToConsoleText(this, 0);
        }
    }
}