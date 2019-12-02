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
        /// <param name="arg">The Line of the definition</param>
        private static void DefineScript(CLAPI instance, string[] arg, Dictionary<string, CLBufferInfo> defines,
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

            string[] args = arg[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);


            string filename = args[0].Trim();

            int InputBufferSize = width * height * depth * channelCount;

            if (IsSurroundedBy(filename, FilepathIndicator))
            {
                Logger.Log("Loading SubScript...", DebugChannel.Log | DebugChannel.OpenFL, 10);

                MemoryBuffer buf =
                    CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite);


                string fn = filename.Replace(FilepathIndicator, "");


                if (IOManager.Exists(fn))
                {
                    Interpreter interpreter = new Interpreter(instance, fn, buf, width, height,
                        depth, channelCount, kernelDb, true);

                    do
                    {
                        interpreter.Step();
                    } while (!interpreter.Terminated);

                    CLBufferInfo info = interpreter.GetActiveBufferInternal();
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

                    CLBufferInfo info = new CLBufferInfo(
                        CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite),
                        true);
                    info.SetKey(varname);
                    defines.Add(varname, info);
                }
            }
            else
            {
                Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Not a valid filepath as argument."),
                    true);

                CLBufferInfo info =
                    new CLBufferInfo(CLAPI.CreateEmpty<byte>(instance, InputBufferSize, MemoryFlag.ReadWrite), true);
                info.SetKey(varname);
                defines.Add(varname, info);
            }
        }
    }
}