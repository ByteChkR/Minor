﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using Engine.Core;
using Engine.IO;
using Engine.Rendering;
using Engine.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Engine.Debug
{

    /// <summary>
    /// A Convenient Debug Console that is quite versatile
    /// </summary>
    public class DebugConsoleComponent : AbstractComponent
    {
        /// <summary>
        /// The Maximum Amount of console lines that is *roughly* working for the usecase
        /// </summary>
        private static int MaxConsoleLines => (GameEngine.Instance.Height - 100) / 20;
        /// <summary>
        /// The Text to be displayed when the Console is Closed.
        /// </summary>
        private const string HelpText = "Press C to Open the FL Console";

        /// <summary>
        /// The Text to be displayed in the title bar when the Console is Opened.
        /// </summary>
        private const string ConsoleTitle = "GameEngine Console:";

        /// <summary>
        /// Reference to the Title Renderer Component
        /// </summary>
        private UITextRendererComponent _title;

        /// <summary>
        /// Reference to the Console Input Renderer Component
        /// </summary>
        private UITextRendererComponent _consoleInput;

        /// <summary>
        /// Reference to the Console Output Renderer Component
        /// </summary>
        private UITextRendererComponent _consoleOutput;

        /// <summary>
        /// Reference to the Console output background Renderer Component
        /// </summary>
        private UIImageRendererComponent _consoleOutputImage;

        /// <summary>
        /// Reference to the Hint Text Renderer Component
        /// </summary>
        private UITextRendererComponent _hintText;

        /// <summary>
        /// Reference to the Background Image Renderer Component
        /// </summary>
        private UIImageRendererComponent _bgImage;

        /// <summary>
        /// String builder used for inputs from the used
        /// </summary>
        private StringBuilder _sb;

        /// <summary>
        /// String builder used for outputs to the console.
        /// </summary>
        private StringBuilder _outSB;

        /// <summary>
        /// Delegate representing a command.
        /// </summary>
        /// <param name="args">The commands that were provided by the user input</param>
        /// <returns></returns>
        public delegate string ConsoleCommand(string[] args);

        /// <summary>
        /// Internal List of Commands
        /// </summary>
        private readonly Dictionary<string, ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();

        /// <summary>
        /// Queue that is storing the Log Text
        /// </summary>
        private Queue<string> _consoleOutBuffer;

        /// <summary>
        /// A list used to store previous inputs from the user
        /// To provide a more "console~ish" feel
        /// </summary>
        private readonly List<string> _commandHistory = new List<string>();

        /// <summary>
        /// The current Index in the command history
        /// </summary>
        private int _currentId;

        /// <summary>
        /// The current position in the input(the position of characters)
        /// </summary>
        private int inputIndex;

        /// <summary>
        /// If the Text Cursor will be shown
        /// </summary>
        private bool _blinkActive;


        /// <summary>
        /// The maximum time the cursor stays idle before changing its visible state.
        /// </summary>
        private readonly float _blinkMaxTime = 0.5f;

        /// <summary>
        /// The current time for the blinking
        /// </summary>
        private float _blinkTime;

        /// <summary>
        /// Flag to show/disable the console
        /// Also disables character input
        /// </summary>
        private bool _showConsole;

        /// <summary>
        /// Flag used to indicate that the console window has changed and we need to redraw it
        /// </summary>
        private bool _invalidate;

        /// <summary>
        /// Render targets for the Background Textures(Used as a workaround because wierd UI rendering issues)
        /// I think Texture buffer gets updated with wrong blending that will overwrite the pixel data even with alpha 0
        /// </summary>
        private static RenderTarget rt, rt2;

        /// <summary>
        /// OnDestroy Implementation that will remove the two render targets from the system
        /// </summary>
        protected override void OnDestroy()
        {
            GameEngine.Instance.RemoveRenderTarget(rt);
            rt.Dispose();
            GameEngine.Instance.RemoveRenderTarget(rt2);
            rt2.Dispose();
        }

        /// <summary>
        /// Static function that will assemble the Console
        /// </summary>
        /// <returns>A gameobject ready to be added to the game</returns>
        public static GameObject CreateConsole()
        {
            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UITextRender.fs"},
                {ShaderType.VertexShader, "shader/UITextRender.vs"}
            }, out var textShader);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UIRender.fs"},
                {ShaderType.VertexShader, "shader/UIRender.vs"}
            }, out var uiShader);

            var obj = new GameObject("Console");
            var _in = new GameObject("ConsoleInput");
            var _out = new GameObject("ConsoleOutput");
            var _titleObj = new GameObject("Title");
            var _bgObj = new GameObject("BackgroundImage");
            var _bgOutObj = new GameObject("BackgroundOutputImage");
            var _hint = new GameObject("HintText");

            obj.Add(_in);
            obj.Add(_out);
            obj.Add(_titleObj);
            obj.Add(_bgObj);
            obj.Add(_hint);
            obj.Add(_bgOutObj);

            rt = new RenderTarget(null, 1 << 29, new Color(0, 0, 0, 0));
            GameEngine.Instance.AddRenderTarget(rt);

            rt2 = new RenderTarget(null, 1 << 28, new Color(0, 0, 0, 0));
            GameEngine.Instance.AddRenderTarget(rt2);

            var _bgImage =
                new UIImageRendererComponent(TextureLoader.FileToTexture("textures/black.png"), false,
                    0.65f, uiShader);
            _bgImage.RenderMask = 1 << 29;

            var _bgOutImage =
                new UIImageRendererComponent(TextureLoader.FileToTexture("textures/black.png"), false, 0.4f,
                    uiShader);
            _bgOutImage.RenderMask = 1 << 28;


            var _tText = new UITextRendererComponent("Arial", false, 1f, textShader)
            {
                Text = "GameEngine Console:"
            };
            var _tHint = new UITextRendererComponent("Arial", false, 1f, textShader)
            {
                Text = "GameEngine Console:"
            };

            var _tIn = new UITextRendererComponent("Arial", false, 1f, textShader)
            {
                Text = ""
            };

            var _tOut = new UITextRendererComponent("Arial", false, 1f, textShader)
            {
                Text = "Console Initialized.."
            };


            _bgObj.AddComponent(_bgImage);
            _in.AddComponent(_tIn);
            _out.AddComponent(_tOut);
            _titleObj.AddComponent(_tText);
            _hint.AddComponent(_tHint);
            _bgOutObj.AddComponent(_bgOutImage);

            _tText.Position = new Vector2(-0.39f, 0.353f);
            _tText.Scale = new Vector2(2f, 2f);

            _tOut.Position = new Vector2(-0.33f, 0.31f);
            _tOut.Scale = new Vector2(0.8f, 0.8f);

            _tIn.Position = new Vector2(-0.39f, -0.39f);
            _tIn.Scale = new Vector2(1.5f, 1.5f);


            _tHint.Position = new Vector2(-0.46f, -0.46f);
            _tHint.Scale = new Vector2(1.5f, 1.5f);


            _bgImage.Scale = new Vector2(0.8f);
            _bgOutImage.Scale = new Vector2(0.7f);

            obj.AddComponent(new DebugConsoleComponent());
            return obj;
        }


        /// <summary>
        /// Writes the Specified text to the console. and removes the oldest messages when reaching message limit.
        /// </summary>
        /// <param name="text"></param>
        public void WriteToConsole(string text)
        {
            var arr = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in arr)
            {
                _consoleOutBuffer.Enqueue(s);
            }

            while (_consoleOutBuffer.Count > MaxConsoleLines)
            {
                _consoleOutBuffer.Dequeue();
            }
        }


        /// <summary>
        /// Constructs the Console Output
        /// </summary>
        /// <returns></returns>
        private string ToConsoleText()
        {
            _outSB.Clear();

            foreach (var line in _consoleOutBuffer)
            {
                _outSB.Append(line + "\n");
            }

            return _outSB.ToString();
        }

        /// <summary>
        /// Awake function getting all the references in need and also adding a few basic commands
        /// </summary>
        protected override void Awake()
        {
            _consoleOutBuffer = new Queue<string>();
            _consoleOutBuffer.Enqueue("Console Initialized..");
            _title = Owner.GetChildWithName("Title").GetComponent<UITextRendererComponent>();
            _consoleInput = Owner.GetChildWithName("ConsoleInput").GetComponent<UITextRendererComponent>();
            _consoleOutput = Owner.GetChildWithName("ConsoleOutput").GetComponent<UITextRendererComponent>();
            _bgImage = Owner.GetChildWithName("BackgroundImage").GetComponent<UIImageRendererComponent>();
            _hintText = Owner.GetChildWithName("HintText").GetComponent<UITextRendererComponent>();
            _consoleOutputImage =
                Owner.GetChildWithName("BackgroundOutputImage").GetComponent<UIImageRendererComponent>();
            _sb = new StringBuilder();
            _outSB = new StringBuilder();
            AddCommand("help", cmd_ListCmds);
            AddCommand("h", cmd_ListCmds);
            AddCommand("q", cmd_Exit);
            AddCommand("exit", cmd_Exit);
            AddCommand("quit", cmd_Exit);
            AddCommand("cls", cmd_Clear);
            AddCommand("clear", cmd_Clear);
            AddCommand("lmem", MemoryTracer.cmdListMemoryInfo);
            AddCommand("llmem", MemoryTracer.cmdListLastMemoryInfo);
            AddCommand("cmd", cmdExOnConsole);
        }

        /// <summary>
        /// Command in Console: q/exit/quit
        /// Exits the console
        /// </summary>
        /// <param name="args">The arguments provided(0)</param>
        /// <returns>Result of Command</returns>
        private string cmd_Exit(string[] args)
        {
            ToggleConsole(false);
            return "Exited.";
        }

        /// <summary>
        /// Command in Console: clear/cls
        /// Clears the console output
        /// </summary>
        /// <param name="args">The arguments provided(0)</param>
        /// <returns>Result of Command</returns>
        private string cmd_Clear(string[] args)
        {
            _consoleOutBuffer.Clear();
            return "Cleared Output";
        }


        /// <summary>
        /// Command in Console: clear/cls
        /// Lists all Console Commands
        /// </summary>
        /// <param name="args">The arguments provided(0)</param>
        /// <returns>Result of Command</returns>
        private string cmd_ListCmds(string[] args)
        {
            _sb.Clear();
            _sb.Append("Commands:");
            var col = 10;
            var count = 0;
            foreach (var consoleCommand in _commands)
            {
                count++;
                if (count % col == 0)
                {
                    _sb.Append("\n ");
                }
                else
                {
                    _sb.Append("   ");
                }

                _sb.Append(consoleCommand.Key);
            }

            return _sb.ToString();
        }

        /// <summary>
        /// A function allowing for addition of commands
        /// </summary>
        /// <param name="name">The name for the command</param>
        /// <param name="command">The Command associated with the Name</param>
        public void AddCommand(string name, ConsoleCommand command)
        {
            if (!_commands.ContainsKey(name))
            {
                _commands.Add(name, command);
            }
        }

        /// <summary>
        /// Overridden OnKeyPress function
        /// </summary>
        /// <param name="sender">Basic Event handler stuff</param>
        /// <param name="e">Contains info about the KeyPress that has been occured</param>
        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (_showConsole)
            {
                if (inputIndex >= _sb.Length)
                {
                    _sb.Append(e.KeyChar);
                }
                else
                {
                    _sb.Insert(inputIndex, e.KeyChar);
                }

                inputIndex++;

                _invalidate = true;

                _currentId = _commandHistory.Count;
            }
        }


        /// <summary>
        /// Command in Console: clear/cls
        /// Writes the Command Output to the Actual CMD Console
        /// </summary>
        /// <param name="args">argument to redirect & parameters passed to the argument</param>
        /// <returns>Result of Command</returns>
        private string cmdExOnConsole(string[] args)
        {
            if (args.Length == 0)
            {
                return "Please enter a function to redirect";
            }

            var ret = args[0] + "\n";

            var words = args.ToList();
            if (_commands.TryGetValue(words[0], out var cmd))
            {
                words.RemoveAt(0);
                var s = cmd?.Invoke(words.ToArray());
                if (s == null)
                {
                    ret += "No Return";
                }
                else
                {
                    ret += s + "";
                }
            }
            else
            {
                ret += "Command Not found";
            }

            Logger.Log(ret, DebugChannel.Log);

            return "Success";
        }


        /// <summary>
        /// Overridden OnKeyUp function
        /// </summary>
        /// <param name="sender">Basic Event handler stuff</param>
        /// <param name="e">Contains info about the Key Event that has been raised</param>
        protected override void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (_showConsole)
            {
                if (e.Key == Key.Enter)
                {
                    WriteToConsole(_sb.ToString());
                    var words = _sb.ToString().Split(' ').ToList();

                    if (_commands.TryGetValue(words[0], out var cmd))
                    {
                        words.RemoveAt(0);
                        var s = cmd?.Invoke(words.ToArray());
                        if (s == null)
                        {
                            s = "No Return";
                        }

                        WriteToConsole(s);
                    }
                    else
                    {
                        WriteToConsole("Command Not found");
                    }

                    inputIndex = 0;
                    _commandHistory.Add(_sb.ToString());
                    _currentId = _commandHistory.Count;
                    _invalidate = true;
                    _sb.Clear();
                }
                else if (e.Key == Key.Right && inputIndex < _sb.Length)
                {
                    inputIndex++;
                }
                else if (e.Key == Key.Left && inputIndex > 0)
                {
                    inputIndex--;
                }
                else if (e.Key == Key.Up && _currentId != 0)
                {
                    _currentId--;


                    _sb.Clear();
                    _sb.Append(_commandHistory[_currentId]);
                    inputIndex = _sb.Length;
                }
                else if (e.Key == Key.Down && _currentId != _commandHistory.Count)
                {
                    _currentId++;

                    if (_currentId != _commandHistory.Count)
                    {
                        _sb.Clear();

                        _sb.Append(_commandHistory[_currentId]);
                        inputIndex = _sb.Length;
                    }
                }
                else if (inputIndex > 0 && e.Key == Key.BackSpace)
                {
                    if (inputIndex == _sb.Length)
                    {
                        _sb.Remove(_sb.Length - 1, 1);
                    }
                    else
                    {
                        _sb.Remove(inputIndex - 1, 1);
                    }

                    inputIndex--;

                    _invalidate = true;
                }
                else if (e.Key == Key.Escape && _sb.Length == 0)
                {
                    ToggleConsole(false);

                    _invalidate = true;
                }
                else if (e.Key == Key.Escape)
                {
                    _sb.Clear();

                    inputIndex = 0;

                    _invalidate = true;
                }
            }
            else
            {
                if (e.Key == Key.C)
                {
                    ToggleConsole(true);

                    inputIndex = 0;

                    _invalidate = true;
                }
            }
        }

        /// <summary>
        /// Update Function
        /// </summary>
        /// <param name="deltaTime">Delta Time in Seconds</param>
        protected override void Update(float deltaTime)
        {
            _blinkTime += deltaTime;
            if (_blinkTime >= _blinkMaxTime)
            {
                _blinkTime = 0;
                _blinkActive = !_blinkActive;
                _invalidate = true;
            }


            if (_invalidate)
            {
                Invalidate();
            }
        }


        /// <summary>
        /// Function that changes the console state from visible to invisible
        /// </summary>
        /// <param name="state"></param>
        private void ToggleConsole(bool state)
        {
            if (_showConsole == state)
            {
                return;
            }

            if (_showConsole)
            {
                _consoleInput.Text = "";
                _title.Text = HelpText;
                _consoleOutput.Text = "";
            }

            _invalidate = true;
            _showConsole = state;
        }

        /// <summary>
        /// When called it will update the console graphics
        /// </summary>
        private void Invalidate()
        {
            _invalidate = false;
            if (_showConsole)
            {
                var input = "Something Went Wrong.";
                if (_currentId == _commandHistory.Count)
                {
                    input = _sb.ToString();
                }
                else if (_currentId >= 0)
                {
                    input = _commandHistory[_currentId];
                }

                string inputCursor;

                if (_blinkActive)
                {
                    inputCursor = "|";
                }
                else
                {
                    inputCursor = " ";
                }

                if (inputIndex >= input.Length)
                {
                    input = input + inputCursor;
                }
                else
                {
                    input = input.Insert(inputIndex, inputCursor);
                }

                _consoleInput.Text = ">>> " + input;
            }

            _bgImage.Alpha = _showConsole ? 0.65f : 0;
            _consoleOutputImage.Alpha = _showConsole ? 0.75f : 0;

            _hintText.Text = _showConsole ? "" : HelpText;

            _title.Text = _showConsole ? ConsoleTitle : "";

            _consoleOutput.Text = _showConsole ? ToConsoleText() : "";
        }
    }
}