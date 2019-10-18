using System;
using System.IO;
using Engine.Debug;
using Engine.Exceptions;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.Memory;

namespace Engine.OpenFL
{

    public partial class Interpreter
    {
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


                string fn = filename.Replace(FilepathIndicator, "");


                if (File.Exists(fn))
                {
                    Interpreter interpreter = new Interpreter(fn, buf, _width, _height,
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
                    Logger.Crash(new FLInvalidFunctionUseException(ScriptDefineKey, "Not a valid filepath as argument.", new InvalidFilePathException(fn)),
                        true);
                    Logger.Log("Invalid Define statement. Using empty buffer", DebugChannel.Error, 10);

                    CLBufferInfo info = new CLBufferInfo(CLAPI.CreateEmpty<byte>(InputBufferSize, MemoryFlag.ReadWrite), true);
                    info.SetKey(varname);
                    AddBufferToDefine(varname, info);
                }
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

    }
}