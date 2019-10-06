using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MinorEngine.debug
{
    public class StepMemoryInformation
    {
        public string Name;
        public bool Finalized { get; private set; }
        public long Before;
        public long After;
        public long AfterGarbageCollection;
        public TimeSpan TimeSpentInStage;

        public StepMemoryInformation Parent;
        public List<StepMemoryInformation> SubSteps;
        public Stopwatch Timer;

        public StepMemoryInformation(string name)
        {
            Name = name;
#if !TRACE_TIME_ONLY
            Before = GC.GetTotalMemory(false) / 1024;
#endif
            SubSteps = new List<StepMemoryInformation>();
            Timer = new Stopwatch();
            Timer.Start();
        }

        public void Finalize()
        {
            Finalized = true;
#if !TRACE_TIME_ONLY
            After = GC.GetTotalMemory(false) / 1024;
            AfterGarbageCollection = GC.GetTotalMemory(true) / 1024;
#endif
            Timer.Stop();
            TimeSpentInStage = Timer.Elapsed;
        }

        public static string ToConsoleText(StepMemoryInformation info, int depth)
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
            ret += ind + "Substep Count: " + info.SubSteps.Count + "\n";
            ret += ind + "Substeps: \n";


            foreach (var stepMemoryInformation in info.SubSteps)
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
        private static List<StepMemoryInformation> _informationCollection = new List<StepMemoryInformation>();
        private static StepMemoryInformation _current;
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
#endif
            return "Engine Was compiled without MemoryTracer enabled.";
        }

        public static string cmdListLastMemoryInfo(string[] args)
        {
#if LEAK_TRACE
            if (_informationCollection.Count < 2) return ""; //We nee one frame to be finished, otherwise part of the data is not correct
            return _informationCollection[_informationCollection.Count - 2].ToString();
#endif
            return "Engine Was compiled without MemoryTracer enabled.";
        }


        /// <summary>
        /// Returns to the next higher parent step(does nothing when in root)
        /// </summary>
        public static void ReturnFromSubStep()
        {
#if LEAK_TRACE
            if (_current != null)
            {
                _current.Finalize();
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
        public static void AddSubStep(string step)
        {
#if LEAK_TRACE
            StepMemoryInformation current = _current; //Store the current step or substep
            ChangeStep(step, false);
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
                current.SubSteps.Add(_current);  //If not null then we add it to the parent list of substeps
            }
#endif
        }


        /// <summary>
        /// Creates a new step information on the same level as before
        /// </summary>
        /// <param name="step"></param>
        public static void NextStep(string step)
        {
#if LEAK_TRACE
            StepMemoryInformation parent = _current?.Parent;
            ChangeStep(step, true); //Change out the step
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
                parent.SubSteps.Add(_current); //If not null then we add it to the parent list of substeps
            }
#endif
        }

        private static StepMemoryInformation ChangeStep(string name, bool finalize)
        {
            var old = _current;
            if (finalize)
            {
                old?.Finalize();
            }

            _current = new StepMemoryInformation(name);
            return old;
        }
    }
}