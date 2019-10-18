using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Debug
{
    /// <summary>
    /// Class used to Raise Statistics about the engines different stages
    /// </summary>
    public static class MemoryTracer
    {
        /// <summary>
        /// List of the last frames informations
        /// </summary>
        private static List<EngineStageInformation> _informationCollection = new List<EngineStageInformation>();

        /// <summary>
        /// The Console
        /// </summary>
        private static DebugConsoleComponent _console;


        /// <summary>
        /// Setter that sets the console
        /// </summary>
        /// <param name="component"></param>
        internal static void SetDebugComponent(DebugConsoleComponent component)
        {
            _console = component;
        }

        /// <summary>
        /// Current active stage
        /// </summary>
        private static EngineStageInformation _current;

        /// <summary>
        /// The maximum history of informations that is kept
        /// </summary>
        public static int MaxTraceCount = 10;


        /// <summary>
        /// Command for the Debug Console to list the current History of the Engine Stages
        /// </summary>
        /// <param name="args">Parameters provided by the user(0)</param>
        /// <returns>Command Result</returns>
        public static string cmdListMemoryInfo(string[] args)
        {
#if LEAK_TRACE
            string ret = "";
            foreach (EngineStageInformation stepMemoryInformation in _informationCollection)
            {
                if (stepMemoryInformation.Finalized)
                {
                    ret += stepMemoryInformation.ToString();
                }
            }

            return ret;
#else
            return "Engine Was compiled without MemoryTracer enabled.";
#endif
        }

        /// <summary>
        /// Command for the Debug Console to list the last complete Stage info of the Engine Stages
        /// </summary>
        /// <param name="args">Parameters provided by the user(0)</param>
        /// <returns>Command Result</returns>
        public static string cmdListLastMemoryInfo(string[] args)
        {
#if LEAK_TRACE
            if (_informationCollection.Count < 2)
            {
                return ""; //We nee one frame to be finished, otherwise part of the data is not correct
            }

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
                current.SubStages.Add(_current); //If not null then we add it to the parent list of substeps
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

            EngineStageInformation cur = _current;

            ChangeStage(step, true); //Change out the step
            _current.Parent = parent; //Set it up to be the substep on the same level
            if (parent == null)
            {
                _informationCollection.Add(_current);

                if (cur != null)
                {
                    _console?.AddGraphValue((float) cur.TimeSpentInStage.TotalMilliseconds);
                }


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

        /// <summary>
        /// Changes the Stage to the next stage/ or a substage
        /// </summary>
        /// <param name="name">Name of the next stage</param>
        /// <param name="finalize">If true it will end the last stage(e.g. its not a substage)</param>
        /// <returns></returns>
        private static EngineStageInformation ChangeStage(string name, bool finalize)
        {
            EngineStageInformation old = _current;
            if (finalize)
            {
                old?.FinalizeStage();
            }

            _current = new EngineStageInformation(name);
            return old;
        }
    }
}