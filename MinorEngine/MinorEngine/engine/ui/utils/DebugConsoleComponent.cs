using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GameEngine.engine.components;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GameEngine.engine.ui.utils
{
    public class DebugConsoleComponent : AbstractComponent
    {

        private const string HelpText = "Press C to Open the FL Console";
        private const string ConsoleTitle = "Game Console:";
        private UITextRendererComponent _title;
        private string _consoleOutputCache;
        private UITextRendererComponent _consoleInput;
        private UITextRendererComponent _consoleOutput;
        private StringBuilder _sb;
        public delegate string ConsoleCommand(string[] args);

        private readonly Dictionary<string, ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();
        private readonly List<string> _commandHistory = new List<string>();
        private int _currentId;


        private bool _blinkActive;
        private readonly float _blinkMaxTime = 0.5f;
        private float _blinkTime;

        private bool _showConsole;
        private bool _invalidate;



        public static GameObject CreateConsole()
        {
            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UITextRender.fs"},
                {ShaderType.VertexShader, "shader/UITextRender.vs"},
            }, out ShaderProgram textShader);

            GameObject obj = new GameObject("Console");
            GameObject _in = new GameObject("ConsoleInput");
            GameObject _out = new GameObject("ConsoleOutput");
            GameObject _titleObj = new GameObject("Title");

            obj.Add(_in);
            obj.Add(_out);
            obj.Add(_titleObj);

            UITextRendererComponent _tText = new UITextRendererComponent("Arial", textShader)
            {
                Position = new Vector2(-0.46f, 0.46f),
                Scale = new Vector2(2f, 2f),
                Text = "Game Console:"
            };

            UITextRendererComponent _tIn = new UITextRendererComponent("Arial", textShader)
            {
                Position = new Vector2(-0.46f, -0.46f),
                Scale = new Vector2(1.5f, 1.5f),
                Text = ""
            };

            UITextRendererComponent _tOut = new UITextRendererComponent("Arial", textShader)
            {
                Position = new Vector2(-0.4f, 0.4f),
                Scale = new Vector2(1.5f, 1.5f),
                Text = "Console Initialized.."
            };
            obj.AddComponent(new DebugConsoleComponent());
            _in.AddComponent(_tIn);
            _out.AddComponent(_tOut);
            _titleObj.AddComponent(_tText);
            return obj;

        }


        protected override void Awake()
        {
            _consoleOutputCache = "Console Initialized..";
            _title = Owner.GetChildWithName("Title").GetComponent<UITextRendererComponent>();
            _consoleInput = Owner.GetChildWithName("ConsoleInput").GetComponent<UITextRendererComponent>();
            _consoleOutput = Owner.GetChildWithName("ConsoleOutput").GetComponent<UITextRendererComponent>();
            _sb = new StringBuilder();
            AddCommand("help", cmd_ListCmds);
            AddCommand("h", cmd_ListCmds);
            AddCommand("q", cmd_Exit);
            AddCommand("exit", cmd_Exit);
            AddCommand("quit", cmd_Exit);
        }

        private string cmd_Exit(string[] args)
        {
            ToggleConsole(false);
            return "Exited.";
        }

        
        private string cmd_ListCmds(string[] args)
        {
            _sb.Clear();
            _sb.Append("Commands:");
            foreach (var consoleCommand in _commands)
            {
                _sb.Append("\n ");
                _sb.Append(consoleCommand.Key);
            }

            return _sb.ToString();
        }

        public void AddCommand(string name, ConsoleCommand command)
        {
            if (!_commands.ContainsKey(name))
            {
                _commands.Add(name, command);
            }
        }

        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (_showConsole)
            {
                _sb.Append(e.KeyChar);

                _invalidate = true;

                _currentId = _commandHistory.Count;
            }

        }

        protected override void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (_showConsole)
            {
                if (e.Key == Key.Enter)
                {
                    List<string> words = _sb.ToString().Split(' ').ToList();

                    if (_commands.TryGetValue(words[0], out ConsoleCommand cmd))
                    {
                        words.RemoveAt(0);
                        string s = cmd?.Invoke(words.ToArray());
                        if (s == null)
                        {
                            s = "No Return";
                        }

                        _consoleOutputCache = s;
                    }
                    else
                    {
                        _consoleOutputCache = "Command Not found";
                    }
                    _commandHistory.Add(_sb.ToString());
                    _currentId = _commandHistory.Count;
                    _invalidate = true;
                    _sb.Clear();
                }
                else if (e.Key == Key.Up && _currentId != 0)
                {
                    _currentId--;


                    _sb.Clear();
                    _sb.Append(_commandHistory[_currentId]);

                }
                else if (e.Key == Key.Down && _currentId != _commandHistory.Count)
                {
                    _currentId++;

                    if (_currentId != _commandHistory.Count)
                    {
                        _sb.Clear();
                        _sb.Append(_commandHistory[_currentId]);
                    }

                }
                else if (_sb.Length > 0 && e.Key == Key.BackSpace)
                {
                    _sb.Remove(_sb.Length - 1, 1);

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

                    _invalidate = true;
                }
            }
            else
            {
                if (e.Key == Key.C)
                {
                    ToggleConsole(true);

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


            if (_invalidate)
            {
                Invalidate();
            }

        }

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



        private void Invalidate()
        {
            _consoleOutput.Text = _consoleOutputCache;
            _invalidate = false;
            if (_showConsole)
            {
                string input = "Something Went Wrong.";
                if (_currentId == _commandHistory.Count)
                {
                    input = _sb.ToString();
                }
                else if (_currentId >= 0)
                {
                    input = _commandHistory[_currentId];
                }
                _consoleInput.Text = ">>> " + input + (_blinkActive ? "_" : "");


            }
            _title.Text = _showConsole ? ConsoleTitle : HelpText;

            _consoleOutput.Text = _showConsole ? _consoleOutputCache : "";
        }

    }
}