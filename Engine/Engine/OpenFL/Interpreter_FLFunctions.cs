using System;
using System.Collections.Generic;
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

                byte channel = (byte) Convert.ChangeType(val, typeof(byte));
                if (channel >= _channelCount)
                {
                    Logger.Log("Script is enabling channels beyond channel count. Ignoring...",
                        DebugChannel.Warning | DebugChannel.OpenFL, 10);
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
                byte channel = (byte) Convert.ChangeType(_currentArgStack.Pop(), typeof(byte));
                temp[channel] = 1;
            }
            else
            {
                _currentBuffer = (CLBufferInfo) _currentArgStack.Pop();
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
                Logger.Log("Updating Channel Buffer", DebugChannel.Log | DebugChannel.OpenFL, 6);
                _activeChannels = temp;
                CLAPI.WriteToBuffer(_instance,_activeChannelBuffer, _activeChannels);
            }
            else
            {
                Logger.Log("Skipping Updating Channel Buffer", DebugChannel.Log | DebugChannel.OpenFL, 6);
            }
        }

        /// <summary>
        /// A function used as RandomFunc of type byte>
        /// </summary>
        /// <returns>a random byte</returns>
        private static byte randombytesource()
        {
            return (byte) rnd.Next();
        }

        /// <summary>
        /// The implementation of the command random
        /// </summary>
        private void cmd_writerandom()
        {
            if (_currentArgStack.Count == 0)
            {
                CLAPI.WriteRandom(_instance, _currentBuffer.Buffer, randombytesource, _activeChannels, false);
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

                CLAPI.WriteRandom(_instance, (obj as CLBufferInfo).Buffer, randombytesource, _activeChannels, false);
            }
        }

        /// <summary>
        /// The implementation of the command random
        /// </summary>
        private void cmd_writerandomu()
        {
            if (_currentArgStack.Count == 0)
            {
                CLAPI.WriteRandom(_instance, _currentBuffer.Buffer, randombytesource, _activeChannels, true);
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

                CLAPI.WriteRandom(_instance, (obj as CLBufferInfo).Buffer, randombytesource, _activeChannels, true);
            }
        }

        /// <summary>
        /// The implementation of the command jmp
        /// </summary>
        private void cmd_jump() //Dummy function. Implementation in AnalyzeLine(code) function(look for isDirectExecute)
        {
            Logger.Log("Jumping.", DebugChannel.Log | DebugChannel.OpenFL, 6);
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
    }
}