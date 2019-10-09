using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Debug
{
    public class EngineStageInformation
    {
        public string Name;
        public bool Finalized { get; private set; }
        public long Before;
        public long After;
        public long AfterGarbageCollection;
        public TimeSpan TimeSpentInStage;

        public EngineStageInformation Parent;
        public List<EngineStageInformation> SubStages;
        public Stopwatch Timer;

        public EngineStageInformation(string name)
        {
            Name = name;
#if !TRACE_TIME_ONLY
            Before = GC.GetTotalMemory(false) / 1024;
#endif
            SubStages = new List<EngineStageInformation>();
            Timer = new Stopwatch();
            Timer.Start();
        }

        public void FinalizeStage()
        {
            Finalized = true;
#if !TRACE_TIME_ONLY
            After = GC.GetTotalMemory(false) / 1024;
            AfterGarbageCollection = GC.GetTotalMemory(true) / 1024;
#endif
            Timer.Stop();
            TimeSpentInStage = Timer.Elapsed;
        }

        public static string ToConsoleText(EngineStageInformation info, int depth)
        {
            var ind = "\t";
            for (var i = 0; i < depth; i++)
            {
                ind += "\t";
            }


            var ret = info.Name + "\n";
            ret += ind + "KB Used Before: " + info.Before + "\n";
            ret += ind + "KB Used After: " + info.After + "\n";
            ret += ind + "KB Used After(post GC): " + info.AfterGarbageCollection + "\n";
            ret += ind + "Elapsed Time(MS): " + info.TimeSpentInStage.TotalMilliseconds + "\n";
            ret += ind + "Substep Count: " + info.SubStages.Count + "\n";
            ret += ind + "Substeps: \n";


            foreach (var stepMemoryInformation in info.SubStages)
            {
                ret += ToConsoleText(stepMemoryInformation, depth + 1);
            }

            return ret;
        }

        public override string ToString()
        {
            return ToConsoleText(this, 0);
        }
    }

    public static class MemoryTracer
    {
        private static List<EngineStageInformation> _informationCollection = new List<EngineStageInformation>();
        private static EngineStageInformation _current;
        public static int MaxTraceCount = 10;


        public static string cmdListMemoryInfo(string[] args)
        {
#if LEAK_TRACE
            string ret = "";
            foreach (var stepMemoryInformation in _informationCollection)
            {
                if (stepMemoryInformation.Finalized)
                    ret += stepMemoryInformation.ToString();
            }

            return ret;
#else

            return "Engine Was compiled without MemoryTracer enabled.";
#endif
        }

        public static string cmdListLastMemoryInfo(string[] args)
        {
#if LEAK_TRACE
            if (_informationCollection.Count < 2) return ""; //We nee one frame to be finished, otherwise part of the data is not correct
            return _informationCollection[_informationCollection.Count - 2].ToString();
#else

            return "Engine Was compiled without MemoryTracer enabled.";
#endif
        }


        /// <summary>
        /// Returns to the next higher parent step(does nothing when in root)
        /// </summary>
        public static void ReturnFromSubStage()
        {
#if LEAK_TRACE
            if (_current != null)
            {
                _current.FinalizeStage();
                if (_current.Parent != null)
                {
                    _current = _current.Parent;
                }
            }

#endif
        }

        /// <summary>
        /// Creates a new step information on one level deeper as before
        /// </summary>
        /// <param name="step"></param>
        public static void AddSubStage(string step)
        {
#if LEAK_TRACE
            EngineStageInformation current = _current; //Store the current step or substep
            ChangeStage(step, false);
            _current.Parent = current; //Set it up to be the substep on one level deeper
            if (current == null)
            {
                _informationCollection.Add(_current);

                while (_informationCollection.Count >= MaxTraceCount)
                {
                    _informationCollection.RemoveAt(0);
                }

            }
            else
            {
                current.SubStages.Add(_current);  //If not null then we add it to the parent list of substeps
            }
#endif
        }


        /// <summary>
        /// Creates a new step information on the same level as before
        /// </summary>
        /// <param name="step"></param>
        public static void NextStage(string step)
        {
#if LEAK_TRACE
            EngineStageInformation parent = _current?.Parent;
            ChangeStage(step, true); //Change out the step
            _current.Parent = parent; //Set it up to be the substep on the same level
            if (parent == null)
            {
                _informationCollection.Add(_current);

                while (_informationCollection.Count >= MaxTraceCount)
                {
                    _informationCollection.RemoveAt(0);
                }
            }
            else
            {
                parent.SubStages.Add(_current); //If not null then we add it to the parent list of substeps
            }
#endif
        }

        private static EngineStageInformation ChangeStage(string name, bool finalize)
        {
            var old = _current;
            if (finalize)
            {
                old?.FinalizeStage();
            }

            _current = new EngineStageInformation(name);
            return old;
        }
    }
}