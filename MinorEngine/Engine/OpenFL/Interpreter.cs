using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using Common;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.DataTypes;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.OpenCL.TypeEnums;
using Engine.WFC;
using Image = System.Drawing.Image;

namespace Engine.OpenFL
{
    /// <summary>
    /// The FL Interpreter
    /// </summary>
    public class Interpreter
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

        /// <summary>
        /// A helper variable to accomodate funky german number parsing
        /// </summary>
        private static readonly CultureInfo NumberParsingHelper = new CultureInfo(CultureInfo.InvariantCulture.LCID);



        /// <summary>
        /// A random that is used to provide random bytes
        /// </summary>
        private readonly Random rnd = new Random();

        /// <summary>
        /// Delegate that is used to import defines
        /// </summary>
        /// <param name="arg">The Line of the definition</param>
        private delegate void DefineHandler(string[] arg);

        #region Define Handler

        /// <summary>
        /// Define handler that loads defined scripts
        /// </summary>
        /// <param name="arg">The Line of the definition</param>
        private void DefineScript(string[] arg)
        {
            if (arg.Length < 2)
            {
                Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Invalid Define statement"), true);
                return;
            }

            string varname = arg[0].Trim();
            if (_definedBuffers.ContainsKey(varname))
            {
                Logger.Log("Overwriting " + varname, DebugChannel.Warning);
                _definedBuffers.Remove(varname);
            }

            string[] args = arg[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);


            string filename = args[0].Trim();


            if (IsSurroundedBy(filename, FilepathIndicator))
            {
                Logger.Log("Loading SubScript...", DebugChannel.Log);

                MemoryBuffer buf =
                    CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite);
                Interpreter interpreter = new Interpreter(filename.Replace(FilepathIndicator, ""), buf, _width, _height,
                    _depth, _channelCount, _kernelDb, true);

                do
                {
                    interpreter.Step();
                } while (!interpreter.Terminated);

                CLBufferInfo info = interpreter.GetActiveBufferInternal();
                info.SetKey(varname);
                _definedBuffers.Add(varname, info);
                interpreter.ReleaseResources();
            }
            else
            {
                Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Not a valid filepath as argument."),
                    true);
                Logger.Log("Invalid Define statement. Using empty buffer", DebugChannel.Error, 10);

                CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                info.SetKey(varname);
                AddBufferToDefine(varname, info);
            }
        }

        /// <summary>
        /// Define handler that loads defined textures
        /// </summary>
        /// <param name="arg">The Line of the definition</param>
        private void DefineTexture(string[] arg)
        {
            if (arg.Length < 2)
            {
                Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Invalid Define statement"), true);
                return;
            }

            string varname = arg[0].Trim();


            if (_definedBuffers.ContainsKey(varname))
            {
                Logger.Log("Overwriting " + varname, DebugChannel.Warning);
                _definedBuffers.Remove(varname);
            }

            MemoryFlag flags = MemoryFlag.ReadWrite;
            string[] flagTest = varname.Split(' ');
            if (flagTest.Length > 1)
            {
                varname = flagTest[1];
                if (flagTest[0] == "r")
                {
                    flags = MemoryFlag.ReadOnly;
                }

                else if (flagTest[0] == "w")
                {
                    flags = MemoryFlag.WriteOnly;
                }
            }

            string[] args = arg[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);


            string filename = args[0].Trim();


            if (IsSurroundedBy(filename, FilepathIndicator))
            {
                Bitmap bmp = (Bitmap)Image.FromFile(filename.Replace(FilepathIndicator, ""));
                AddBufferToDefine(varname,
                    new CLBufferInfo(CLAPI.CreateFromImage(bmp,
                        MemoryFlag.CopyHostPointer | flags), true));
            }
            else if (filename == "random")
            {
                MemoryBuffer buf = CLAPI.CreateEmpty<byte>(InputBufferSize, flags | MemoryFlag.CopyHostPointer);
                CLAPI.WriteRandom(buf, randombytesource, _activeChannels);

                CLBufferInfo info = new CLBufferInfo(buf, true);
                AddBufferToDefine(varname, info);
            }
            else if (filename == "empty")
            {
                CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, flags), true);
                info.SetKey(varname);
                AddBufferToDefine(varname, info);
            }
            else if (filename == "wfc")
            {
                if (args.Length < 10)
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!int.TryParse(args[2], out int n))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!int.TryParse(args[3], out int width))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!int.TryParse(args[4], out int height))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!bool.TryParse(args[5], out bool periodicInput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!bool.TryParse(args[6], out bool periodicOutput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!int.TryParse(args[7], out int symetry))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!int.TryParse(args[8], out int ground))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                if (!int.TryParse(args[9], out int limit))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }

                WaveFunctionCollapse wfc = new WFCOverlayMode(args[1].Trim().Replace(FilepathIndicator, ""), n, width,
                    height, periodicInput, periodicOutput, symetry, ground);

                wfc.Run(limit);

                Bitmap bmp = new Bitmap(wfc.Graphics(), new Size(_width, _height)); //Apply scaling
                _definedBuffers.Add(varname,
                    new CLBufferInfo(CLAPI.CreateFromImage(bmp,
                        MemoryFlag.CopyHostPointer | flags), true));
            }
            else
            {
                Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Define statement."), true);
                Logger.Log("Invalid Define statement. Using empty buffer", DebugChannel.Error, 10);
                CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                info.SetKey(varname);
                AddBufferToDefine(varname, info);
            }
        }

        #endregion


        /// <summary>
        /// A struct that gets returned every step of the interpreter. Mostly for debuggin purposes and when hitting a break point
        /// </summary>
        public struct InterpreterStepResult : IEquatable<InterpreterStepResult>
        {
            /// <summary>
            /// A flag that is set when the Interpreter jumped in the program code in the last step
            /// </summary>
            public bool HasJumped { get; set; }

            /// <summary>
            /// A flag that is set when the interpreter reached the end of the script
            /// </summary>
            public bool Terminated { get; set; }

            /// <summary>
            /// A flag that is set when the interpreter is currently at a break statement
            /// </summary>
            public bool TriggeredDebug { get; set; }

            /// <summary>
            /// The CPU Side verison of the active channels
            /// </summary>
            public byte[] ActiveChannels { get; set; }

            /// <summary>
            /// The list of Memory buffer that were defined. Matched by key
            /// </summary>
            public List<string> DefinedBuffers { get; set; }

            /// <summary>
            /// The list of Memory buffer that were defined. Matched by key
            /// </summary>
            public List<string> BuffersInJumpStack { get; set; }

            /// <summary>
            /// The Currently active buffer.
            /// </summary>
            public string DebugBufferName { get; set; }

            /// <summary>
            /// The Current line the interpreter is operating on
            /// </summary>
            public string SourceLine { get; set; }

            /// <summary>
            /// IEquatable Implementation
            /// </summary>
            /// <param name="other">Other</param>
            /// <returns>a Not implemented exception</returns>
            public bool Equals(InterpreterStepResult other)
            {
                return false;
            }

            /// <summary>
            /// String builder instance that is used when doing some more heaview string operations
            /// </summary>
            private static StringBuilder _sb = new StringBuilder();

            /// <summary>
            /// ToString Implementation
            /// </summary>
            /// <returns>Returns Current Processor State</returns>
            public override string ToString()
            {
                _sb.Clear();
                for (int i = 0; i < ActiveChannels.Length; i++)
                {
                    _sb.Append(ActiveChannels[i]);
                }

                string channels = _sb.ToString();
                _sb.Clear();
                foreach (string definedBuffer in DefinedBuffers)
                {
                    _sb.Append($"\n  {definedBuffer}");
                }
                
                string definedBuffers = _sb.ToString();

                _sb.Clear();
                foreach (string jumpBuffer in BuffersInJumpStack)
                {
                    _sb.Append($"\n  {jumpBuffer}");
                }

                string jumpBuffers = _sb.ToString();
                return
                    $"Debug Step Info:\n Active Buffer: {DebugBufferName}\n SourceLine:{SourceLine}\n HasJumped:{HasJumped}\n Triggered Breakpoint:{TriggeredDebug}\n Terminated:{Terminated}\n Active Channels:{channels}\n Defined Buffers:{definedBuffers}\n JumpStack:{jumpBuffers}";
            }
        }

        /// <summary>
        /// A Dictionary containing the special functions of the interpreter, indexed by name
        /// </summary>
        private readonly Dictionary<string, FLFunctionInfo> _flFunctions;

        /// <summary>
        /// The kernel database that provides the Interpreter with kernels to execute
        /// </summary>
        private KernelDatabase _kernelDb;

        /// <summary>
        /// The Source of the script in lines
        /// </summary>
        private List<string> _source;

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
        /// The buffers indexed by name that were defined with the DefineKey
        /// </summary>
        private readonly Dictionary<string, CLBufferInfo> _definedBuffers = new Dictionary<string, CLBufferInfo>();

        /// <summary>
        /// A list of possible jump locations
        /// </summary>
        private readonly Dictionary<string, int> _jumpLocations = new Dictionary<string, int>();

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
        /// A flag that indicates if the Interpreter reached the end of the script
        /// </summary>
        public bool Terminated { get; private set; }

        /// <summary>
        /// The Entry point of the fl script
        /// Throws a FLInvalidEntryPointException if no main function is found
        /// </summary>
        private int EntryIndex
        {
            get
            {
                int idx = _source.IndexOf(EntrySignature + FunctionNamePostfix);
                if (idx == -1 || _source.Count - 1 == idx)
                {
                    Logger.Crash(new FLInvalidEntryPointException("There needs to be a main function."), true);
                    return 0;
                }

                return idx + 1;
            }
        }




        #region FL_Functions

        /// <summary>
        /// The implementation of the command setactive
        /// </summary>
        private void cmd_setactive()
        {
            if (_currentArgStack.Count < 1)
            {
                Logger.Crash(new FLInvalidFunctionUseException("setactive", "Specify the buffer you want to activate"),
                    true);
                return;
            }

            byte[] temp = new byte[_channelCount];
            while (_currentArgStack.Count != 1)
            {
                object val = _currentArgStack.Pop();
                if (!(val is decimal))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("setactive", "Invalid channel Arguments"), true);
                    val = 0;
                }

                byte channel = (byte)Convert.ChangeType(val, typeof(byte));
                if (channel >= _channelCount)
                {
                    Logger.Log("Script is enabling channels beyond channel count. Ignoring...", DebugChannel.Warning);
                }
                else
                {
                    temp[channel] = 1;
                }
            }

            if (_currentArgStack.Peek() == null ||
                !(_currentArgStack.Peek() is CLBufferInfo) && !(_currentArgStack.Peek() is decimal))
            {
                Logger.Crash(new FLInvalidFunctionUseException("setactive", "Specify the buffer you want to activate"),
                    true);
                return;
            }

            if (_currentArgStack.Peek() is decimal)
            {
                byte channel = (byte)Convert.ChangeType(_currentArgStack.Pop(), typeof(byte));
                temp[channel] = 1;
            }
            else
            {
                _currentBuffer = (CLBufferInfo)_currentArgStack.Pop();
            }

            bool needCopy = false;
            for (int i = 0; i < _channelCount; i++)
            {
                if (_activeChannels[i] != temp[i])
                {
                    needCopy = true;
                    break;
                }
            }

            if (needCopy)
            {
                Logger.Log("Updating Channel Buffer", DebugChannel.Log);
                _activeChannels = temp;
                CLAPI.WriteToBuffer(_activeChannelBuffer, _activeChannels);
            }
        }

        /// <summary>
        /// A function used as RandomFunc of type byte>
        /// </summary>
        /// <returns>a random byte</returns>
        private byte randombytesource()
        {
            return (byte)rnd.Next();
        }

        /// <summary>
        /// The implementation of the command random
        /// </summary>
        private void cmd_writerandom()
        {
            if (_currentArgStack.Count == 0)
            {
                CLAPI.WriteRandom(_currentBuffer.Buffer, randombytesource, _activeChannels);
            }

            while (_currentArgStack.Count != 0)
            {
                object obj = _currentArgStack.Pop();
                if (!(obj is CLBufferInfo))
                {
                    Logger.Crash(
                        new FLInvalidArgumentType("Argument: " + _currentArgStack.Count + 1, "MemoyBuffer/Image"),
                        true);
                    continue;
                }

                CLAPI.WriteRandom((obj as CLBufferInfo).Buffer, randombytesource, _activeChannels);
            }
        }

        /// <summary>
        /// The implementation of the command jmp
        /// </summary>
        private void cmd_jump() //Dummy function. Implementation in AnalyzeLine(code) function(look for isDirectExecute)
        {
            Logger.Log("Jumping.", DebugChannel.Log);
        }

        /// <summary>
        /// The implementation of the command brk
        /// </summary>
        private void cmd_break()
        {
            if (_ignoreDebug)
            {
                return;
            }

            _stepResult.TriggeredDebug = true;
            if (_currentArgStack.Count == 0)
            {
                _stepResult.DebugBufferName = _currentBuffer.ToString();
            }
            else if (_currentArgStack.Count == 1)
            {
                object obj = _currentArgStack.Pop();
                if (!(obj is CLBufferInfo))
                {
                    Logger.Crash(
                        new FLInvalidArgumentType("Argument: " + _currentArgStack.Count + 1, "MemoyBuffer/Image"),
                        true);
                    return;
                }

                _stepResult.DebugBufferName = (obj as CLBufferInfo).ToString();
            }
            else
            {
                Logger.Crash(new FLInvalidFunctionUseException("Break", "only one or zero arguments"), true);
            }
        }

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
                {"random", new FLFunctionInfo(cmd_writerandom, false)},
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

            if (_definedBuffers != null)
            {
                foreach (KeyValuePair<string, CLBufferInfo> memoryBuffer in _definedBuffers)
                {
                    if (memoryBuffer.Value.IsInternal)
                    {
                        Logger.Log("Freeing Buffer: "+memoryBuffer.Value.ToString(), DebugChannel.Log);
                        memoryBuffer.Value.Buffer.Dispose();
                    }
                }
            }


            _jumpStack?.Clear();
            _definedBuffers?.Clear();
            _jumpLocations?.Clear();
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
            AddBufferToDefine(InputBufferName, _currentBuffer);

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


            LoadSource(file);

            ParseDefines(ScriptDefineKey, DefineScript);
            ParseDefines(DefineKey, DefineTexture);
            ParseJumpLocations();

            Reset();
        }

        #endregion


        private void AddBufferToDefine(string key, CLBufferInfo info)
        {
            info.SetKey(key);
            _definedBuffers.Add(key, info);
        }

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
                SourceLine = _source[_currentIndex]
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
            _stepResult.DefinedBuffers = _definedBuffers.Select(x => x.Value.ToString()).ToList();
            _stepResult.BuffersInJumpStack = _jumpStack.Select(x => x.ActiveBuffer.ToString()).ToList();

            return _stepResult;
        }

        private string SanitizeLine(string line)
        {
            return line.Split(CommentPrefix)[0];
        }

        private string[] SplitLine(string line)
        {
            return line.Split(WordSeparator, StringSplitOptions.RemoveEmptyEntries);
        }

        private void Execute()
        {
            string code = SanitizeLine(_source[_currentIndex]);
            string[] cd = SplitLine(code);
            if (cd.Length == 0)
            {
                _currentIndex++; //Next Line since this one is emtpy
                _currentWord = 1;
            }
            else
            {
                LineAnalysisResult ret = AnalyzeLine(cd);
                if (ret == LineAnalysisResult.IncreasePC)
                {
                    _currentIndex++;
                    _currentWord = 1;
                }
                else if (ret == LineAnalysisResult.ParseError)
                {
                    //Error Ocurred. We fix the error by pretending it didnt happen and we treat the failed instruction as a NOP
                    _currentIndex++;
                    _currentWord = 1;
                }
            }
            DetectEnd();
        }




        /// <summary>
        /// Finds all jump locations inside the script
        /// </summary>
        private void ParseJumpLocations()
        {
            for (int i = _source.Count - 1; i >= 0; i--)
            {
                if (_source[i].EndsWith(FunctionNamePostfix) && _source.Count - 1 != i)
                {
                    _jumpLocations.Add(_source[i].Remove(_source[i].Length - 1, 1), i + 1);
                }
            }
        }


        /// <summary>
        /// Analyzes a line of code
        /// </summary>
        /// <param name="code">the line to analyze</param>
        /// <returns>True if the program counter should be increased</returns>
        private LineAnalysisResult AnalyzeLine(string[] words)
        {
            string function = words[0];
            CLKernel kernel = null;

            bool isBakedFunction = _flFunctions.ContainsKey(function);
            bool keepBuffer = isBakedFunction && _flFunctions[function].LeaveStack;

            if (!isBakedFunction && !_kernelDb.TryGetCLKernel(function, out kernel))
            {
                Logger.Crash(new FLParseError(_source[_currentIndex]), true);
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
                _currentWord < words.Length;
                _currentWord++) //loop through the words. start value can be != 0 when returning from a function specified as an argument to a kernel
            {
                if (AnalyzeWord(words[_currentWord], out object val))
                {
                    JumpTo(_jumpLocations[words[_currentWord]], keepBuffer);
                    ret = LineAnalysisResult.Jump; //We Jumped to another point in the code.
                    _currentArgStack
                        .Push(null); //Push null to signal the interpreter that he returned before assigning the right value.
                    break;
                }
                else
                {
                    _currentArgStack.Push(val); //push the value to the stack
                }
            }

            if (_currentWord == words.Length && ret != LineAnalysisResult.Jump) //We finished parsing the line and we didnt jump.
            {
                if (isBakedFunction)
                {
                    _flFunctions[function].Run(); //Execute baked function
                }
                else if (kernel == null || words.Length - 1 != kernel.Parameter.Count - FLHeaderArgCount)
                {
                    Logger.Crash(new FLInvalidFunctionUseException(function, "Not the right amount of arguments."),
                        true);
                    return LineAnalysisResult.ParseError;
                }
                else
                {
                    //Execute filter
                    for (int i = kernel.Parameter.Count - 1; i >= FLHeaderArgCount; i--)
                    {
                        object obj = _currentArgStack.Pop(); //Get the arguments and set them to the kernel
                        if (obj is CLBufferInfo buf) //Unpack the Buffer from the CLBuffer Object.
                        {
                            obj = buf.Buffer;
                        }
                        kernel.SetArg(i, obj);
                    }

                    Logger.Log("Running kernel: " + function, DebugChannel.Log);
                    CLAPI.Run(kernel, _currentBuffer.Buffer, new int3(_width, _height, _depth),
                        KernelParameter.GetDataMaxSize(_kernelDb.GenDataType), _activeChannelBuffer,
                        _channelCount); //Running the kernel
                }
            }

            return ret;
        }

        /// <summary>
        /// Analyzes a single word of a line
        /// </summary>
        /// <param name="word">The input word</param>
        /// <param name="val">the parsed output</param>
        /// <returns>if the word could be parsed</returns>
        private bool AnalyzeWord(string word, out object val)
        {
            if (_jumpLocations.ContainsKey(word))
            {
                Logger.Log("Jumping to location: " + word, DebugChannel.Log);

                val = null;
                return true;
            }

            val = null;
            if (_definedBuffers.ContainsKey(word))
            {
                val = _definedBuffers[word];
            }
            else if (decimal.TryParse(word, NumberStyles.Any, NumberParsingHelper, out decimal numberDecimal))
            {
                val = numberDecimal;
            }

#if !NO_CL
            if (val == null)
            {
                Logger.Crash(new FLInvalidArgumentType(word, "Number or Defined buffer."), true);
            }
#endif
            val = val ?? "PLACEHOLDER";
            return false;
        }

        /// <summary>
        /// Resets the Interpreter program counter and word counter to the beginning
        /// </summary>
        private void Reset()
        {
            _currentIndex = EntryIndex;
            _currentWord = 1;
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

        /// <summary>
        /// Finds, Parses and Loads all define statements
        /// </summary>
        private void ParseDefines(string key, DefineHandler handler)
        {
            for (int i = _source.Count - 1; i >= 0; i--)
            {
                if (_source[i].StartsWith(key))
                {
                    string[] kvp = _source[i].Remove(0, key.Length).Split(FunctionNamePostfix);

                    handler?.Invoke(kvp);
                    _source.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Detects if the Interpreter has reached the end of the current function
        /// </summary>
        private void DetectEnd()
        {
            if (_currentIndex == _source.Count || _source[_currentIndex].EndsWith(FunctionNamePostfix))
            {
                if (_jumpStack.Count == 0)
                {
                    Logger.Log("Reached End of Code", DebugChannel.Log);

                    Terminated = true;
                }
                else
                {
                    InterpreterState lastState = _jumpStack.Pop();

                    Logger.Log("Returning to location: " + _source[lastState.Line], DebugChannel.Log);
                    _currentIndex = lastState.Line;


                    if (lastState.ArgumentStack.Count != 0 && lastState.ArgumentStack.Peek() == null)
                    {
                        _leaveStack = true;
                        lastState.ArgumentStack.Pop();
                        lastState.ArgumentStack.Push(_currentBuffer);
                    }

                    _currentArgStack = lastState.ArgumentStack;
                    _currentBuffer = lastState.ActiveBuffer;

                    _currentWord = lastState.ArgumentStack.Count + 1;
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
            int size = (int)_currentBuffer.Buffer.Size;
            if (!leaveBuffer)
            {
                _currentBuffer = new CLBufferInfo(CLAPI.CreateEmpty<byte>(size, MemoryFlag.ReadWrite | MemoryFlag.CopyHostPointer), true);
                _currentBuffer.SetKey("Internal_JumpBuffer_Stack_Index" + (_jumpStack.Count - 1));
            }

            _currentIndex = index;
            _currentWord = 1;
        }

        /// <summary>
        /// Loads the source from file
        /// </summary>
        /// <param name="file"></param>
        private void LoadSource(string file)
        {
            Logger.Log("Loading Source..", DebugChannel.Log);

            Dictionary<string, bool> defs = new Dictionary<string, bool>();

            for (int i = 0; i < _channelCount; i++)
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

            _source = lines;
        }
    }
}