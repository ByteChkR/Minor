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
        private const string DEFINE_KEY = "--define texture ";
        private static CultureInfo numberParsingHelper = new CultureInfo(CultureInfo.InvariantCulture.LCID);
        private delegate void FL_Function();
        public struct InterpreterStepResult
        {
            public bool hasJumped;
            public bool terminated;
            public bool triggeredDebug;
            public MemoryBuffer debugBuffer;
        }


        private static Dictionary<string, FL_Function> flFunctions;

        private KernelDatabase kernelDB = null;

        private List<string> source;
        private int currentIndex;
        private int currentWord;
        private Stack<object> currentArgStack;
        private MemoryBuffer currentBuffer;
        private Stack<InterpreterState> jumpStack = new Stack<InterpreterState>();
        private int width, height, depth, channelCount;
        private int size => width * height * depth * channelCount;
        private byte[] activeChannels;
        private MemoryBuffer activeChannelBuffer;


        private Dictionary<string, MemoryBuffer> definedBuffers = new Dictionary<string, MemoryBuffer>();
        private Dictionary<string, int> jumpLocations = new Dictionary<string, int>();
        private bool leaveStack = false;
        private bool ignoreDebug = false;
        private InterpreterStepResult stepResult;
        private int EntryIndex
        {
            get
            {
                int idx = source.IndexOf("Main:");
                if (idx == -1 || source.Count - 1 == idx)
                    throw new Exception("There needs to be a main function.");
                return idx + 1;
            }
        }


        public bool Terminated { get; private set; }


        #region FL_Functions

        private void cmd_setactive()
        {
            if (currentArgStack.Count < 1)
            {

                this.Crash(new FL_InvalidFunctionUse("setactive", "Specify the buffer you want to activate"));

            }

            byte[] temp = new byte[channelCount];
            while (currentArgStack.Count != 1)
            {
                object val = currentArgStack.Pop();
                if (!(val is decimal))
                {
                    this.Crash(new FL_InvalidFunctionUse("setactive", "Invalid channel Arguments"));
                }

                byte channel = (byte)Convert.ChangeType(val, typeof(byte));
                if (channel >= channelCount)
                {
                    this.Log("Script is enabling channels beyond channel count. Ignoring...", DebugChannel.Warning);
                }
                else
                {
                    temp[channel] = 1;
                }
            }

            if (currentArgStack.Peek() == null || (!(currentArgStack.Peek() is MemoryBuffer) && !(currentArgStack.Peek() is decimal)))
            {
                this.Crash(new FL_InvalidFunctionUse("setactive", "Specify the buffer you want to activate"));
            }

            if (currentArgStack.Peek() is decimal)
            {
                byte channel = (byte)Convert.ChangeType(currentArgStack.Pop(), typeof(byte));
                temp[channel] = 1;
            }
            else
            {
                currentBuffer = (MemoryBuffer)currentArgStack.Pop();
            }

            bool needCopy = false;
            for (int i = 0; i < channelCount; i++)
            {
                if (activeChannels[i] != temp[i])
                {
                    needCopy = true;
                    break;
                }
            }

            if (needCopy)
            {
                this.Log("Updating Channel Buffer", DebugChannel.Log);
                activeChannels = temp;
                CL.WriteToBuffer(activeChannelBuffer, activeChannels);
            }

        }

        private Random rnd = new Random();
        private byte randombytesource()
        {
            return (byte)rnd.Next();
        }

        private void cmd_writerandom()
        {
            if (currentArgStack.Count == 0)
            {
                CL.WriteRandom(currentBuffer, randombytesource, activeChannels);
            }
            while (currentArgStack.Count != 0)
            {
                object obj = currentArgStack.Pop();
                if (!(obj is MemoryBuffer))
                {
                    throw new InvalidOperationException("Argument has the wrong type: " + obj.ToString());
                }
                CL.WriteRandom(obj as MemoryBuffer, randombytesource, activeChannels);
            }
        }

        private void cmd_jump() //Dummy function. Implementation in Analyze(code) function(look for isDirectExecute)
        {

        }

        private void cmd_break()
        {
            stepResult.triggeredDebug = true;
            if (currentArgStack.Count == 0)
            {
                stepResult.debugBuffer = currentBuffer;
            }
            else if (currentArgStack.Count == 1)
            {
                object obj = currentArgStack.Pop();
                if (!(obj is MemoryBuffer))
                {
                    throw new InvalidOperationException("Argument has the wrong type or is null");
                }
                stepResult.debugBuffer = obj as MemoryBuffer;

            }
            else
            {
                throw new InvalidOperationException("Only One or Zero argument");
            }
        }

        #endregion



        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount, KernelDatabase kernelDB,
            bool ignoreDebug = false)
        {
            flFunctions = new Dictionary<string, FL_Function>()
            {
                {"setactive", cmd_setactive },
                {"random", cmd_writerandom },
                {"jmp", cmd_setactive },
                {"brk", cmd_writerandom }
            };


            numberParsingHelper.NumberFormat.NumberDecimalSeparator = ",";
            numberParsingHelper.NumberFormat.NumberGroupSeparator = ".";

            Reset(file, input, width, height, depth, channelCount, kernelDB, ignoreDebug);
        }
        public Interpreter(string file, MemoryBuffer input, int width, int height, int depth, int channelCount, string kernelDBFolder,
            bool ignoreDebug = false):this(file, input, width, height, depth, channelCount, new KernelDatabase(kernelDBFolder), ignoreDebug) { }

        public void Reset(string file, MemoryBuffer input, int width, int height, int depth, int chanelCount, KernelDatabase kernelDB, bool ignoreDebug=false)
        {
            currentBuffer = input;
            this.ignoreDebug = ignoreDebug;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.channelCount = chanelCount;
            this.kernelDB = kernelDB;
            activeChannels = new byte[channelCount];
            for (int i = 0; i < channelCount; i++)
            {
                activeChannels[i] = 1;
            }

            activeChannelBuffer = CL.CreateBuffer(activeChannels, MemoryFlag.ReadOnly | MemoryFlag.CopyHostPointer);

            currentArgStack = new Stack<object>();
            foreach (KeyValuePair<string, MemoryBuffer> memoryBuffer in definedBuffers)
            {
                memoryBuffer.Value.Dispose();
            }

            jumpStack.Clear();
            definedBuffers.Clear();
            definedBuffers.Add("in", input);
            jumpLocations.Clear();


            LoadSource(file);

            ParseDefines();
            ParseJumpLocations();

            Reset();

        }



        public MemoryBuffer GetResultBuffer()
        {
            return currentBuffer;
        }

        public T[] GetResult<T>() where T : struct
        {
            return CL.ReadBuffer<T>(currentBuffer, (int)currentBuffer.Size);
        }

        public InterpreterStepResult Step()
        {

            stepResult = new InterpreterStepResult();
            if (Terminated)
            {
                stepResult.terminated = true;

            }
            else
            {
                string code = source[currentIndex].Split("//")[0];
                if (code == string.Empty)
                {
                    currentIndex++; //Next Line since this one is emtpy

                }
                else
                {
                    if (Analyze(code))
                    {
                        currentIndex++;
                        currentWord = 1;
                    }
                }


                DetectEnd();
            }


            return stepResult;
        }


        private void ParseJumpLocations()
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i].EndsWith(":") && source.Count - 1 != i)
                {
                    jumpLocations.Add(source[i].Remove(source[i].Length - 1, 1), i + 1);

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

            bool isBakedFunction = flFunctions.ContainsKey(function);
            bool isDirectExecute = function == "jmp";



            if (!isBakedFunction && !kernelDB.TryGetCLKernel(function, out kernel))
            {
                throw new NotImplementedException("Argument Not found in line " + code + ".");
            }

            if (leaveStack) //This keeps the stack when returning from a "function"
                leaveStack = false;
            else
                currentArgStack = new Stack<object>();

            bool ret = true;
            for (; currentWord < words.Length; currentWord++) //loop through the words. start value can be != 0 when returning from a function specified as an argument to a kernel
            {
                if (AnalyzeWord(words[currentWord], out object val))
                {
                    JumpTo(jumpLocations[words[currentWord]], isDirectExecute);
                    ret = false; //We Jumped to another point in the code.
                    currentArgStack.Push(null); //Push null to signal the interpreter that he returned before assigning the right value.
                    break;
                }
                else
                    currentArgStack.Push(val); //push the value to the stack
            }

            if (currentWord == words.Length && ret) //We finished parsing the line and we didnt jump.
            {
                if (isBakedFunction)
                {
                    flFunctions[function](); //Execute baked function
                }
                else if (kernel == null || words.Length - 1 != kernel.parameter.Count - 4) throw new Exception("Not the right amount of arguments.");
                else
                {
                    //Execute filter
                    for (int i = kernel.parameter.Count - 1; i >= 4; i--)
                    {
                        object obj = currentArgStack.Pop(); //Get the arguments and set them to the kernel
                        kernel.SetArg(i, obj);
                    }

                    this.Log("Running kernel: " + kernel.name, DebugChannel.Log);
                    CL.Run(kernel, currentBuffer, new int3(width, height, depth), activeChannelBuffer, channelCount); //Running the kernel
                    //byte[] buf = CL.ReadBuffer<byte>(currentBuffer, (int)currentBuffer.Size);
                }

            }
            return ret;
        }

        bool AnalyzeWord(string word, out object val)
        {
            if (jumpLocations.ContainsKey(word))
            {
                this.Log("Jumping to location: " + word, DebugChannel.Log);

                val = null;
                return true;
            }
            else
            {
                val = null;
                if (definedBuffers.ContainsKey(word))
                {
                    //Console.WriteLine("Found define key with entry: " + defines[word]);
                    val = definedBuffers[word];
                }
                else if (decimal.TryParse(word, NumberStyles.Any, numberParsingHelper, out decimal numberDecimal))
                {
                    //Console.WriteLine("Found decimal number: " + numberDecimal);
                    val = numberDecimal;
                }

                if (val == null) throw new NotImplementedException("No Baked in functions in arguments");
                val = val ?? "PLACEHOLDER";
                return false;
            }
        }

        private void Reset()
        {
            currentIndex = EntryIndex;
            currentWord = 1;
        }
        private bool IsSurroundedBy(string text, string surrStr)
        {
            return text.StartsWith(surrStr) && text.EndsWith(surrStr);
        }

        private void ParseDefines()
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i].StartsWith(DEFINE_KEY))
                {
                    string[] kvp = source[i].Remove(0, DEFINE_KEY.Length).Split(':');
                    if (kvp.Length < 2) this.Crash(new FL_InvalidFunctionUse(DEFINE_KEY, "Invalid Define statement at line " + i));
                    string varname = kvp[0].Trim();


                    if (definedBuffers.ContainsKey(varname))
                    {
                        this.Log("Overwriting " + varname, DebugChannel.Warning);
                        definedBuffers.Remove(varname);
                    }

                    MemoryFlag flags = MemoryFlag.ReadWrite;
                    string[] flagTest = varname.Split(' ');
                    if (flagTest.Length > 1)
                    {
                        varname = flagTest[1];
                        if (flagTest[0] == "r") flags = MemoryFlag.ReadOnly;

                        else if (flagTest[0] == "w") flags = MemoryFlag.WriteOnly;
                    }

                    string[] args = kvp[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);


                    string filename = args[0].Trim();



                    if (IsSurroundedBy(filename, "\""))
                    {
                        definedBuffers.Add(varname,
                            CL.CreateFromImage((Bitmap)Image.FromFile(filename.Replace("\"", "")),
                                MemoryFlag.CopyHostPointer | flags));
                    }
                    else if (filename == "random")
                    {
                        MemoryBuffer buf = CL.CreateEmpty<byte>(size, flags | MemoryFlag.CopyHostPointer);
                        CL.WriteRandom(buf, randombytesource, activeChannels);
                        definedBuffers.Add(varname, buf);
                    }
                    else if (filename == "empty")
                    {
                        definedBuffers.Add(varname, CL.CreateEmpty<byte>(size, MemoryFlag.CopyHostPointer | flags));
                    }
                    else if (filename == "wfc")
                    {
                        if (args.Length < 10) throw new Exception("Invalid Define statement at line " + i);
                        if (!int.TryParse(args[2], out int n)) throw new Exception("Invalid N argument");
                        if (!int.TryParse(args[3], out int width)) throw new Exception("Invalid width argument");
                        if (!int.TryParse(args[4], out int height)) throw new Exception("Invalid height argument");
                        if (!bool.TryParse(args[5], out bool periodicInput)) throw new Exception("Invalid periodicInput argument");
                        if (!bool.TryParse(args[6], out bool periodicOutput)) throw new Exception("Invalid periodicOutput argument");
                        if (!int.TryParse(args[7], out int symetry)) throw new Exception("Invalid symetry argument");
                        if (!int.TryParse(args[8], out int ground)) throw new Exception("Invalid ground argument");
                        if (!int.TryParse(args[9], out int limit)) throw new Exception("Invalid limit argument");

                        WaveFunctionCollapse wfc = new WFCOverlayMode(args[1].Trim().Replace("\"", ""), n, width, height, periodicInput, periodicOutput, symetry, ground);

                        wfc.Run(limit);
                        Bitmap bmp = new Bitmap(wfc.Graphics(), new Size(this.width, this.height)); //Apply scaling
                        definedBuffers.Add(varname,
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
            if (currentIndex == source.Count || source[currentIndex].EndsWith(":"))
            {
                if (jumpStack.Count == 0)
                {
                    this.Log("Reached End of Code", DebugChannel.Log);

                    Terminated = true;
                    return true;
                }
                else
                {

                    InterpreterState lastState = jumpStack.Pop();

                    this.Log("Returning to location: " + source[lastState.line], DebugChannel.Log);
                    currentIndex = lastState.line;


                    if (lastState.argumentStack.Count != 0 && lastState.argumentStack.Peek() == null)
                    {
                        leaveStack = true;
                        lastState.argumentStack.Pop();
                        lastState.argumentStack.Push(currentBuffer);
                    }

                    currentArgStack = lastState.argumentStack;
                    currentBuffer = lastState.activeBuffer;

                    currentWord = lastState.argumentStack.Count + 1;
                    return true;
                }
            }

            return false;
        }

        void JumpTo(int index, bool keepBuffer = false)
        {
            //currentArgStack.Push(null);
            jumpStack.Push(new InterpreterState(currentIndex, currentBuffer, currentArgStack));
            stepResult.hasJumped = true;
            int size = (int)currentBuffer.Size;
            if (!keepBuffer) currentBuffer = CL.CreateEmpty<byte>(size, MemoryFlag.ReadWrite);
            currentIndex = index;
            currentWord = 1;
        }

        void LoadSource(string file)
        {

            this.Log($"Loading Source..", DebugChannel.Log);

            Dictionary<string, bool> defs = new Dictionary<string, bool>();

            for (int i = 0; i < channelCount; i++)
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

            source = lines;
        }

    }
}
