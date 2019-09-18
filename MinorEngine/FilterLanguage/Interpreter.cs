using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using CLHelperLibrary;
using Common;
using Common.Exceptions;
using FilterLanguage.Generators;
using OpenCl.DotNetCore.DataTypes;
using OpenCl.DotNetCore.Memory;
using Image = System.Drawing.Image;

namespace FilterLanguage
{

    public class Interpreter
    {
        private const string DefineKey = "--define texture ";
        private static readonly CultureInfo NumberParsingHelper = new CultureInfo(CultureInfo.InvariantCulture.LCID);
        private delegate void FlFunction();
        public struct InterpreterStepResult
        {
            public bool HasJumped;
            public bool Terminated;
            public bool TriggeredDebug;
            public MemoryBuffer DebugBuffer;
        }


        private static Dictionary<string, FlFunction> _flFunctions;

        private KernelDatabase _kernelDb = null;

        private List<string> _source;
        private int _currentIndex;
        private int _currentWord;
        private Stack<object> _currentArgStack;
        private MemoryBuffer _currentBuffer;
        private readonly Stack<InterpreterState> _jumpStack = new Stack<InterpreterState>();
        private int _width, _height, _depth, _channelCount;
        private int InputBufferSize => _width * _height * _depth * _channelCount;
        private byte[] _activeChannels;
        private MemoryBuffer _activeChannelBuffer;


        private readonly Dictionary<string, MemoryBuffer> _definedBuffers = new Dictionary<string, MemoryBuffer>();
        private readonly Dictionary<string, int> _jumpLocations = new Dictionary<string, int>();
        private bool _leaveStack = false;
        private bool _ignoreDebug = false;
        private InterpreterStepResult stepResult;
        private int EntryIndex
        {
            get
            {
                int idx = _source.IndexOf("Main:");
                if (idx == -1 || _source.Count - 1 == idx)
                    throw new Exception("There needs to be a main function.");
                return idx + 1;
            }
        }


        public bool Terminated { get; private set; }


        #region FL_Functions

        private void cmd_setactive()
        {
            if (_currentArgStack.Count < 1)
            {

                this.Crash(new FL_InvalidFunctionUse("setactive", "Specify the buffer you want to activate"));

            }

            byte[] temp = new byte[_channelCount];
            while (_currentArgStack.Count != 1)
            {
                object val = _currentArgStack.Pop();
                if (!(val is decimal))
                {
                    this.Crash(new FL_InvalidFunctionUse("setactive", "Invalid channel Arguments"));
                }

                byte channel = (byte)Convert.ChangeType(val, typeof(byte));
                if (channel >= _channelCount)
                {
                    this.Log("Script is enabling channels beyond channel count. Ignoring...", DebugChannel.Warning);
                }
                else
                {
                    temp[channel] = 1;
                }
            }

            if (_currentArgStack.Peek() == null || (!(_currentArgStack.Peek() is MemoryBuffer) && !(_currentArgStack.Peek() is decimal)))
            {
                this.Crash(new FL_InvalidFunctionUse("setactive", "Specify the buffer you want to activate"));
            }

            if (_currentArgStack.Peek() is decimal)
            {
                byte channel = (byte)Convert.ChangeType(_currentArgStack.Pop(), typeof(byte));
                temp[channel] = 1;
            }
            else
            {
                _currentBuffer = (MemoryBuffer)_currentArgStack.Pop();
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
                this.Log("Updating Channel Buffer", DebugChannel.Log);
                _activeChannels = temp;
                CL.WriteToBuffer(_activeChannelBuffer, _activeChannels);
            }

        }

        private Random rnd = new Random();
        private byte randombytesource()
        {
            return (byte)rnd.Next();
        }

        private void cmd_writerandom()
        {
            if (_currentArgStack.Count == 0)
            {
                CL.WriteRandom(_currentBuffer, randombytesource, _activeChannels);
            }
            while (_currentArgStack.Count != 0)
            {
                object obj = _currentArgStack.Pop();
                if (!(obj is MemoryBuffer))
                {
                    throw new InvalidOperationException("Argument has the wrong type: " + obj);
                }
                CL.WriteRandom(obj as MemoryBuffer, randombytesource, _activeChannels);
            }
        }

        private void cmd_jump() //Dummy function. Implementation in Analyze(code) function(look for isDirectExecute)
        {

        }

        private void cmd_break()
        {
            if (_ignoreDebug)
            {
                return;
            }
            stepResult.TriggeredDebug = true;
            if (_currentArgStack.Count == 0)
            {
                stepResult.DebugBuffer = _currentBuffer;
            }
            else if (_currentArgStack.Count == 1)
            {
                object obj = _currentArgStack.Pop();
                if (!(obj is MemoryBuffer))
                {
                    throw new InvalidOperationException("Argument has the wrong type or is null");
                }
                stepResult.DebugBuffer = obj as MemoryBuffer;

            }
            else
            {
                throw new InvalidOperationException("Only One or Zero argument");
            }
        }

        #endregion



        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount, KernelDatabase kernelDB,
            bool ignoreDebug)
        {
            _flFunctions = new Dictionary<string, FlFunction>()
            {
                {"setactive", cmd_setactive },
                {"random", cmd_writerandom },
                {"jmp", cmd_setactive },
                {"brk", cmd_break }
            };


            NumberParsingHelper.NumberFormat.NumberDecimalSeparator = ",";
            NumberParsingHelper.NumberFormat.NumberGroupSeparator = ".";

            Reset(file, input, width, height, depth, channelCount, kernelDB, ignoreDebug);
        }
        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount, string kernelDBFolder,
            bool ignoreDebug) : this(file, input, width, height, depth, channelCount, new KernelDatabase(kernelDBFolder), ignoreDebug) { }
        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount, string kernelDBFolder) : this(file, input, width, height, depth, channelCount, new KernelDatabase(kernelDBFolder), false) { }
        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount, KernelDatabase kernelDB) : this(file, input, width, height, depth, channelCount, kernelDB, false) { }

        public void Reset(string file, MemoryBuffer input, int width, int height, int depth, int chanelCount,
            KernelDatabase kernelDB)
        {
            Reset(file, input, width, height, depth, chanelCount, kernelDB, false);
        }

        public void Reset(string file, MemoryBuffer input, int width, int height, int depth, int chanelCount, KernelDatabase kernelDB, bool ignoreDebug)
        {
            _currentBuffer = input;
            this._ignoreDebug = ignoreDebug;
            this._width = width;
            this._height = height;
            this._depth = depth;
            this._channelCount = chanelCount;
            this._kernelDb = kernelDB;
            _activeChannels = new byte[_channelCount];
            for (int i = 0; i < _channelCount; i++)
            {
                _activeChannels[i] = 1;
            }

            _activeChannelBuffer = CL.CreateBuffer(_activeChannels, MemoryFlag.ReadOnly | MemoryFlag.CopyHostPointer);

            _currentArgStack = new Stack<object>();
            foreach (KeyValuePair<string, MemoryBuffer> memoryBuffer in _definedBuffers)
            {
                memoryBuffer.Value.Dispose();
            }

            _jumpStack.Clear();
            _definedBuffers.Clear();
            _definedBuffers.Add("in", input);
            _jumpLocations.Clear();


            LoadSource(file);

            ParseDefines();
            ParseJumpLocations();

            Reset();

        }



        public MemoryBuffer GetResultBuffer()
        {
            return _currentBuffer;
        }

        public T[] GetResult<T>() where T : struct
        {
            return CL.ReadBuffer<T>(_currentBuffer, (int)_currentBuffer.Size);
        }

        public InterpreterStepResult Step()
        {

            stepResult = new InterpreterStepResult();
            if (Terminated)
            {
                stepResult.Terminated = true;

            }
            else
            {
                string code = _source[_currentIndex].Split("//")[0];
                if (code == string.Empty)
                {
                    _currentIndex++; //Next Line since this one is emtpy

                }
                else
                {
                    if (Analyze(code))
                    {
                        _currentIndex++;
                        _currentWord = 1;
                    }
                }


                DetectEnd();
            }


            return stepResult;
        }


        private void ParseJumpLocations()
        {
            for (int i = _source.Count - 1; i >= 0; i--)
            {
                if (_source[i].EndsWith(":") && _source.Count - 1 != i)
                {
                    _jumpLocations.Add(_source[i].Remove(_source[i].Length - 1, 1), i + 1);

                }
            }
        }


        bool Analyze(string code)
        {

            string[] words = code.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            string function = words.Length == 0 ? "" : words[0];
            CLKernel kernel = null;
            if (function == "")
            {
                return false;
            }

            bool isBakedFunction = _flFunctions.ContainsKey(function);
            bool isDirectExecute = function == "jmp";



            if (!isBakedFunction && !_kernelDb.TryGetCLKernel(function, out kernel))
            {
                throw new NotImplementedException("Argument Not found in line " + code + ".");
            }

            if (_leaveStack) //This keeps the stack when returning from a "function"
            {
                _leaveStack = false;
            }
            else
            {
                _currentArgStack = new Stack<object>();
            }

            bool ret = true;
            for (; _currentWord < words.Length; _currentWord++) //loop through the words. start value can be != 0 when returning from a function specified as an argument to a kernel
            {
                if (AnalyzeWord(words[_currentWord], out object val))
                {
                    JumpTo(_jumpLocations[words[_currentWord]], isDirectExecute);
                    ret = false; //We Jumped to another point in the code.
                    _currentArgStack.Push(null); //Push null to signal the interpreter that he returned before assigning the right value.
                    break;
                }
                else
                {
                    _currentArgStack.Push(val); //push the value to the stack
                }
            }

            if (_currentWord == words.Length && ret) //We finished parsing the line and we didnt jump.
            {
                if (isBakedFunction)
                {
                    _flFunctions[function](); //Execute baked function
                }
                else if (kernel == null || words.Length - 1 != kernel.parameter.Count - 4)
                {
                    throw new Exception("Not the right amount of arguments.");
                }
                else
                {
                    //Execute filter
                    for (int i = kernel.parameter.Count - 1; i >= 4; i--)
                    {
                        object obj = _currentArgStack.Pop(); //Get the arguments and set them to the kernel
                        kernel.SetArg(i, obj);
                    }

                    this.Log("Running kernel: " + function, DebugChannel.Log);
                    CL.Run(kernel, _currentBuffer, new int3(_width, _height, _depth), _activeChannelBuffer, _channelCount); //Running the kernel
                    
                }

            }
            return ret;
        }

        bool AnalyzeWord(string word, out object val)
        {
            if (_jumpLocations.ContainsKey(word))
            {
                this.Log("Jumping to location: " + word, DebugChannel.Log);

                val = null;
                return true;
            }
            else
            {
                val = null;
                if (_definedBuffers.ContainsKey(word))
                {
                    val = _definedBuffers[word];
                }
                else if (decimal.TryParse(word, NumberStyles.Any, NumberParsingHelper, out decimal numberDecimal))
                {
                    val = numberDecimal;
                }

                if (val == null)
                {
                    throw new NotImplementedException("No Baked in functions in arguments");
                }
                val = val ?? "PLACEHOLDER";
                return false;
            }
        }

        private void Reset()
        {
            _currentIndex = EntryIndex;
            _currentWord = 1;
        }
        private bool IsSurroundedBy(string text, string surrStr)
        {
            return text.StartsWith(surrStr) && text.EndsWith(surrStr);
        }

        private void ParseDefines()
        {
            for (int i = _source.Count - 1; i >= 0; i--)
            {
                if (_source[i].StartsWith(DefineKey))
                {
                    string[] kvp = _source[i].Remove(0, DefineKey.Length).Split(':');
                    if (kvp.Length < 2)
                    {
                        this.Crash(new FL_InvalidFunctionUse(DefineKey, "Invalid Define statement at line " + i));
                    }
                    string varname = kvp[0].Trim();


                    if (_definedBuffers.ContainsKey(varname))
                    {
                        this.Log("Overwriting " + varname, DebugChannel.Warning);
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

                    string[] args = kvp[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);


                    string filename = args[0].Trim();



                    if (IsSurroundedBy(filename, "\""))
                    {
                        _definedBuffers.Add(varname,
                            CL.CreateFromImage((Bitmap)Image.FromFile(filename.Replace("\"", "")),
                                MemoryFlag.CopyHostPointer | flags));
                    }
                    else if (filename == "random")
                    {
                        MemoryBuffer buf = CL.CreateEmpty<byte>(InputBufferSize, flags | MemoryFlag.CopyHostPointer);
                        CL.WriteRandom(buf, randombytesource, _activeChannels);
                        _definedBuffers.Add(varname, buf);
                    }
                    else if (filename == "empty")
                    {
                        _definedBuffers.Add(varname, CL.CreateEmpty<byte>(InputBufferSize, MemoryFlag.CopyHostPointer | flags));
                    }
                    else if (filename == "wfc")
                    {
                        if (args.Length < 10)
                        {
                            throw new Exception("Invalid Define statement at line " + i);
                        }
                        if (!int.TryParse(args[2], out int n))
                        {
                            throw new Exception("Invalid N argument");
                        }
                        if (!int.TryParse(args[3], out int width))
                        {
                            throw new Exception("Invalid width argument");
                        }
                        if (!int.TryParse(args[4], out int height))
                        {
                            throw new Exception("Invalid height argument");
                        }
                        if (!bool.TryParse(args[5], out bool periodicInput))
                        {
                            throw new Exception("Invalid periodicInput argument");
                        }
                        if (!bool.TryParse(args[6], out bool periodicOutput))
                        {
                            throw new Exception("Invalid periodicOutput argument");
                        }
                        if (!int.TryParse(args[7], out int symetry))
                        {
                            throw new Exception("Invalid symmetry argument");
                        }
                        if (!int.TryParse(args[8], out int ground))
                        {
                            throw new Exception("Invalid ground argument");
                        }
                        if (!int.TryParse(args[9], out int limit))
                        {
                            throw new Exception("Invalid limit argument");
                        }

                        WaveFunctionCollapse wfc = new WFCOverlayMode(args[1].Trim().Replace("\"", ""), n, width, height, periodicInput, periodicOutput, symetry, ground);

                        wfc.Run(limit);
                        Bitmap bmp = new Bitmap(wfc.Graphics(), new Size(this._width, this._height)); //Apply scaling
                        _definedBuffers.Add(varname,
                            CL.CreateFromImage(bmp,
                                MemoryFlag.CopyHostPointer | flags));
                    }
                    else
                    {
                        throw new InvalidOperationException("Can not resolve symbol: " + varname + " in line " + i);
                    }

                }
            }
        }
        bool DetectEnd()
        {
            if (_currentIndex == _source.Count || _source[_currentIndex].EndsWith(":"))
            {
                if (_jumpStack.Count == 0)
                {
                    this.Log("Reached End of Code", DebugChannel.Log);

                    Terminated = true;
                    return true;
                }
                else
                {

                    InterpreterState lastState = _jumpStack.Pop();

                    this.Log("Returning to location: " + _source[lastState.Line], DebugChannel.Log);
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
                    return true;
                }
            }

            return false;
        }

        void JumpTo(int index, bool keepBuffer = false)
        {
            //currentArgStack.Push(null);
            _jumpStack.Push(new InterpreterState(_currentIndex, _currentBuffer, _currentArgStack));
            stepResult.HasJumped = true;
            int size = (int)_currentBuffer.Size;
            if (!keepBuffer)
            {
                _currentBuffer = CL.CreateEmpty<byte>(size, MemoryFlag.ReadWrite);
            }
            _currentIndex = index;
            _currentWord = 1;
        }

        void LoadSource(string file)
        {

            this.Log($"Loading Source..", DebugChannel.Log);

            Dictionary<string, bool> defs = new Dictionary<string, bool>();

            for (int i = 0; i < _channelCount; i++)
            {
                defs.Add("channel" + i, true);
            }

            List<string> lines = TextProcessorAPI.PreprocessLines(file, defs).ToList();


            for (var i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("//"))
                {
                    lines.RemoveAt(i);//Remove otherwise emtpty lines after removing comments
                }
                else
                {
                    lines[i] = line.Split("//")[0];
                }
            }

            _source = lines;
        }

    }
}
