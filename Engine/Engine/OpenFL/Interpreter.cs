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
        #region Static Properties

        /// <summary>
        /// A helper variable to accomodate funky german number parsing
        /// </summary>
        private static readonly CultureInfo NumberParsingHelper = new CultureInfo(CultureInfo.InvariantCulture.LCID);

        #endregion

        #region Public Properties

        /// <summary>
        /// A flag that indicates if the Interpreter reached the end of the script
        /// </summary>
        public bool Terminated { get; private set; }

        #endregion

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
        private const int FlHeaderArgCount = 5;

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

        #region Private Properties

        private Clapi instance;

        private FlScriptData data;

        /// <summary>
        /// A random that is used to provide random bytes
        /// </summary>
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Delegate that is used to import defines
        /// </summary>
        /// <param name="arg">The Line of the definition</param>
        private delegate void DefineHandler(Clapi instance, string[] arg, Dictionary<string, ClBufferInfo> defines,
            int width,
            int height, int depth, int channelCount, KernelDatabase kernelDb);

        /// <summary>
        /// A Dictionary containing the special functions of the interpreter, indexed by name
        /// </summary>
        private readonly Dictionary<string, FlFunctionInfo> flFunctions;

        /// <summary>
        /// The kernel database that provides the Interpreter with kernels to execute
        /// </summary>
        private KernelDatabase kernelDb;

        /// <summary>
        /// The current index in the program source
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The current word in the line at current index
        /// </summary>
        private int currentWord;

        /// <summary>
        /// The argument stack that is currently beeing worked on
        /// </summary>
        private Stack<object> currentArgStack;

        /// <summary>
        /// The active buffer
        /// </summary>
        private ClBufferInfo currentBuffer;

        /// <summary>
        /// The jump stack containing all the previous jumps
        /// </summary>
        private readonly Stack<InterpreterState> jumpStack = new Stack<InterpreterState>();

        /// <summary>
        /// The width of the input buffer
        /// </summary>
        private int width;

        /// <summary>
        /// The height of the input buffer
        /// </summary>
        private int height;

        /// <summary>
        /// The depth of the input buffer
        /// </summary>
        private int depth;

        /// <summary>
        /// The Channel count
        /// </summary>
        private int channelCount;

        /// <summary>
        /// A property that returns the input buffer size
        /// </summary>
        private int InputBufferSize => width * height * depth * channelCount;

        /// <summary>
        /// A byte array of the current active channels (0 = Inactive|1 = Active)
        /// </summary>
        private byte[] activeChannels;

        /// <summary>
        /// The memory buffer containing the active channels
        /// The buffer gets updated as soon the channel configuration changes
        /// </summary>
        private MemoryBuffer activeChannelBuffer;

        /// <summary>
        /// a flag that indicates if the stack should not be deleted(get used when returning from a jump)
        /// </summary>
        private bool leaveStack;

        /// <summary>
        /// A flag that when set to true will ignore the break statement
        /// </summary>
        private bool ignoreDebug;

        /// <summary>
        /// The current step result
        /// </summary>
        private InterpreterStepResult stepResult;

        /// <summary>
        /// The Entry point of the fl script
        /// Throws a FLInvalidEntryPointException if no main function is found
        /// </summary>
        private int EntryIndex
        {
            get
            {
                int idx = data.Source.IndexOf(EntrySignature + FunctionNamePostfix);
                if (idx == -1 || data.Source.Count - 1 == idx)
                {
                    Logger.Crash(new FLInvalidEntryPointException("There needs to be a main function."), true);
                    return 0;
                }

                return idx + 1;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="file">The file containing the source</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDb">The Kernel DB that will be used</param>
        /// <param name="ignoreDebug">a flag to ignore the brk statement</param>
        public Interpreter(Clapi instance, string file, MemoryBuffer input, int width, int height, int depth,
            int channelCount,
            KernelDatabase kernelDb,
            bool ignoreDebug)
        {
            this.instance = instance;
            flFunctions = new Dictionary<string, FlFunctionInfo>
            {
                {"setactive", new FlFunctionInfo(cmd_setactive, false)},
                {"rnd", new FlFunctionInfo(cmd_writerandom, false)},
                {"urnd", new FlFunctionInfo(cmd_writerandomu, false)},
                {"jmp", new FlFunctionInfo(cmd_jump, true)},
                {"brk", new FlFunctionInfo(cmd_break, false)}
            };


            NumberParsingHelper.NumberFormat.NumberDecimalSeparator = ",";
            NumberParsingHelper.NumberFormat.NumberGroupSeparator = ".";

            Reset(file, input, width, height, depth, channelCount, kernelDb, ignoreDebug);
        }

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="file">The file containing the source</param>
        /// <param name="genType">The Type of the data the interpreter is operating on</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDbFolder">The folder the kernel data base will be initialized in</param>
        /// <param name="ignoreDebug">a flag to ignore the brk statement</param>
        public Interpreter(Clapi instance, string file, OpenCL.TypeEnums.DataTypes genType, MemoryBuffer input,
            int width, int height,
            int depth,
            int channelCount, string kernelDbFolder,
            bool ignoreDebug) : this(instance, file, input, width, height, depth, channelCount,
            new KernelDatabase(instance, kernelDbFolder, genType), ignoreDebug)
        {
        }

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="file">The file containing the source</param>
        /// <param name="genType">The Type of the data the interpreter is operating on</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDbFolder">The folder the kernel data base will be initialized in</param>
        public Interpreter(Clapi instance, string file, OpenCL.TypeEnums.DataTypes genType, MemoryBuffer input,
            int width, int height,
            int depth,
            int channelCount, string kernelDbFolder) : this(instance, file, input, width, height, depth, channelCount,
            new KernelDatabase(instance, kernelDbFolder, genType), false)
        {
        }

        /// <summary>
        /// A public constructor
        /// </summary>
        /// <param name="instance">Clapi Instance for the current thread</param>
        /// <param name="file">The file containing the source</param>
        /// <param name="input">The input buffer</param>
        /// <param name="width">Width of the input buffer</param>
        /// <param name="height">Height of the input buffer</param>
        /// <param name="depth">Depth of the input buffer</param>
        /// <param name="channelCount">The Channel Count</param>
        /// <param name="kernelDb">The Kernel DB that will be used</param>
        public Interpreter(Clapi instance, string file, MemoryBuffer input, int width, int height, int depth,
            int channelCount,
            KernelDatabase kernelDb) : this(instance, file, input, width, height, depth, channelCount, kernelDb, false)
        {
        }

        #endregion

        #region Reset Functions

        /// <summary>
        /// Resets the Interpreter program counter and word counter to the beginning
        /// </summary>
        private void Reset()
        {
            currentIndex = EntryIndex;
            currentWord = 0;
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
        /// <param name="kernelDb">The Kernel DB that will be used</param>
        public void Reset(string file, MemoryBuffer input, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDb)
        {
            Reset(file, input, width, height, depth, channelCount, kernelDb, false);
        }

        public void ReleaseResources()
        {
            activeChannelBuffer?.Dispose();

            if (data.Defines != null)
            {
                foreach (KeyValuePair<string, ClBufferInfo> memoryBuffer in data.Defines)
                {
                    if (memoryBuffer.Value.IsInternal)
                    {
                        Logger.Log("Freeing Buffer: " + memoryBuffer.Value,
                            DebugChannel.Log | DebugChannel.OpenFl, 5);
                        memoryBuffer.Value.Buffer.Dispose();
                    }
                }
            }


            jumpStack?.Clear();
            data.Defines?.Clear();
            data.JumpLocations?.Clear();
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
        /// <param name="kernelDb">The Kernel DB that will be used</param>
        /// <param name="ignoreDebug">a flag to ignore the brk statement</param>
        public void Reset(string file, MemoryBuffer input, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDb, bool ignoreDebug)
        {
            //Clear old stuff

            ReleaseResources();

            //Setting variables
            currentBuffer = new ClBufferInfo(input, false);
            currentBuffer.SetKey(InputBufferName);

            this.ignoreDebug = ignoreDebug;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.channelCount = channelCount;
            this.kernelDb = kernelDb;
            activeChannels = new byte[this.channelCount];
            currentArgStack = new Stack<object>();
            for (int i = 0; i < this.channelCount; i++)
            {
                activeChannels[i] = 1;
            }

            activeChannelBuffer =
                Clapi.CreateBuffer(instance, activeChannels, MemoryFlag.ReadOnly | MemoryFlag.CopyHostPointer);

            //Parsing File
            currentBuffer.SetKey(InputBufferName);
            data = LoadScriptData(instance, file, currentBuffer, width, height, depth, channelCount, kernelDb,
                flFunctions);

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
            FlInstructionData data = this.data.ParsedSource[currentIndex];
            if (data.InstructionType == FlInstructionType.Nop || data.InstructionType == FlInstructionType.Unknown)
            {
                currentIndex++;
                currentWord = 0;
            }
            else
            {
                LineAnalysisResult ret = AnalyzeLine(data);
                if (ret != LineAnalysisResult.Jump)
                {
                    currentIndex++;
                    currentWord = 0;
                }
            }

            DetectEnd();
        }

        private LineAnalysisResult AnalyzeLine(FlInstructionData data)
        {
            if (data.InstructionType != FlInstructionType.FlFunction &&
                data.InstructionType != FlInstructionType.ClKernel)
            {
                Logger.Crash(new FLParseError(this.data.Source[currentIndex]), true);
                return LineAnalysisResult.ParseError;
            }

            if (leaveStack) //This keeps the stack when returning from a "function"
            {
                leaveStack = false;
            }
            else
            {
                currentArgStack = new Stack<object>();
            }

            LineAnalysisResult ret = LineAnalysisResult.IncreasePc;
            for (;
                currentWord < data.Arguments.Count;
                currentWord++) //loop through the words. start value can be != 0 when returning from a function specified as an argument to a kernel
            {
                if (data.Arguments[currentWord].argType == FlArgumentType.Function)
                {
                    bool keepBuffer = data.InstructionType == FlInstructionType.FlFunction &&
                                      ((FlFunctionInfo) data.Instruction).LeaveStack;
                    JumpTo((int) data.Arguments[currentWord].value, keepBuffer);
                    ret = LineAnalysisResult.Jump; //We Jumped to another point in the code.
                    currentArgStack
                        .Push(null); //Push null to signal the interpreter that he returned before assigning the right value.
                    break;
                }

                if (data.Arguments[currentWord].argType != FlArgumentType.Unknown)
                {
                    currentArgStack.Push(data.Arguments[currentWord].value);
                }
            }


            if (currentWord == data.Arguments.Count && ret != LineAnalysisResult.Jump)
            {
                if (data.InstructionType == FlInstructionType.FlFunction)
                {
                    ((FlFunctionInfo) data.Instruction).Run();
                    return LineAnalysisResult.IncreasePc;
                }

                CLKernel k = (CLKernel) data.Instruction;
                if (k == null || data.Arguments.Count != k.Parameter.Count - FlHeaderArgCount)
                {
                    Logger.Crash(
                        new FLInvalidFunctionUseException(this.data.Source[currentIndex],
                            "Not the right amount of arguments."),
                        true);
                    return LineAnalysisResult.ParseError;
                }

                //Execute filter
                for (int i = k.Parameter.Count - 1; i >= FlHeaderArgCount; i--)
                {
                    object obj = currentArgStack.Pop(); //Get the arguments and set them to the kernel
                    if (obj is ClBufferInfo buf) //Unpack the Buffer from the CLBuffer Object.
                    {
                        obj = buf.Buffer;
                    }

                    k.SetArg(i, obj);
                }

                Logger.Log("Running kernel: " + k.Name, DebugChannel.Log | DebugChannel.OpenFl, 8);
                Clapi.Run(instance, k, currentBuffer.Buffer, new int3(width, height, depth),
                    KernelParameter.GetDataMaxSize(kernelDb.GenDataType), activeChannelBuffer,
                    channelCount); //Running the kernel
            }

            return ret;
        }

        /// <summary>
        /// Detects if the Interpreter has reached the end of the current function
        /// </summary>
        private void DetectEnd()
        {
            if (currentIndex == data.ParsedSource.Count ||
                data.ParsedSource[currentIndex].InstructionType == FlInstructionType.FunctionHeader)
            {
                if (jumpStack.Count == 0)
                {
                    Logger.Log("Reached End of Code", DebugChannel.Log | DebugChannel.OpenFl, 9);

                    Terminated = true;
                }
                else
                {
                    InterpreterState lastState = jumpStack.Pop();

                    Logger.Log("Returning to location: " + data.Source[lastState.Line],
                        DebugChannel.Log | DebugChannel.OpenFl, 6);
                    currentIndex = lastState.Line;


                    if (lastState.ArgumentStack.Count != 0 && lastState.ArgumentStack.Peek() == null)
                    {
                        leaveStack = true;
                        lastState.ArgumentStack.Pop();
                        lastState.ArgumentStack.Push(currentBuffer);
                    }

                    currentArgStack = lastState.ArgumentStack;
                    currentBuffer = lastState.ActiveBuffer;

                    currentWord = lastState.ArgumentStack.Count;
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
            Logger.Log("Jumping To Function: " + data.Source[index], DebugChannel.OpenFl | DebugChannel.Log, 6);
            jumpStack.Push(new InterpreterState(currentIndex, currentBuffer, currentArgStack));
            stepResult.HasJumped = true;

#if NO_CL
            int size = 1;
#else
            int size = (int) currentBuffer.Buffer.Size;
#endif


            if (!leaveBuffer)
            {
                currentBuffer =
                    new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, size, MemoryFlag.ReadWrite | MemoryFlag.CopyHostPointer),
                        true);
                currentBuffer.SetKey("Internal_JumpBuffer_Stack_Index" + (jumpStack.Count - 1));
            }

            currentIndex = index + 1; //+1 because the index is the function header
            currentWord = 0;
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
        private static void ParseDefines(Clapi instance, string key, DefineHandler handler, List<string> source,
            Dictionary<string, ClBufferInfo> defines, int width, int height, int depth, int channelCount,
            KernelDatabase kernelDb)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i].StartsWith(key))
                {
                    string[] kvp = source[i].Remove(0, key.Length).Split(FunctionNamePostfix);

                    handler?.Invoke(instance, kvp, defines, width, height, depth, channelCount, kernelDb);
                    source.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Loads the source from file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="channelCount"></param>
        private static List<string> LoadSource(string file, int channelCount)
        {
            Logger.Log("Loading Source..", DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 9);

            Dictionary<string, bool> defs = new Dictionary<string, bool>();

            for (int i = 0; i < channelCount; i++)
            {
                defs.Add("channel" + i, true);
            }

            List<string> lines = TextProcessorApi.PreprocessLines(file, defs).ToList();


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

        private static FlScriptData LoadScriptData(Clapi instance, string file, ClBufferInfo inBuffer, int width,
            int height, int depth,
            int channelCount,
            KernelDatabase db, Dictionary<string, FlFunctionInfo> funcs)
        {
            Logger.Log("Loading Script Data for File: " + file,
                DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 6);

            FlScriptData ret = new FlScriptData(LoadSource(file, channelCount));


            ret.Defines.Add(InputBufferName, inBuffer);

            Logger.Log("Parsing Texture Defines for File: " + file,
                DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 5);
            ParseDefines(instance, DefineKey, DefineTexture, ret.Source, ret.Defines, width, height, depth,
                channelCount, db);

            Logger.Log("Parsing Script Defines for File: " + file,
                DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 5);
            ParseDefines(instance, ScriptDefineKey, DefineScript, ret.Source, ret.Defines, width, height, depth,
                channelCount,
                db);

            Logger.Log("Parsing JumpLocations for File: " + file,
                DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 5);
            ret.JumpLocations = ParseJumpLocations(ret.Source);

            Logger.Log("Parsing Instruction Data for File: " + file,
                DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 5);
            foreach (string line in ret.Source)
            {
                Logger.Log("Parsing Instruction Data for Line: " + line,
                    DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 3);
                FlInstructionData data = GetInstructionData(line, ret.Defines, ret.JumpLocations, funcs, db);

                Logger.Log("Parsed Instruction Data: " + Enum.GetName(typeof(FlInstructionType), data.InstructionType),
                    DebugChannel.Log | DebugChannel.OpenFl | DebugChannel.Io, 2);

                ret.ParsedSource.Add(data);
            }


            return ret;
        }

        private static FlInstructionData GetInstructionData(string line, Dictionary<string, ClBufferInfo> defines,
            Dictionary<string, int> jumpLocations, Dictionary<string, FlFunctionInfo> funcs, KernelDatabase db)
        {
            string[] code = SplitLine(SanitizeLine(line));

            if (code.Length == 0)
            {
                return new FlInstructionData {InstructionType = FlInstructionType.Nop};
            }

            if (code[0].Trim().EndsWith(FunctionNamePostfix))
            {
                return new FlInstructionData {InstructionType = FlInstructionType.FunctionHeader};
            }

            bool isBakedFunction = funcs.ContainsKey(code[0]);

            FlInstructionData ret = new FlInstructionData();

            if (isBakedFunction)
            {
                ret.InstructionType = FlInstructionType.FlFunction;
                ret.Instruction = funcs[code[0]];
            }
            else if (db.TryGetClKernel(code[0], out CLKernel kernel))
            {
                ret.Instruction = kernel;
                ret.InstructionType = FlInstructionType.ClKernel;
            }

            List<FlArgumentData> argData = new List<FlArgumentData>();
            for (int i = 1; i < code.Length; i++)
            {
                if (defines.ContainsKey(code[i]))
                {
                    argData.Add(new FlArgumentData {value = defines[code[i]], argType = FlArgumentType.Buffer});
                }
                else if (jumpLocations.ContainsKey(code[i]))
                {
                    argData.Add(
                        new FlArgumentData {value = jumpLocations[code[i]], argType = FlArgumentType.Function});
                }
                else if (decimal.TryParse(code[i], NumberStyles.Any, NumberParsingHelper, out decimal valResult))
                {
                    argData.Add(new FlArgumentData {value = valResult, argType = FlArgumentType.Number});
                }
                else
                {
                    argData.Add(new FlArgumentData {value = null, argType = FlArgumentType.Unknown});
#if !NO_CL
                    Logger.Crash(new FLInvalidArgumentType(code[i], "Number or Defined buffer."), true);
#endif
                }
            }

            ret.Arguments = argData;
            return ret;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns the currently active buffer
        /// </summary>
        /// <returns>The active buffer</returns>
        public MemoryBuffer GetActiveBuffer()
        {
            currentBuffer.SetInternalState(false);
            return currentBuffer.Buffer;
        }

        public ClBufferInfo GetBuffer(string name)
        {
            if (data.Defines.ContainsKey(name))
            {
                return data.Defines[name];
            }

            return null;
        }

        internal ClBufferInfo GetActiveBufferInternal()
        {
            return currentBuffer;
        }

        /// <summary>
        /// Returns the currently active buffer
        /// </summary>
        /// <returns>The active buffer read from the gpu and placed in cpu memory</returns>
        public T[] GetResult<T>() where T : struct
        {
            return Clapi.ReadBuffer<T>(instance, currentBuffer.Buffer, (int) currentBuffer.Buffer.Size);
        }

        /// <summary>
        /// Simulates a step on a processor
        /// </summary>
        /// <returns>The Information about the current step(mostly for debugging)</returns>
        public InterpreterStepResult Step()
        {
            stepResult = new InterpreterStepResult
            {
                SourceLine = data.Source[currentIndex]
            };

            if (Terminated)
            {
                stepResult.Terminated = true;
            }
            else
            {
                Execute();
            }

            stepResult.DebugBufferName = currentBuffer.ToString();
            stepResult.ActiveChannels = activeChannels;
            stepResult.DefinedBuffers = data.Defines.Select(x => x.Value.ToString()).ToList();
            stepResult.BuffersInJumpStack = jumpStack.Select(x => x.ActiveBuffer.ToString()).ToList();

            return stepResult;
        }

        #endregion
    }
}