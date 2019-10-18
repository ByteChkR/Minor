using System;
using System.Drawing;
using System.IO;
using Engine.Debug;
using Engine.Exceptions;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.WFC;
using Image = Engine.OpenCL.DotNetCore.Memory.Image;

namespace Engine.OpenFL
{
    public partial class Interpreter
    {
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
                string fn = filename.Replace(FilepathIndicator, "");
                if (File.Exists(fn))
                {
                    Bitmap bmp = new Bitmap((Bitmap)System.Drawing.Image.FromFile(fn), _width, _height);
                    AddBufferToDefine(varname,
                        new CLBufferInfo(CLAPI.CreateFromImage(bmp,
                            MemoryFlag.CopyHostPointer | flags), true));
                }
                else
                {
                    Logger.Crash(new FLInvalidFunctionUseException(DefineKey, "Invalid Filepath", new InvalidFilePathException(fn)), true);
                    Logger.Log("Invalid Filepath. Using empty buffer. " + fn, DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
            }
            else if (filename == "rnd")
            {
                MemoryBuffer buf = CLAPI.CreateEmpty<byte>(InputBufferSize, flags | MemoryFlag.CopyHostPointer);
                CLAPI.WriteRandom(buf, randombytesource, _activeChannels, false);

                CLBufferInfo info = new CLBufferInfo(buf, true);
                AddBufferToDefine(varname, info);
            }
            else if (filename == "urnd")
            {
                MemoryBuffer buf = CLAPI.CreateEmpty<byte>(InputBufferSize, flags | MemoryFlag.CopyHostPointer);
                CLAPI.WriteRandom(buf, randombytesource, _activeChannels, true);

                CLBufferInfo info = new CLBufferInfo(buf, true);
                AddBufferToDefine(varname, info);
            }
            else if (filename == "empty")
            {
                CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, flags), true);
                info.SetKey(varname);
                AddBufferToDefine(varname, info);
            }
            else if (filename == "wfc" || filename == "wfcf")
            {
                bool force = filename == "wfcf";
                if (args.Length < 10)
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!int.TryParse(args[2], out int n))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!int.TryParse(args[3], out int width))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!int.TryParse(args[4], out int height))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!bool.TryParse(args[5], out bool periodicInput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!bool.TryParse(args[6], out bool periodicOutput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!int.TryParse(args[7], out int symmetry))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!int.TryParse(args[8], out int ground))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else if (!int.TryParse(args[9], out int limit))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    Logger.Log("Invalid WFC Define statement. Using empty buffer", DebugChannel.Error, 10);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
                else
                {
                    string fn = args[1].Trim().Replace(FilepathIndicator, "");
                    if (File.Exists(fn))
                    {
                        Bitmap bmp;
                        WFCOverlayMode wfc = new WFCOverlayMode(fn, n, width,
                            height, periodicInput, periodicOutput, symmetry, ground);
                        if (force)
                            do
                            {
                                wfc.Run(limit);
                                bmp = new Bitmap(wfc.Graphics(), new Size(_width, _height)); //Apply scaling
                            } while (!wfc.Success);
                        else
                        {
                            wfc.Run(limit);
                            bmp = new Bitmap(wfc.Graphics(), new Size(_width, _height)); //Apply scaling
                        }

                        CLBufferInfo info = new CLBufferInfo(CLAPI.CreateFromImage(bmp,
                            MemoryFlag.CopyHostPointer | flags), true);
                        info.SetKey(varname);
                        AddBufferToDefine(varname, info);
                    }
                    else
                    {
                        Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Image statement", new InvalidFilePathException(fn)), true);
                        Logger.Log("Invalid Image statement. Using empty buffer", DebugChannel.Error, 10);
                        CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                        info.SetKey(varname);
                        AddBufferToDefine(varname, info);
                    }
                }


            }

            else
            {
                string s = "";
                foreach (string s1 in args)
                {
                    s += s1 + " ";
                }
                Logger.Crash(new FLInvalidFunctionUseException(DefineKey, "Define statement wrong: " + s), true);
                Logger.Log("Invalid Define statement. Using empty buffer", DebugChannel.Error, 10);
                CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                info.SetKey(varname);
                AddBufferToDefine(varname, info);
            }
        }
    }
}