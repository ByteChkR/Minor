using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;
using Engine.WFC;

namespace Engine.OpenFL
{
    /// <summary>
    /// partial class that contains the logic how to Parse Defines that are referencing a texture or other keywords
    /// </summary>
    public partial class Interpreter
    {
        /// <summary>
        /// Define handler that loads defined textures
        /// </summary>
        /// <param name="arg">The Line of the definition</param>
        private static void DefineTexture(CLAPI instance, string[] arg, Dictionary<string, CLBufferInfo> defines,
            int width, int height,
            int depth, int channelCount, KernelDatabase kernelDb)
        {
            if (arg.Length < 2)
            {
                Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Invalid Define statement"), true);
                return;
            }

            string varname = arg[0].Trim();


            if (defines.ContainsKey(varname))
            {
                Logger.Log("Overwriting " + varname, DebugChannel.Warning | DebugChannel.OpenFL, 10);
                defines.Remove(varname);
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
            byte[] activeChannels = new byte[channelCount];
            for (int i = 0; i < activeChannels.Length; i++)
            {
                activeChannels[i] = 1;
            }

            int InputBufferSize = width * height * depth * channelCount;

            if (IsSurroundedBy(filename, FilepathIndicator))
            {
                string fn = filename.Replace(FilepathIndicator, "");
                if (File.Exists(fn))
                {
                    Bitmap bmp = new Bitmap((Bitmap) System.Drawing.Image.FromFile(fn), width, height);
                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateFromImage(instance, bmp,
                        MemoryFlag.CopyHostPointer | flags), true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else
                {
                    Logger.Crash(
                        new FLInvalidFunctionUseException(DefineKey, "Invalid Filepath",
                            new InvalidFilePathException(fn)), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
            }
            else if (filename == "rnd")
            {
                MemoryBuffer buf =
                    CLAPI.CreateEmpty<byte>(instance, InputBufferSize, flags | MemoryFlag.CopyHostPointer);
                CLAPI.WriteRandom(instance, buf, randombytesource, activeChannels, false);

                CLBufferInfo info = new CLBufferInfo(buf, true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
            else if (filename == "urnd")
            {
                MemoryBuffer buf =
                    CLAPI.CreateEmpty<byte>(instance, InputBufferSize, flags | MemoryFlag.CopyHostPointer);
                CLAPI.WriteRandom(instance, buf, randombytesource, activeChannels, true);

                CLBufferInfo info = new CLBufferInfo(buf, true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
            else if (filename == "empty")
            {
                CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(instance, InputBufferSize, flags), true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
            else if (filename == "wfc" || filename == "wfcf")
            {
                bool force = filename == "wfcf";
                if (args.Length < 10)
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[2], out int n))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[3], out int widh))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[4], out int heigt))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!bool.TryParse(args[5], out bool periodicInput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!bool.TryParse(args[6], out bool periodicOutput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[7], out int symmetry))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[8], out int ground))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[9], out int limit))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else
                {
                    string fn = args[1].Trim().Replace(FilepathIndicator, "");
                    if (IOManager.Exists(fn))
                    {
                        Bitmap bmp;
                        WFCOverlayMode wfc = new WFCOverlayMode(fn, n, widh,
                            heigt, periodicInput, periodicOutput, symmetry, ground);
                        if (force)
                        {
                            do
                            {
                                wfc.Run(limit);
                                bmp = new Bitmap(wfc.Graphics(), new Size(width, height)); //Apply scaling
                            } while (!wfc.Success);
                        }
                        else
                        {
                            wfc.Run(limit);
                            bmp = new Bitmap(wfc.Graphics(), new Size(width, height)); //Apply scaling
                        }

                        CLBufferInfo info = new CLBufferInfo(CLAPI.CreateFromImage(instance, bmp,
                            MemoryFlag.CopyHostPointer | flags), true);
                        info.SetKey(varname);
                        defines.Add(varname, info);
                    }
                    else
                    {
                        Logger.Crash(
                            new FLInvalidFunctionUseException("wfc", "Invalid WFC Image statement",
                                new InvalidFilePathException(fn)), true);
                        CLBufferInfo info =
                            new CLBufferInfo(CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                                true);
                        info.SetKey(varname);
                        defines.Add(varname, info);
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
                CLBufferInfo info =
                    new CLBufferInfo(CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite), true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
        }
    }
}