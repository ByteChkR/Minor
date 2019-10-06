using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinorEngine.debug;
using MinorEngine.engine.components.ui;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace MinorEngine.engine.components
{
    public class DebugConsoleComponent : AbstractComponent
    {
        private static int MaxConsoleLines => (GameEngine.Instance.Height - 100) / 20;
        private const string HelpText = "Press C to Open the FL Console";
        private const string ConsoleTitle = "GameEngine Console:";
        private UITextRendererComponent _title;
        private UITextRendererComponent _consoleInput;
        private UITextRendererComponent _consoleOutput;
        private UIImageRendererComponent _consoleOutputImage;
        private UITextRendererComponent _hintText;
        private UIImageRendererComponent _bgImage;
        private StringBuilder _sb;
        private StringBuilder _outSB;

        public delegate string ConsoleCommand(string[] args);

        private readonly Dictionary<string, ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();
        private Queue<string> _consoleOutBuffer;
        private readonly List<string> _commandHistory = new List<string>();
        private int _currentId;

        private int inputIndex;

        private bool _blinkActive;
        private readonly float _blinkMaxTime = 0.5f;
        private float _blinkTime;

        private bool _showConsole;
        private bool _invalidate;

        private static RenderTarget rt, rt2;

        protected override void OnDestroy()
        {
            GameEngine.Instance.RemoveRenderTarget(rt);
            rt.Dispose();
            GameEngine.Instance.RemoveRenderTarget(rt2);
            rt2.Dispose();
        }

        public static GameObject CreateConsole()
        {
            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UITextRender.fs"},
                {ShaderType.VertexShader, "shader/UITextRender.vs"}
            }, out ShaderProgram textShader);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UIRender.fs"},
                {ShaderType.VertexShader, "shader/UIRender.vs"}
            }, out ShaderProgram uiShader);

            GameObject obj = new GameObject("Console");
            GameObject _in = new GameObject("ConsoleInput");
            GameObject _out = new GameObject("ConsoleOutput");
            GameObject _titleObj = new GameObject("Title");
            GameObject _bgObj = new GameObject("BackgroundImage");
            GameObject _bgOutObj = new GameObject("BackgroundOutputImage");
            GameObject _hint = new GameObject("HintText");

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

            UIImageRendererComponent _bgImage = new UIImageRendererComponent(ResourceManager.TextureIO.FileToTexture("textures/black.png"), false, 0.65f, uiShader);
            _bgImage.RenderMask = 1 << 29;

            UIImageRendererComponent _bgOutImage = new UIImageRendererComponent(ResourceManager.TextureIO.FileToTexture("textures/black.png"), false, 0.4f, uiShader);
            _bgOutImage.RenderMask = 1 << 28;



            UITextRendererComponent _tText = new UITextRendererComponent("Arial", false, 1f, textShader)
            {
                Text = "GameEngine Console:"
            };
            UITextRendererComponent _tHint = new UITextRendererComponent("Arial", false, 1f, textShader)
            {
                Text = "GameEngine Console:"
            };

            UITextRendererComponent _tIn = new UITextRendererComponent("Arial", false, 1f, textShader)
            {
                Text = ""
            };

            UITextRendererComponent _tOut = new UITextRendererComponent("Arial", false, 1f, textShader)
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


        public void WriteToConsole(string text)
        {
            string[] arr = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in arr) _consoleOutBuffer.Enqueue(s);
            while (_consoleOutBuffer.Count > MaxConsoleLines) _consoleOutBuffer.Dequeue();
        }


        private string ToConsoleText()
        {
            _outSB.Clear();

            foreach (var line in _consoleOutBuffer) _outSB.Append(line + "\n");

            return _outSB.ToString();
        }

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

            ResourceManager.AddConsoleCommands(this);
        }

        private string cmd_Exit(string[] args)
        {
            ToggleConsole(false);
            return "Exited.";
        }


        private string cmd_Clear(string[] args)
        {
            _consoleOutBuffer.Clear();
            return "Cleared Output";
        }

        private string cmd_ListCmds(string[] args)
        {
            _sb.Clear();
            _sb.Append("Commands:");
            int col = 10;
            int count = 0;
            foreach (var consoleCommand in _commands)
            {
                count++;
                if (count % col == 0)
                    _sb.Append("\n ");
                else
                    _sb.Append("   ");
                _sb.Append(consoleCommand.Key);
            }

            return _sb.ToString();
        }

        public void AddCommand(string name, ConsoleCommand command)
        {
            if (!_commands.ContainsKey(name)) _commands.Add(name, command);
        }

        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (_showConsole)
            {
                if (inputIndex >= _sb.Length)
                    _sb.Append(e.KeyChar);
                else
                    _sb.Insert(inputIndex, e.KeyChar);

                inputIndex++;

                _invalidate = true;

                _currentId = _commandHistory.Count;
            }
        }

        private string cmdExOnConsole(string[] args)
        {
            if (args.Length == 0) return "Please enter a function to redirect";
            string ret = args[0] + "\n";

            List<string> words = args.ToList();
            if (_commands.TryGetValue(words[0], out ConsoleCommand cmd))
            {
                words.RemoveAt(0);
                string s = cmd?.Invoke(words.ToArray());
                if (s == null) ret += "No Return";
                else ret += s + "";
            }
            else
            {
                ret += "Command Not found";
            }

            Logger.Log(ret, DebugChannel.Log);

            return "Success";

        }

        protected override void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (_showConsole)
            {
                if (e.Key == Key.Enter)
                {
                    WriteToConsole(_sb.ToString());
                    List<string> words = _sb.ToString().Split(' ').ToList();

                    if (_commands.TryGetValue(words[0], out ConsoleCommand cmd))
                    {
                        words.RemoveAt(0);
                        string s = cmd?.Invoke(words.ToArray());
                        if (s == null) s = "No Return";

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
                        _sb.Remove(_sb.Length - 1, 1);
                    else
                        _sb.Remove(inputIndex - 1, 1);

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

        protected override void Update(float deltaTime)
        {
            _blinkTime += deltaTime;
            if (_blinkTime >= _blinkMaxTime)
            {
                _blinkTime = 0;
                _blinkActive = !_blinkActive;
                _invalidate = true;
            }


            if (_invalidate) Invalidate();
        }

        private void ToggleConsole(bool state)
        {
            if (_showConsole == state) return;

            if (_showConsole)
            {
                _consoleInput.Text = "";
                _title.Text = HelpText;
                _consoleOutput.Text = "";
            }

            _invalidate = true;
            _showConsole = state;
        }


        private void Invalidate()
        {
            _invalidate = false;
            if (_showConsole)
            {
                string input = "Something Went Wrong.";
                if (_currentId == _commandHistory.Count)
                    input = _sb.ToString();
                else if (_currentId >= 0) input = _commandHistory[_currentId];

                string inputCursor;

                if (_blinkActive)
                    inputCursor = "|";
                else
                    inputCursor = " ";
                if (inputIndex >= input.Length)
                    input = input + inputCursor;
                else
                    input = input.Insert(inputIndex, inputCursor);

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