using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Core;
using Engine.DataTypes;
using Engine.IO;
using Engine.UI;
using OpenTK;
using OpenTK.Input;

namespace Engine.Debug
{
    /// <summary>
    /// A Convenient Debug Console that is quite versatile
    /// </summary>
    public class DebugConsoleComponent : AbstractComponent
    {
        /// <summary>
        /// Delegate representing a command.
        /// </summary>
        /// <param name="args">The commands that were provided by the user input</param>
        /// <returns></returns>
        public delegate string ConsoleCommand(string[] args);

        /// <summary>
        /// The Text to be displayed when the Console is Closed.
        /// </summary>
        private const string HelpText = "Press C to Open the FL Console";

        /// <summary>
        /// The Text to be displayed in the title bar when the Console is Opened.
        /// </summary>
        private const string ConsoleTitle = "GameEngine Console:";


        /// <summary>
        /// The maximum time the cursor stays idle before changing its visible state.
        /// </summary>
        private readonly float blinkMaxTime = 0.5f;

        /// <summary>
        /// A list used to store previous inputs from the user
        /// To provide a more "console~ish" feel
        /// </summary>
        private readonly List<string> commandHistory = new List<string>();

        /// <summary>
        /// Internal List of Commands
        /// </summary>
        private readonly Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();

        /// <summary>
        /// Reference to the Background Image Renderer Component
        /// </summary>
        private UiImageRendererComponent bgImage;

        /// <summary>
        /// If the Text Cursor will be shown
        /// </summary>
        private bool blinkActive;

        /// <summary>
        /// The current time for the blinking
        /// </summary>
        private float blinkTime;

        /// <summary>
        /// Reference to the Console Input Renderer Component
        /// </summary>
        private UiTextRendererComponent consoleInput;

        /// <summary>
        /// Queue that is storing the Log Text
        /// </summary>
        private Queue<string> consoleOutBuffer;

        /// <summary>
        /// Reference to the Console Output Renderer Component
        /// </summary>
        private UiTextRendererComponent consoleOutput;

        /// <summary>
        /// Reference to the Console output background Renderer Component
        /// </summary>
        private UiImageRendererComponent consoleOutputImage;

        /// <summary>
        /// The current Index in the command history
        /// </summary>
        private int currentId;

        /// <summary>
        /// The Graph component
        /// </summary>
        private GraphDrawingComponent graph;

        /// <summary>
        /// the Graph data stored.
        /// </summary>
        private Queue<float> graphData;

        /// <summary>
        /// Reference to the Hint Text Renderer Component
        /// </summary>
        private UiTextRendererComponent hintText;

        /// <summary>
        /// Flag used to indicate that the console window has changed and we need to redraw it
        /// </summary>
        private bool invalidate;

        /// <summary>
        /// The Maximum amount of graph data stored
        /// </summary>
        private readonly int maxGraphCount = 1600;

        /// <summary>
        /// String builder used for outputs to the console.
        /// </summary>
        private StringBuilder outSb;

        /// <summary>
        /// String builder used for inputs from the used
        /// </summary>
        private StringBuilder sb;

        /// <summary>
        /// Flag to show/disable the console
        /// Also disables character input
        /// </summary>
        private bool showConsole;

        /// <summary>
        /// Reference to the Title Renderer Component
        /// </summary>
        private UiTextRendererComponent title;

        private int fps;
        private float frames;

        /// <summary>
        /// The current position in the input(the position of characters)
        /// </summary>
        private int inputIndex;

        private float time;

        /// <summary>
        /// The Maximum Amount of console lines that is *roughly* working for the usecase
        /// </summary>
        private static int MaxConsoleLines => (GameEngine.Instance.Height - 100) / 20;


        /// <summary>
        /// OnDestroy Implementation that will remove the two render targets from the system
        /// </summary>
        protected override void OnDestroy()
        {
            //TODO Dispose correct objects if not attached to a gameobject
        }

        /// <summary>
        /// Static function that will assemble the Console
        /// </summary>
        /// <returns>A gameobject ready to be added to the game</returns>
        public static GameObject CreateConsole()
        {
            GameObject obj = new GameObject("Console");
            GameObject _in = new GameObject("ConsoleInput");
            GameObject _out = new GameObject("ConsoleOutput");
            GameObject titleObj = new GameObject("Title");
            GameObject bgObj = new GameObject("BackgroundImage");
            GameObject bgOutObj = new GameObject("BackgroundOutputImage");
            GameObject hint = new GameObject("HintText");
            GameObject graph = new GameObject("Graph");

            obj.Add(_in);
            obj.Add(_out);
            obj.Add(titleObj);
            obj.Add(bgObj);
            obj.Add(hint);
            obj.Add(bgOutObj);
            obj.Add(graph);

            Bitmap bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.Black);

            UiImageRendererComponent bgImage =
                new UiImageRendererComponent(TextureLoader.BitmapToTexture(bmp), false, 0.65f,
                    DefaultFilepaths.DefaultUiImageShader);

            UiImageRendererComponent bgOutImage =
                new UiImageRendererComponent(TextureLoader.BitmapToTexture(bmp), false, 0.4f,
                    DefaultFilepaths.DefaultUiImageShader);


            UiTextRendererComponent tText =
                new UiTextRendererComponent(DefaultFilepaths.DefaultFont, false, 1f,
                    DefaultFilepaths.DefaultUiTextShader)
                {
                    Text = "GameEngine Console:"
                };
            UiTextRendererComponent tHint =
                new UiTextRendererComponent(DefaultFilepaths.DefaultFont, false, 1f,
                    DefaultFilepaths.DefaultUiTextShader)
                {
                    Text = "GameEngine Console:"
                };

            UiTextRendererComponent tIn =
                new UiTextRendererComponent(DefaultFilepaths.DefaultFont, false, 1f,
                    DefaultFilepaths.DefaultUiTextShader)
                {
                    Text = ""
                };

            UiTextRendererComponent tOut =
                new UiTextRendererComponent(DefaultFilepaths.DefaultFont, false, 1f,
                    DefaultFilepaths.DefaultUiTextShader)
                {
                    Text = "Console Initialized.."
                };

            GraphDrawingComponent gDraw = new GraphDrawingComponent(DefaultFilepaths.DefaultUiGraphShader, false, 1f);

            bgOutObj.AddComponent(bgOutImage);
            bgObj.AddComponent(bgImage);
            graph.AddComponent(gDraw);
            _in.AddComponent(tIn);
            _out.AddComponent(tOut);
            titleObj.AddComponent(tText);
            hint.AddComponent(tHint);

            gDraw.Scale = new Vector2(0.5f, 0.5f);
            gDraw.Position = new Vector2(0.5f);
            gDraw.Points = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 1f)
            };

            tText.Position = new Vector2(-0.39f, 0.353f) * 2;
            tText.Scale = new Vector2(2f, 2f);

            tOut.Position = new Vector2(-0.33f, 0.31f) * 2;
            tOut.Scale = new Vector2(0.8f, 0.8f);

            tIn.Position = new Vector2(-0.39f, -0.39f) * 2;
            tIn.Scale = new Vector2(1.5f, 1.5f);


            tHint.Position = new Vector2(-0.46f, -0.46f) * 2;
            tHint.Scale = new Vector2(1.5f, 1.5f);


            bgImage.Scale = new Vector2(0.8f);
            bgOutImage.Scale = new Vector2(0.7f);

            obj.AddComponent(new DebugConsoleComponent());
            return obj;
        }

        /// <summary>
        /// Updates the graph
        /// </summary>
        private void UpdateGraph()
        {
            Vector2[] graphPoints = new Vector2[graphData.Count];
            float[] serializedComponents = graphData.ToArray();
            float xInc = 1f / graphPoints.Length;
            for (int i = 0; i < graphPoints.Length; i++)
            {
                graphPoints[i] = new Vector2(xInc * i, serializedComponents[i]);
            }

            graph.Points = graphPoints;
        }


        /// <summary>
        /// Adds a value to the graph data
        /// </summary>
        /// <param name="yValue"></param>
        public void AddGraphValue(float yValue)
        {
            graphData.Enqueue(yValue);
            while (graphData.Count >= maxGraphCount)
            {
                graphData.Dequeue();
            }

            UpdateGraph();
        }

        /// <summary>
        /// Writes the Specified text to the console. and removes the oldest messages when reaching message limit.
        /// </summary>
        /// <param name="text"></param>
        public void WriteToConsole(string text)
        {
            string[] arr = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in arr)
            {
                consoleOutBuffer.Enqueue(s);
            }

            while (consoleOutBuffer.Count > MaxConsoleLines)
            {
                consoleOutBuffer.Dequeue();
            }
        }


        /// <summary>
        /// Constructs the Console Output
        /// </summary>
        /// <returns></returns>
        private string ToConsoleText()
        {
            outSb.Clear();

            foreach (string line in consoleOutBuffer)
            {
                outSb.Append(line + "\n");
            }

            return outSb.ToString();
        }

        /// <summary>
        /// Awake function getting all the references in need and also adding a few basic commands
        /// </summary>
        protected override void Awake()
        {
            graphData = new Queue<float>();
            MemoryTracer.SetDebugComponent(this);
            consoleOutBuffer = new Queue<string>();
            consoleOutBuffer.Enqueue("Console Initialized..");
            title = Owner.GetChildWithName("Title").GetComponent<UiTextRendererComponent>();
            consoleInput = Owner.GetChildWithName("ConsoleInput").GetComponent<UiTextRendererComponent>();
            consoleOutput = Owner.GetChildWithName("ConsoleOutput").GetComponent<UiTextRendererComponent>();
            bgImage = Owner.GetChildWithName("BackgroundImage").GetComponent<UiImageRendererComponent>();
            hintText = Owner.GetChildWithName("HintText").GetComponent<UiTextRendererComponent>();
            graph = Owner.GetChildWithName("Graph").GetComponent<GraphDrawingComponent>();
            graph.Enabled = false;
            consoleOutputImage =
                Owner.GetChildWithName("BackgroundOutputImage").GetComponent<UiImageRendererComponent>();
            sb = new StringBuilder();
            outSb = new StringBuilder();
            AddCommand("help", cmd_ListCmds);
            AddCommand("h", cmd_ListCmds);
            AddCommand("q", cmd_Exit);
            AddCommand("exit", cmd_Exit);
            AddCommand("quit", cmd_Exit);
            AddCommand("cls", cmd_Clear);
            AddCommand("clear", cmd_Clear);
            AddCommand("lmem", MemoryTracer.CmdListMemoryInfo);
            AddCommand("llmem", MemoryTracer.CmdListLastMemoryInfo);
            AddCommand("cmd", CmdExOnConsole);
            AddCommand("tg", cmd_ToggleGraph);
            AddCommand("togglegraph", cmd_ToggleGraph);
            AddCommand("reload", cmd_ReLoadScene);
            AddCommand("load", cmd_LoadScene);
            AddCommand("listscenes", cmd_ListScenes);
            AddCommand("ls", cmd_ListScenes);
        }

        private static Dictionary<string, Type> SceneList
        {
            get
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Dictionary<string, Type> ret = new Dictionary<string, Type>();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    Type[] types = assemblies[i].GetTypes();
                    foreach (Type type in types)
                    {
                        if (typeof(AbstractScene) != type && typeof(AbstractScene).IsAssignableFrom(type))
                        {
                            if (!ret.ContainsKey(type.Name))
                                ret.Add(type.Name, type);
                            else if (type != ret[type.Name])
                            {
                                Type t1 = ret[type.Name];
                                ret.Remove(type.Name);
                                ret.Add(t1.FullName, t1);
                                ret.Add(type.FullName, type);
                            }
                        }
                    }

                }

                return ret;
            }
        }

        private static string cmd_ListScenes(string[] args)
        {
            string ret = "";
            foreach (KeyValuePair<string, Type> keyValuePair in SceneList)
            {
                ret += keyValuePair.Key + "\n";
            }

            return ret;
        }

        private static string cmd_LoadScene(string[] args)
        {
            if (args.Length < 1 || !SceneList.ContainsKey(args[0]))
            {
                return "Scene Not found";
            }
            else
            {
                GameEngine.Instance.InitializeScene(SceneList[args[0]]);

                return "Scene Loaded";
            }
        }

        private static string cmd_ReLoadScene(string[] args)
        {
            GameEngine.Instance.InitializeScene(GameEngine.Instance.CurrentScene.GetType());
            return "Reloaded";
        }

        /// <summary>
        /// Command in Console: q/exit/quit
        /// Exits the console
        /// </summary>
        /// <param name="args">The arguments provided(0)</param>
        /// <returns>Result of Command</returns>
        private string cmd_ToggleGraph(string[] args)
        {
            graph.Enabled = !graph.Enabled;
            return "Enabled Graph: " + graph.Enabled;
        }

        /// <summary>
        /// Command in Console: tg/togglegraph
        /// Shows/Hides the Graph
        /// </summary>
        /// <param name="args">The arguments provided(0)</param>
        /// <returns>Result of Command</returns>
        private string cmd_Exit(string[] args)
        {
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
            consoleOutBuffer.Clear();
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
            sb.Clear();
            sb.Append("Commands:");
            int col = 10;
            int count = 0;
            foreach (KeyValuePair<string, ConsoleCommand> consoleCommand in commands)
            {
                count++;
                if (count % col == 0)
                {
                    sb.Append("\n ");
                }
                else
                {
                    sb.Append("   ");
                }

                sb.Append(consoleCommand.Key);
            }

            return sb.ToString();
        }

        /// <summary>
        /// A function allowing for addition of commands
        /// </summary>
        /// <param name="name">The name for the command</param>
        /// <param name="command">The Command associated with the Name</param>
        public void AddCommand(string name, ConsoleCommand command)
        {
            if (!commands.ContainsKey(name))
            {
                commands.Add(name, command);
            }
        }

        /// <summary>
        /// Overridden OnKeyPress function
        /// </summary>
        /// <param name="sender">Basic Event handler stuff</param>
        /// <param name="e">Contains info about the KeyPress that has been occured</param>
        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (showConsole)
            {
                if (inputIndex >= sb.Length)
                {
                    sb.Append(e.KeyChar);
                }
                else
                {
                    sb.Insert(inputIndex, e.KeyChar);
                }

                inputIndex++;

                invalidate = true;

                currentId = commandHistory.Count;
            }
        }


        /// <summary>
        /// Command in Console: clear/cls
        /// Writes the Command Output to the Actual CMD Console
        /// </summary>
        /// <param name="args">argument to redirect & parameters passed to the argument</param>
        /// <returns>Result of Command</returns>
        private string CmdExOnConsole(string[] args)
        {
            if (args.Length == 0)
            {
                return "Please enter a function to redirect";
            }

            string ret = args[0] + "\n";

            List<string> words = args.ToList();
            if (commands.TryGetValue(words[0], out ConsoleCommand cmd))
            {
                words.RemoveAt(0);
                string s = cmd?.Invoke(words.ToArray());
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

            Logger.Log(ret, DebugChannel.Log | DebugChannel.Debug, int.MaxValue);

            return "Success";
        }


        /// <summary>
        /// Overridden OnKeyUp function
        /// </summary>
        /// <param name="sender">Basic Event handler stuff</param>
        /// <param name="e">Contains info about the Key Event that has been raised</param>
        protected override void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (showConsole)
            {
                if (e.Key == Key.Enter)
                {
                    WriteToConsole(sb.ToString());
                    List<string> words = sb.ToString().Split(' ').ToList();

                    if (commands.TryGetValue(words[0], out ConsoleCommand cmd))
                    {
                        words.RemoveAt(0);
                        string s = cmd?.Invoke(words.ToArray());
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
                    commandHistory.Add(sb.ToString());
                    currentId = commandHistory.Count;
                    invalidate = true;
                    sb.Clear();
                }
                else if (e.Key == Key.Right && inputIndex < sb.Length)
                {
                    inputIndex++;
                }
                else if (e.Key == Key.Left && inputIndex > 0)
                {
                    inputIndex--;
                }
                else if (e.Key == Key.Up && currentId != 0)
                {
                    currentId--;


                    sb.Clear();
                    sb.Append(commandHistory[currentId]);
                    inputIndex = sb.Length;
                }
                else if (e.Key == Key.Down && currentId != commandHistory.Count)
                {
                    currentId++;

                    if (currentId != commandHistory.Count)
                    {
                        sb.Clear();

                        sb.Append(commandHistory[currentId]);
                        inputIndex = sb.Length;
                    }
                }
                else if (inputIndex > 0 && e.Key == Key.BackSpace)
                {
                    if (inputIndex == sb.Length)
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                    else
                    {
                        sb.Remove(inputIndex - 1, 1);
                    }

                    currentId = commandHistory.Count;
                    inputIndex--;

                    invalidate = true;
                }
                else if (e.Key == Key.Escape && sb.Length == 0)
                {
                    ToggleConsole(false);

                    invalidate = true;
                }
                else if (e.Key == Key.Escape)
                {
                    sb.Clear();

                    inputIndex = 0;

                    invalidate = true;
                }
            }
            else
            {
                if (e.Key == Key.C)
                {
                    ToggleConsole(true);

                    inputIndex = 0;

                    invalidate = true;
                }
            }
        }

        /// <summary>
        /// Update Function
        /// </summary>
        /// <param name="deltaTime">Delta Time in Seconds</param>
        protected override void Update(float deltaTime)
        {
            time += deltaTime;
            fps++;
            if (time >= 1)
            {
                frames = fps;
                time = fps = 0;
            }

            blinkTime += deltaTime;
            if (blinkTime >= blinkMaxTime)
            {
                blinkTime = 0;
                blinkActive = !blinkActive;
                invalidate = true;
            }


            if (invalidate)
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
            if (showConsole == state)
            {
                return;
            }

            if (showConsole)
            {
                consoleInput.Text = "";
                title.Text = HelpText;
                consoleOutput.Text = "";
            }

            invalidate = true;
            showConsole = state;
        }

        /// <summary>
        /// When called it will update the console graphics
        /// </summary>
        private void Invalidate()
        {
            invalidate = false;
            if (showConsole)
            {
                string input = "Something Went Wrong.";
                if (currentId == commandHistory.Count)
                {
                    input = sb.ToString();
                }
                else if (currentId >= 0)
                {
                    input = commandHistory[currentId];
                }

                string inputCursor;

                if (blinkActive)
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

                consoleInput.Text = ">>> " + input;
            }

            bgImage.Alpha = showConsole ? 0.65f : 0;
            consoleOutputImage.Alpha = showConsole ? 0.75f : 0;


            hintText.Text = showConsole ? "FPS: " + frames : HelpText;

            title.Text = showConsole ? ConsoleTitle : "";

            consoleOutput.Text = showConsole ? ToConsoleText() : "";
        }
    }
}