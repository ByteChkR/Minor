using System;
using Engine.Debug;
using Engine.Exceptions;
using Engine.OpenCL;

namespace Engine.OpenFL
{
    /// <summary>
    /// Partial Class that Contains the Baked FL Functions
    /// </summary>
    public partial class Interpreter
    {
        /// <summary>
        /// The implementation of the command setactive
        /// </summary>
        private void cmd_setactive()
        {
            if (currentArgStack.Count < 1)
            {
                Logger.Crash(new FLInvalidFunctionUseException("setactive", "Specify the buffer you want to activate"),
                    true);
                return;
            }

            byte[] temp = new byte[channelCount];
            while (currentArgStack.Count != 1)
            {
                object val = currentArgStack.Pop();
                if (!(val is decimal))
                {
                    Logger.Crash(new FLInvalidFunctionUseException("setactive", "Invalid channel Arguments"), true);
                    val = 0;
                }

                byte channel = (byte) Convert.ChangeType(val, typeof(byte));
                if (channel >= channelCount)
                {
                    Logger.Log("Script is enabling channels beyond channel count. Ignoring...",
                        DebugChannel.Warning | DebugChannel.OpenFl, 10);
                }
                else
                {
                    temp[channel] = 1;
                }
            }

            if (currentArgStack.Peek() == null ||
                !(currentArgStack.Peek() is ClBufferInfo) && !(currentArgStack.Peek() is decimal))
            {
                Logger.Crash(new FLInvalidFunctionUseException("setactive", "Specify the buffer you want to activate"),
                    true);
                return;
            }

            if (currentArgStack.Peek() is decimal)
            {
                byte channel = (byte) Convert.ChangeType(currentArgStack.Pop(), typeof(byte));
                temp[channel] = 1;
            }
            else
            {
                currentBuffer = (ClBufferInfo) currentArgStack.Pop();
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
                Logger.Log("Updating Channel Buffer", DebugChannel.Log | DebugChannel.OpenFl, 6);
                activeChannels = temp;
                Clapi.WriteToBuffer(instance, activeChannelBuffer, activeChannels);
            }
            else
            {
                Logger.Log("Skipping Updating Channel Buffer", DebugChannel.Log | DebugChannel.OpenFl, 6);
            }
        }

        /// <summary>
        /// A function used as RandomFunc of type byte>
        /// </summary>
        /// <returns>a random byte</returns>
        private static byte Randombytesource()
        {
            return (byte) Rnd.Next();
        }

        /// <summary>
        /// The implementation of the command random
        /// </summary>
        private void cmd_writerandom()
        {
            if (currentArgStack.Count == 0)
            {
                Clapi.WriteRandom(instance, currentBuffer.Buffer, Randombytesource, activeChannels, false);
            }

            while (currentArgStack.Count != 0)
            {
                object obj = currentArgStack.Pop();
                if (!(obj is ClBufferInfo))
                {
                    Logger.Crash(
                        new FLInvalidArgumentType("Argument: " + currentArgStack.Count + 1, "MemoyBuffer/Image"),
                        true);
                    continue;
                }

                Clapi.WriteRandom(instance, (obj as ClBufferInfo).Buffer, Randombytesource, activeChannels, false);
            }
        }

        /// <summary>
        /// The implementation of the command random
        /// </summary>
        private void cmd_writerandomu()
        {
            if (currentArgStack.Count == 0)
            {
                Clapi.WriteRandom(instance, currentBuffer.Buffer, Randombytesource, activeChannels, true);
            }

            while (currentArgStack.Count != 0)
            {
                object obj = currentArgStack.Pop();
                if (!(obj is ClBufferInfo))
                {
                    Logger.Crash(
                        new FLInvalidArgumentType("Argument: " + currentArgStack.Count + 1, "MemoyBuffer/Image"),
                        true);
                    continue;
                }

                Clapi.WriteRandom(instance, (obj as ClBufferInfo).Buffer, Randombytesource, activeChannels, true);
            }
        }

        /// <summary>
        /// The implementation of the command jmp
        /// </summary>
        private void cmd_jump() //Dummy function. Implementation in AnalyzeLine(code) function(look for isDirectExecute)
        {
            Logger.Log("Jumping.", DebugChannel.Log | DebugChannel.OpenFl, 6);
        }

        /// <summary>
        /// The implementation of the command brk
        /// </summary>
        private void cmd_break()
        {
            if (ignoreDebug)
            {
                return;
            }

            stepResult.TriggeredDebug = true;
            if (currentArgStack.Count == 0)
            {
                stepResult.DebugBufferName = currentBuffer.ToString();
            }
            else if (currentArgStack.Count == 1)
            {
                object obj = currentArgStack.Pop();
                if (!(obj is ClBufferInfo))
                {
                    Logger.Crash(
                        new FLInvalidArgumentType("Argument: " + currentArgStack.Count + 1, "MemoyBuffer/Image"),
                        true);
                    return;
                }

                stepResult.DebugBufferName = (obj as ClBufferInfo).ToString();
            }
            else
            {
                Logger.Crash(new FLInvalidFunctionUseException("Break", "only one or zero arguments"), true);
            }
        }
    }
}