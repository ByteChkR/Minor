using System;
using System.Collections.Generic;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;

namespace Engine.OpenFL
{
    /// <summary>
    /// partial class that contains the logic how to Parse Defines that are referencing another fl script
    /// </summary>
    public partial class Interpreter
    {
        /// <summary>
        /// Define handler that loads defined scripts
        /// </summary>
        /// <param name="instance">Clapi Instance of the Current Thread</param>
        /// <param name="arg">Args from the FL Script</param>
        /// <param name="defines">Defines</param>
        /// <param name="width">width of the input buffer</param>
        /// <param name="height">height of the input buffer</param>
        /// <param name="depth">depth of the input buffer</param>
        /// <param name="channelCount">channel count of the input buffer</param>
        /// <param name="kernelDb">the kernel database to use</param>
        private static void DefineScript(Clapi instance, string[] arg, Dictionary<string, ClBufferInfo> defines,
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
                Logger.Log("Overwriting " + varname, DebugChannel.Warning | DebugChannel.EngineOpenFL, 10);
                defines.Remove(varname);
            }

            string[] args = arg[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);


            string filename = args[0].Trim();

            int inputBufferSize = width * height * depth * channelCount;

            if (IsSurroundedBy(filename, FilepathIndicator))
            {
                Logger.Log("Loading SubScript...", DebugChannel.Log | DebugChannel.EngineOpenFL, 10);

                MemoryBuffer buf =
                    Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite);


                string fn = filename.Replace(FilepathIndicator, "");


                if (IOManager.Exists(fn))
                {
                    Interpreter interpreter = new Interpreter(instance, fn, buf, width, height,
                        depth, channelCount, kernelDb, true);

                    do
                    {
                        interpreter.Step();
                    } while (!interpreter.Terminated);

                    ClBufferInfo info = interpreter.GetActiveBufferInternal();
                    info.SetKey(varname);
                    defines.Add(varname, info);
                    interpreter.ReleaseResources();
                }
                else
                {
                    Logger.Crash(
                        new FLInvalidFunctionUseException(ScriptDefineKey, "Not a valid filepath as argument.",
                            new InvalidFilePathException(fn)),
                        true);

                    ClBufferInfo info = new ClBufferInfo(
                        Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
            }
            else
            {
                Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Not a valid filepath as argument."),
                    true);

                ClBufferInfo info =
                    new ClBufferInfo(Clapi.CreateEmpty<byte>(instance, inputBufferSize, MemoryFlag.ReadWrite), true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
        }
    }
}