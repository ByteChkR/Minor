using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine.Common;
using Engine.Debug;
using Engine.Exceptions;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.DataTypes;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenFL.FLDataObjects;

namespace Engine.OpenFL
{
    /// <summary>
    /// The FL Interpreter
    /// </summary>
    public partial class Interpreter
    {
        #region Constant Keywords

        /// <summary>
        /// The key to look for when parsing defined textures
        /// </summary>
        private const string DefineKey = "--define texture ";

        /// <summary>
        /// The key to look for when parsing defined scripts
        /// </summary>
        private const string ScriptDefineKey = "--define script ";

        /// <summary>
        /// FL header Count(the offset from 0 where the "user" parameter start)
        /// </summary>
        private const int FLHeaderArgCount = 5;

        /// <summary>
        /// Everything past this string gets ignored by the interpreter
        /// </summary>
        private const string CommentPrefix = "#";

        /// <summary>
        /// The function name that is used as the starting function
        /// </summary>
        private const string EntrySignature = "Main";

        /// <summary>
        /// A buffer that is defined by default.
        /// (The input buffer contains the texture that the script is operating on)
        /// </summary>
        private const string InputBufferName = "in";

        /// <summary>
        /// The Symbol that is used to determine if the line is a function header
        /// </summary>
        private const string FunctionNamePostfix = ":";

        /// <summary>
        /// The Separator that is used to separate words(instructions and arguments)
        /// </summary>
        private const string WordSeparator = " ";

        /// <summary>
        /// The Symbol that indicates a filepath. (has to be surrounded e.g. "/path/to/file")
        /// </summary>
        private const string FilepathIndicator = "\"";

        #endregion

        #region Static Properties

        /// <summary>
        /// A helper variable to accomodate funky german number parsing
        /// </summary>
        private static readonly CultureInfo NumberParsingHelper = new CultureInfo(CultureInfo.InvariantCulture.LCID);

        #endregion

        #region Private Properties

        private FLScriptData Data;

        /// <summary>
        /// A random that is used to provide random bytes
        /// </summary>
        private static readonly Random rnd = new Random();

        /// <summary>
        /// Delegate that is used to import defines
        /// </summary>
        /// <param name="arg">The Line of the definition</param>
        private delegate void DefineHandler(string[] arg, Dictionary<string, CLBufferInfo> defines, int width,
            int height, int depth, int channelCount, KernelDatabase kernelDb);

        /// <summary>
        /// A Dictionary containing the special functions of the interpreter, indexed by name
        /// </summary>
        private readonly Dictionary<string, FLFunctionInfo> _flFunctions;

        /// <summary>
        /// The kernel database that provides the Interpreter with kernels to execute
        /// </summary>
        private KernelDatabase _kernelDb;

        /// <summary>
        /// The current index in the program source
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// The current word in the line at current index
        /// </summary>
        private int _currentWord;

        /// <summary>
        /// The argument stack that is currently beeing worked on
        /// </summary>
        private Stack<object> _currentArgStack;

        /// <summary>
        /// The active buffer
        /// </summary>
        private CLBufferInfo _currentBuffer;

        /// <summary>
        /// The jump stack containing all the previous jumps
        /// </summary>
        private readonly Stack<InterpreterState> _jumpStack = new Stack<InterpreterState>();

        /// <summary>
        /// The width of the input buffer
        /// </summary>
        private int _width;

        /// <summary>
        /// The height of the input buffer
        /// </summary>
        private int _height;

        /// <summary>
        /// The depth of the input buffer
        /// </summary>
        private int _depth;

        /// <summary>
        /// The Channel count
        /// </summary>
        private int _channelCount;

        /// <summary>
        /// A property that returns the input buffer size
        /// </summary>
        private int InputBufferSize => _width * _height * _depth * _channelCount;

        /// <summary>
        /// A byte array of the current active channels (0 = Inactive|1 = Active)
        /// </summary>
        private byte[] _activeChannels;

        /// <summary>
        /// The memory buffer containing the active channels
        /// The buffer gets updated as soon the channel configuration changes
        /// </summary>
        private MemoryBuffer _activeChannelBuffer;

        /// <summary>
        /// a flag that indicates if the stack should not be deleted(get used when returning from a jump)
        /// </summary>
        private bool _leaveStack;

        /// <summary>
        /// A flag that when set to true will ignore the break statement
        /// </summary>
        private bool _ignoreDebug;

        /// <summary>
        /// The current step result
        /// </summary>
        private InterpreterStepResult _stepResult;

        /// <summary>
        /// The Entry point of the fl script
        /// Throws a FLInvalidEntryPointException if no main function is found
        /// </summary>
        private int EntryIndex
        {
            get
            {
                int idx = Data.Source.IndexOf(EntrySignature + FunctionNamePostfix);
                if (idx == -1 || Data.Source.Count - 1 == idx)
                {
                    Logger.Crash(new FLInvalidEntryPointException("There needs to be a main function."), true);
                    return 0;
                }

                return idx + 1;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A flag that indicates if the Interpreter reached the end of the script
        /// </summary>
        public bool Terminated { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="file">The file containing the source</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDB">The Kernel DB that will be used</param>
        /// <param name="ignoreDebug">a flag to ignore the brk statement</param>
        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDB,
            bool ignoreDebug)
        {
            _flFunctions = new Dictionary<string, FLFunctionInfo>
            {
                {"setactive", new FLFunctionInfo(cmd_setactive, false)},
                {"rnd", new FLFunctionInfo(cmd_writerandom, false)},
                {"urnd", new FLFunctionInfo(cmd_writerandomu, false)},
                {"jmp", new FLFunctionInfo(cmd_jump, true)},
                {"brk", new FLFunctionInfo(cmd_break, false)}
            };


            NumberParsingHelper.NumberFormat.NumberDecimalSeparator = ",";
            NumberParsingHelper.NumberFormat.NumberGroupSeparator = ".";

            Reset(file, input, width, height, depth, channelCount, kernelDB, ignoreDebug);
        }

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="file">The file containing the source</param>
        /// <param name="genType">The Type of the data the interpreter is operating on</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDBFolder">The folder the kernel data base will be initialized in</param>
        /// <param name="ignoreDebug">a flag to ignore the brk statement</param>
        public Interpreter(string file, OpenCL.TypeEnums.DataTypes genType, MemoryBuffer input, int width, int height,
            int depth,
            int channelCount, string kernelDBFolder,
            bool ignoreDebug) : this(file, input, width, height, depth, channelCount,
            new KernelDatabase(kernelDBFolder, genType), ignoreDebug)
        {
        }

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="file">The file containing the source</param>
        /// <param name="genType">The Type of the data the interpreter is operating on</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDBFolder">The folder the kernel data base will be initialized in</param>
        public Interpreter(string file, OpenCL.TypeEnums.DataTypes genType, MemoryBuffer input, int width, int height,
            int depth,
            int channelCount, string kernelDBFolder) : this(file, input, width, height, depth, channelCount,
            new KernelDatabase(kernelDBFolder, genType), false)
        {
        }

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="file">The file containing the source</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDB">The Kernel DB that will be used</param>
        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDB) : this(file, input, width, height, depth, channelCount, kernelDB, false)
        {
        }

        #endregion

        #region Reset Functions

        /// <summary>
        /// Resets the Interpreter program counter and word counter to the beginning
        /// </summary>
        private void Reset()
        {
            _currentIndex = EntryIndex;
            _currentWord = 0;
        }

        /// <summary>
        /// Resets the Interpreter to work with a new script
        /// </summary>
        /// <param name="file">The file containing the source</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDB">The Kernel DB that will be used</param>
        public void Reset(string file, MemoryBuffer input, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDB)
        {
            Reset(file, input, width, height, depth, channelCount, kernelDB, false);
        }

        public void ReleaseResources()
        {
            _activeChannelBuffer?.Dispose();

            if (Data.Defines != null)
            {
                foreach (KeyValuePair<string, CLBufferInfo> memoryBuffer in Data.Defines)
                {
                    if (memoryBuffer.Value.IsInternal)
                    {
                        Logger.Log("Freeing Buffer: " + memoryBuffer.Value.ToString(), DebugChannel.Log);
                        memoryBuffer.Value.Buffer.Dispose();
                    }
                }
            }


            _jumpStack?.Clear();
            Data.Defines?.Clear();
            Data.JumpLocations?.Clear();
        }

        /// <summary>
        /// Resets the Interpreter to work with a new script
        /// </summary>
        /// <param name="file">The file containing the source</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDB">The Kernel DB that will be used</param>
        /// <param name="ignoreDebug">a flag to ignore the brk statement</param>
        public void Reset(string file, MemoryBuffer input, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDB, bool ignoreDebug)
        {
            //Clear old stuff

            ReleaseResources();

            //Setting variables
            _currentBuffer = new CLBufferInfo(input, false);
            _currentBuffer.SetKey(InputBufferName);

            _ignoreDebug = ignoreDebug;
            _width = width;
            _height = height;
            _depth = depth;
            _channelCount = channelCount;
            _kernelDb = kernelDB;
            _activeChannels = new byte[_channelCount];
            _currentArgStack = new Stack<object>();
            for (int i = 0; i < _channelCount; i++)
            {
                _activeChannels[i] = 1;
            }

            _activeChannelBuffer =
                CLAPI.CreateBuffer(_activeChannels, MemoryFlag.ReadOnly | MemoryFlag.CopyHostPointer);

            //Parsing File
            _currentBuffer.SetKey(InputBufferName);
            Data = LoadScriptData(file, _currentBuffer, width, height, depth, channelCount, _kernelDb, _flFunctions);

            Reset();
        }

        #endregion

        #region String Operations

        /// <summary>
        /// Returns the Code part(removes the comments)
        /// </summary>
        /// <param name="line">The line to be Sanizied</param>
        /// <returns>The Sanizied line</returns>
        private static string SanitizeLine(string line)
        {
            return line.Split(CommentPrefix)[0];
        }

        /// <summary>
        /// Splits the line into words
        /// </summary>
        /// <param name="line">the line to be split</param>
        /// <returns></returns>
        private static string[] SplitLine(string line)
        {
            return line.Split(WordSeparator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Returns true if the text is surrounded by the surrStr
        /// </summary>
        /// <param name="text">The text to be checked</param>
        /// <param name="surrStr">the pattern to search for</param>
        /// <returns>True of the text is surrounded by the surrStr</returns>
        private static bool IsSurroundedBy(string text, string surrStr)
        {
            return text.StartsWith(surrStr) && text.EndsWith(surrStr);
        }

        #endregion

        #region Execution

        /// <summary>
        /// Executes one step of the Processor
        /// </summary>
        private void Execute()
        {
            FLInstructionData data = Data.ParsedSource[_currentIndex];
            if (data.InstructionType == FLInstructionType.NOP || data.InstructionType == FLInstructionType.Unknown)
            {
                _currentIndex++;
                _currentWord = 0;
            }
            else
            {
                LineAnalysisResult ret = AnalyzeLine(data);
                if (ret != LineAnalysisResult.Jump)
                {
                    _currentIndex++;
                    _currentWord = 0;
                }
            }
            DetectEnd();
        }

        private LineAnalysisResult AnalyzeLine(FLInstructionData data)
        {
            if (data.InstructionType != FLInstructionType.FLFunction && data.InstructionType != FLInstructionType.CLKernel)
            {
                Logger.Crash(new FLParseError(Data.Source[_currentIndex]), true);
                return LineAnalysisResult.ParseError;
            }

            if (_leaveStack) //This keeps the stack when returning from a "function"
            {
                _leaveStack = false;
            }
            else
            {
                _currentArgStack = new Stack<object>();
            }

            LineAnalysisResult ret = LineAnalysisResult.IncreasePC;
            for (;
                _currentWord < data.Arguments.Count;
                _currentWord++) //loop through the words. start value can be != 0 when returning from a function specified as an argument to a kernel
            {
                if (data.Arguments[_currentWord].argType == FLArgumentType.Function)
                {
                    bool KeepBuffer = data.InstructionType == FLInstructionType.FLFunction && ((FLFunctionInfo)data.Instruction).LeaveStack;
                    JumpTo((int)data.Arguments[_currentWord].value, KeepBuffer);
                    ret = LineAnalysisResult.Jump; //We Jumped to another point in the code.
                    _currentArgStack
                        .Push(null); //Push null to signal the interpreter that he returned before assigning the right value.
                    break;
                }
                if (data.Arguments[_currentWord].argType != FLArgumentType.Unknown)
                {
                    _currentArgStack.Push(data.Arguments[_currentWord].value);
                }
            }


            if (_currentWord == data.Arguments.Count && ret != LineAnalysisResult.Jump)
            {
                if (data.InstructionType == FLInstructionType.FLFunction)
                {
                    ((FLFunctionInfo)data.Instruction).Run();
                    return LineAnalysisResult.IncreasePC;
                }

                CLKernel K = (CLKernel)data.Instruction;
                if (K == null || data.Arguments.Count != K.Parameter.Count - FLHeaderArgCount)
                {
                    Logger.Crash(new FLInvalidFunctionUseException(Data.Source[_currentIndex], "Not the right amount of arguments."),
                        true);
                    return LineAnalysisResult.ParseError;
                }

                //Execute filter
                for (int i = K.Parameter.Count - 1; i >= FLHeaderArgCount; i--)
                {
                    object obj = _currentArgStack.Pop(); //Get the arguments and set them to the kernel
                    if (obj is CLBufferInfo buf) //Unpack the Buffer from the CLBuffer Object.
                    {
                        obj = buf.Buffer;
                    }

                    K.SetArg(i, obj);
                }

                Logger.Log("Running kernel: " + K.Name, DebugChannel.Log);
                CLAPI.Run(K, _currentBuffer.Buffer, new int3(_width, _height, _depth),
                    KernelParameter.GetDataMaxSize(_kernelDb.GenDataType), _activeChannelBuffer,
                    _channelCount); //Running the kernel

            }
            return ret;
        }

        /// <summary>
        /// Detects if the Interpreter has reached the end of the current function
        /// </summary>
        private void DetectEnd()
        {
            if (_currentIndex == Data.ParsedSource.Count || Data.ParsedSource[_currentIndex].InstructionType == FLInstructionType.FunctionHeader)
            {
                if (_jumpStack.Count == 0)
                {
                    Logger.Log("Reached End of Code", DebugChannel.Log);

                    Terminated = true;
                }
                else
                {
                    InterpreterState lastState = _jumpStack.Pop();

                    Logger.Log("Returning to location: " + Data.Source[lastState.Line], DebugChannel.Log);
                    _currentIndex = lastState.Line;


                    if (lastState.ArgumentStack.Count != 0 && lastState.ArgumentStack.Peek() == null)
                    {
                        _leaveStack = true;
                        lastState.ArgumentStack.Pop();
                        lastState.ArgumentStack.Push(_currentBuffer);
                    }

                    _currentArgStack = lastState.ArgumentStack;
                    _currentBuffer = lastState.ActiveBuffer;

                    _currentWord = lastState.ArgumentStack.Count;
                }
            }
        }

        /// <summary>
        /// Jumps the interpreter to the specified index
        /// </summary>
        /// <param name="index">the index of the line to jump to</param>
        /// <param name="leaveBuffer">a flag to optionally keep the current buffer</param>
        private void JumpTo(int index, bool leaveBuffer = false)
        {
            _jumpStack.Push(new InterpreterState(_currentIndex, _currentBuffer, _currentArgStack));
            _stepResult.HasJumped = true;

#if NO_CL
            int size = 1;
#else
            int size = (int)_currentBuffer.Buffer.Size;
#endif


            if (!leaveBuffer)
            {
                _currentBuffer =
                    new CLBufferInfo(CLAPI.CreateEmpty<byte>(size, MemoryFlag.ReadWrite | MemoryFlag.CopyHostPointer),
                        true);
                _currentBuffer.SetKey("Internal_JumpBuffer_Stack_Index" + (_jumpStack.Count - 1));
            }

            _currentIndex = index;
            _currentWord = 0;
        }

        #endregion

        #region Parsing

        /// <summary>
        /// Finds all jump locations inside the script
        /// </summary>
        private static Dictionary<string, int> ParseJumpLocations(List<string> source)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i].EndsWith(FunctionNamePostfix) && source.Count - 1 != i)
                {
                    ret.Add(source[i].Remove(source[i].Length - 1, 1), i);
                }
            }

            return ret;
        }

        /// <summary>
        /// Finds, Parses and Loads all define statements
        /// </summary>
        private static void ParseDefines(string key, DefineHandler handler, List<string> source,
            Dictionary<string, CLBufferInfo> defines, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDb)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i].StartsWith(key))
                {
                    string[] kvp = source[i].Remove(0, key.Length).Split(FunctionNamePostfix);

                    handler?.Invoke(kvp, defines, width, height, depth, channelCount, kernelDb);
                    source.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Loads the source from file
        /// </summary>
        /// <param name="file"></param>
        private static List<string> LoadSource(string file, int channelCount)
        {
            Logger.Log("Loading Source..", DebugChannel.Log);

            Dictionary<string, bool> defs = new Dictionary<string, bool>();

            for (int i = 0; i < channelCount; i++)
            {
                defs.Add("channel" + i, true);
            }

            List<string> lines = TextProcessorAPI.PreprocessLines(file, defs).ToList();


            for (int i = lines.Count - 1; i >= 0; i--)
            {
                string line = lines[i].Trim();
                if (line.StartsWith(CommentPrefix))
                {
                    lines.RemoveAt(i); //Remove otherwise emtpty lines after removing comments
                }
                else
                {
                    lines[i] = line.Split(CommentPrefix)[0].Trim();
                }
            }

            return lines;
        }

        #endregion

        private static FLScriptData LoadScriptData(string file, CLBufferInfo inBuffer, int width, int height, int depth, int channelCount,
            KernelDatabase db, Dictionary<string, FLFunctionInfo> funcs)
        {
            Logger.Log("Loading Script Data for File: " + file, DebugChannel.Log);

            FLScriptData ret = new FLScriptData(LoadSource(file, channelCount));


            Logger.Log("Parsing JumpLocations for File: " + file, DebugChannel.Log);
            ret.JumpLocations = ParseJumpLocations(ret.Source);

            ret.Defines.Add(InputBufferName, inBuffer);

            Logger.Log("Parsing Texture Defines for File: " + file, DebugChannel.Log);
            ParseDefines(DefineKey, DefineTexture, ret.Source, ret.Defines, width, height, depth, channelCount, db);

            Logger.Log("Parsing Script Defines for File: " + file, DebugChannel.Log);
            ParseDefines(ScriptDefineKey, DefineScript, ret.Source, ret.Defines, width, height, depth, channelCount, db);


            Logger.Log("Parsing Instruction Data for File: " + file, DebugChannel.Log);
            foreach (string line in ret.Source)
            {
                Logger.Log("Parsing Instruction Data for Line: " + line, DebugChannel.Log);
                FLInstructionData data = GetInstructionData(line, ret.Defines, ret.JumpLocations, funcs, db);

                Logger.Log("Parsed Instruction Data: " + Enum.GetName(typeof(FLInstructionType), data.InstructionType), DebugChannel.Log);

                ret.ParsedSource.Add(data);
            }


            return ret;
        }

        private static FLInstructionData GetInstructionData(string line, Dictionary<string, CLBufferInfo> defines,
            Dictionary<string, int> jumpLocations, Dictionary<string, FLFunctionInfo> funcs, KernelDatabase db)
        {

            string[] code = SplitLine(SanitizeLine(line));

            if (code.Length == 0)
            {
                return new FLInstructionData() { InstructionType = FLInstructionType.NOP };
            }
            if (code[0].Trim().EndsWith(FunctionNamePostfix))
            {
                return new FLInstructionData() { InstructionType = FLInstructionType.FunctionHeader };
            }

            bool isBakedFunction = funcs.ContainsKey(code[0]);

            FLInstructionData ret = new FLInstructionData();

            if (isBakedFunction)
            {
                ret.InstructionType = FLInstructionType.FLFunction;
                ret.Instruction = funcs[code[0]];
            }
            else if (db.TryGetCLKernel(code[0], out CLKernel kernel))
            {
                ret.Instruction = kernel;
                ret.InstructionType = FLInstructionType.CLKernel;
            }

            List<FLArgumentData> argData = new List<FLArgumentData>();
            for (int i = 1; i < code.Length; i++)
            {
                if (defines.ContainsKey(code[i]))
                {
                    argData.Add(new FLArgumentData() { value = defines[code[i]], argType = FLArgumentType.Buffer });
                }
                else if (jumpLocations.ContainsKey(code[i]))
                {
                    argData.Add(new FLArgumentData() { value = jumpLocations[code[i]], argType = FLArgumentType.Function });
                }
                else if (decimal.TryParse(code[i], NumberStyles.Any, NumberParsingHelper, out decimal valResult))
                {
                    argData.Add(new FLArgumentData() { value = valResult, argType = FLArgumentType.Number });
                }
                else
                {
                    argData.Add(new FLArgumentData() { value = null, argType = FLArgumentType.Unknown });
#if !NO_CL
                    Logger.Crash(new FLInvalidArgumentType(code[i], "Number or Defined buffer."), true);
#endif
                }
            }

            ret.Arguments = argData;
            return ret;

        }

        #region Public Functions

        /// <summary>
        /// Returns the currently active buffer
        /// </summary>
        /// <returns>The active buffer</returns>
        public MemoryBuffer GetActiveBuffer()
        {
            _currentBuffer.SetInternalState(false);
            return _currentBuffer.Buffer;
        }

        internal CLBufferInfo GetActiveBufferInternal()
        {
            return _currentBuffer;
        }

        /// <summary>
        /// Returns the currently active buffer
        /// </summary>
        /// <returns>The active buffer read from the gpu and placed in cpu memory</returns>
        public T[] GetResult<T>() where T : struct
        {
            return CLAPI.ReadBuffer<T>(_currentBuffer.Buffer, (int)_currentBuffer.Buffer.Size);
        }

        /// <summary>
        /// Simulates a step on a processor
        /// </summary>
        /// <returns>The Information about the current step(mostly for debugging)</returns>
        public InterpreterStepResult Step()
        {
            _stepResult = new InterpreterStepResult
            {
                SourceLine = Data.Source[_currentIndex]
            };

            if (Terminated)
            {
                _stepResult.Terminated = true;
            }
            else
            {
                Execute();
            }

            _stepResult.DebugBufferName = _currentBuffer.ToString();
            _stepResult.ActiveChannels = _activeChannels;
            _stepResult.DefinedBuffers = Data.Defines.Select(x => x.Value.ToString()).ToList();
            _stepResult.BuffersInJumpStack = _jumpStack.Select(x => x.ActiveBuffer.ToString()).ToList();

            return _stepResult;
        }

        #endregion
    }
}