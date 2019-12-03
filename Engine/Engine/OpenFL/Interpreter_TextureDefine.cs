using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
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
        /// <param name="instance"></param>
        /// <param name="arg"></param>
        /// <param name="defines"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="channelCount"></param>
        /// <param name="kernelDb"></param>
        private static void DefineTexture(Clapi instance, string[] arg, Dictionary<string, ClBufferInfo> defines,
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
                Logger.Log("Overwriting " + varname, DebugChannel.Warning | DebugChannel.OpenFl, 10);
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

            int inputBufferSize = width * height * depth * channelCount;

            if (IsSurroundedBy(filename, FilepathIndicator))
            {
                string fn = filename.Replace(FilepathIndicator, "");
                if (File.Exists(fn))
                {
                    Bitmap bmp = new Bitmap((Bitmap)System.Drawing.Image.FromFile(fn), width, height);
                    ClBufferInfo info = new ClBufferInfo(Clapi.CreateFromImage(instance, bmp,
                        MemoryFlag.CopyHostPointer | flags), true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else
                {
                    Logger.Crash(
                        new FLInvalidFunctionUseException(DefineKey, "Invalid Filepath",
                            new InvalidFilePathException(fn)), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
            }
            else if (filename == "rnd")
            {
                MemoryBuffer buf =
                    Clapi.CreateEmpty<byte>(instance, inputBufferSize, flags | MemoryFlag.CopyHostPointer);
                Clapi.WriteRandom(instance, buf, Randombytesource, activeChannels, false);

                ClBufferInfo info = new ClBufferInfo(buf, true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
            else if (filename == "urnd")
            {
                MemoryBuffer buf =
                    Clapi.CreateEmpty<byte>(instance, inputBufferSize, flags | MemoryFlag.CopyHostPointer);
                Clapi.WriteRandom(instance, buf, Randombytesource, activeChannels, true);

                ClBufferInfo info = new ClBufferInfo(buf, true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
            else if (filename == "empty")
            {
                ClBufferInfo info = new ClBufferInfo(Clapi.CreateEmpty<byte>(instance, inputBufferSize, flags), true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
            else if (filename == "wfc" || filename == "wfcf")
            {
                bool force = filename == "wfcf";
                if (args.Length < 10)
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[2], out int n))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[3], out int widh))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[4], out int heigt))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!bool.TryParse(args[5], out bool periodicInput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!bool.TryParse(args[6], out bool periodicOutput))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[7], out int symmetry))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[8], out int ground))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
                else if (!int.TryParse(args[9], out int limit))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("wfc", "Invalid WFC Define statement"), true);
                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
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
                        WfcOverlayMode wfc = new WfcOverlayMode(fn, n, widh,
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

                        ClBufferInfo info = new ClBufferInfo(Clapi.CreateFromImage(instance, bmp,
                            MemoryFlag.CopyHostPointer | flags), true);
                        info.SetKey(varname);
                        defines.Add(varname, info);
                    }
                    else
                    {
                        Logger.Crash(
                            new FLInvalidFunctionUseException("wfc", "Invalid WFC Image statement",
                                new InvalidFilePathException(fn)), true);
                        ClBufferInfo info =
                            new ClBufferInfo(Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                                true);
                        info.SetKey(varname);
                        defines.Add(varname, info);
                    }
                }
            }

            else
            {
                StringBuilder s = new StringBuilder();
                foreach (string s1 in args)
                {
                    s.Append(s1 + " ");
                }

                Logger.Crash(new FLInvalidFunctionUseException(DefineKey, "Define statement wrong: " + s), true);
                ClBufferInfo info =
                    new ClBufferInfo(Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite), true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
        }
    }
}