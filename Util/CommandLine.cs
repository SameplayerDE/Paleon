using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class CommandLine
    {
        private struct CommandInfo
        {
            public Action<string[]> Action;
            public string Help;
            public string Usage;
        }

        private struct Line
        {
            public string Text;
            public Color Color;

            public Line(string text)
            {
                Text = text;
                Color = Color.White;
            }

            public Line(string text, Color color)
            {
                Text = text;
                Color = color;
            }
        }

        private const float UNDERSCORE_TIME = .5f;
        private const float REPEAT_DELAY = .5f;
        private const float REPEAT_EVERY = 1 / 30f;
        private const float OPACITY = .8f;

        public bool Enabled = true;
        public bool Open;
        public Action[] FunctionKeyActions { get; private set; }

        private Dictionary<string, CommandInfo> commands;
        private List<string> sorted;

        private KeyboardState oldState;
        private KeyboardState currentState;
        private string currentText = "";
        private List<Line> drawCommands;
        private bool underscore;
        private float underscoreCounter;
        private List<string> commandHistory;
        private int seekIndex = -1;
        private int tabIndex = -1;
        private string tabSearch;
        private float repeatCounter = 0;
        private Keys? repeatKey = null;
        private bool canOpen;

        public CommandLine()
        {
            commandHistory = new List<string>();
            drawCommands = new List<Line>();
            commands = new Dictionary<string, CommandInfo>();
            sorted = new List<string>();
            FunctionKeyActions = new Action[12];

            BuildCommandsList();
        }

        private void BuildCommandsList()
        {
#if !CONSOLE
            //Check Monocle for Commands
            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    ProcessMethod(method);

            //Check the calling assembly for Commands
            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    ProcessMethod(method);

            //Maintain the sorted command list
            foreach (var command in commands)
                sorted.Add(command.Key);
            sorted.Sort();
#endif
        }

        private void ProcessMethod(MethodInfo method)
        {
            Command attr = null;
            {
                var attrs = method.GetCustomAttributes(typeof(Command), false);
                if (attrs.Length > 0)
                    attr = attrs[0] as Command;
            }

            if (attr != null)
            {
                if (!method.IsStatic)
                    throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but is not static");
                else
                {
                    CommandInfo info = new CommandInfo();
                    info.Help = attr.Help;

                    var parameters = method.GetParameters();
                    var defaults = new object[parameters.Length];
                    string[] usage = new string[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        usage[i] = p.Name + ":";

                        if (p.ParameterType == typeof(string))
                            usage[i] += "string";
                        else if (p.ParameterType == typeof(int))
                            usage[i] += "int";
                        else if (p.ParameterType == typeof(float))
                            usage[i] += "float";
                        else if (p.ParameterType == typeof(bool))
                            usage[i] += "bool";
                        else
                            throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but has an invalid parameter type. Allowed types are: string, int, float, and bool");

                        if (p.DefaultValue == DBNull.Value)
                            defaults[i] = null;
                        else if (p.DefaultValue != null)
                        {
                            defaults[i] = p.DefaultValue;
                            if (p.ParameterType == typeof(string))
                                usage[i] += "=\"" + p.DefaultValue + "\"";
                            else
                                usage[i] += "=" + p.DefaultValue;
                        }
                        else
                            defaults[i] = null;
                    }

                    if (usage.Length == 0)
                        info.Usage = "";
                    else
                        info.Usage = "[" + string.Join(" ", usage) + "]";

                    info.Action = (args) =>
                    {
                        if (parameters.Length == 0)
                            InvokeMethod(method);
                        else
                        {
                            object[] param = (object[])defaults.Clone();

                            for (int i = 0; i < param.Length && i < args.Length; i++)
                            {
                                if (parameters[i].ParameterType == typeof(string))
                                    param[i] = ArgString(args[i]);
                                else if (parameters[i].ParameterType == typeof(int))
                                    param[i] = ArgInt(args[i]);
                                else if (parameters[i].ParameterType == typeof(float))
                                    param[i] = ArgFloat(args[i]);
                                else if (parameters[i].ParameterType == typeof(bool))
                                    param[i] = ArgBool(args[i]);
                            }

                            InvokeMethod(method, param);
                        }
                    };

                    commands[attr.Name] = info;
                }
            }
        }

        private void InvokeMethod(MethodInfo method, object[] param = null)
        {
            try
            {
                method.Invoke(null, param);
            }
            catch (Exception e)
            {
                Engine.CommandLine.Log(e.InnerException.Message, Color.Yellow);
                LogStackTrace(e.InnerException.StackTrace);
            }
        }

        private void LogStackTrace(string stackTrace)
        {
            foreach (var call in stackTrace.Split('\n'))
            {
                string log = call;

                //Remove File Path
                {
                    var from = log.LastIndexOf(" in ") + 4;
                    var to = log.LastIndexOf('\\') + 1;
                    if (from != -1 && to != -1)
                        log = log.Substring(0, from) + log.Substring(to);
                }

                //Remove arguments list
                {
                    var from = log.IndexOf('(') + 1;
                    var to = log.IndexOf(')');
                    if (from != -1 && to != -1)
                        log = log.Substring(0, from) + log.Substring(to);
                }

                //Space out the colon line number
                var colon = log.LastIndexOf(':');
                if (colon != -1)
                    log = log.Insert(colon + 1, " ").Insert(colon, " ");

                log = log.TrimStart();
                log = "-> " + log;

                Engine.CommandLine.Log(log, Color.White);
            }
        }

        private static string ArgString(string arg)
        {
            if (arg == null)
                return "";
            else
                return arg;
        }

        private static bool ArgBool(string arg)
        {
            if (arg != null)
                return !(arg == "0" || arg.ToLower() == "false" || arg.ToLower() == "f");
            else
                return false;
        }

        private static int ArgInt(string arg)
        {
            try
            {
                return Convert.ToInt32(arg);
            }
            catch
            {
                return 0;
            }
        }

        private static float ArgFloat(string arg)
        {
            try
            {
                return Convert.ToSingle(arg);
            }
            catch
            {
                return 0;
            }
        }

        internal void UpdateClosed()
        {
            if (!canOpen)
                canOpen = true;
            else if (MInput.Keyboard.Pressed(Keys.OemTilde, Keys.Oem8))
            {
                Open = true;
                currentState = Keyboard.GetState();
            }

            for (int i = 0; i < FunctionKeyActions.Length; i++)
                if (MInput.Keyboard.Pressed((Keys)(Keys.F1 + i)))
                    ExecuteFunctionKeyAction(i);
        }

        internal void UpdateOpen()
        {
            oldState = currentState;
            currentState = Keyboard.GetState();

            underscoreCounter += Engine.DeltaTime;
            while (underscoreCounter >= UNDERSCORE_TIME)
            {
                underscoreCounter -= UNDERSCORE_TIME;
                underscore = !underscore;
            }

            if (repeatKey.HasValue)
            {
                if (currentState[repeatKey.Value] == KeyState.Down)
                {
                    repeatCounter += Engine.DeltaTime;

                    while (repeatCounter >= REPEAT_DELAY)
                    {
                        HandleKey(repeatKey.Value);
                        repeatCounter -= REPEAT_EVERY;
                    }
                }
                else
                    repeatKey = null;
            }

            foreach (Keys key in currentState.GetPressedKeys())
            {
                if (oldState[key] == KeyState.Up)
                {
                    HandleKey(key);
                    break;
                }
            }
        }

        private void HandleKey(Keys key)
        {
            if (key != Keys.Tab && key != Keys.LeftShift && key != Keys.RightShift && key != Keys.RightAlt && key != Keys.LeftAlt && key != Keys.RightControl && key != Keys.LeftControl)
                tabIndex = -1;

            if (key != Keys.OemTilde && key != Keys.Oem8 && key != Keys.Enter && repeatKey != key)
            {
                repeatKey = key;
                repeatCounter = 0;
            }

            switch (key)
            {
                default:
                    if (key.ToString().Length == 1)
                    {
                        if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                            currentText += key.ToString();
                        else
                            currentText += key.ToString().ToLower();
                    }
                    break;

                case (Keys.D1):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '!';
                    else
                        currentText += '1';
                    break;
                case (Keys.D2):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '@';
                    else
                        currentText += '2';
                    break;
                case (Keys.D3):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '#';
                    else
                        currentText += '3';
                    break;
                case (Keys.D4):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '$';
                    else
                        currentText += '4';
                    break;
                case (Keys.D5):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '%';
                    else
                        currentText += '5';
                    break;
                case (Keys.D6):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '^';
                    else
                        currentText += '6';
                    break;
                case (Keys.D7):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '&';
                    else
                        currentText += '7';
                    break;
                case (Keys.D8):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '*';
                    else
                        currentText += '8';
                    break;
                case (Keys.D9):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '(';
                    else
                        currentText += '9';
                    break;
                case (Keys.D0):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += ')';
                    else
                        currentText += '0';
                    break;
                case (Keys.OemComma):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '<';
                    else
                        currentText += ',';
                    break;
                case Keys.OemPeriod:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '>';
                    else
                        currentText += '.';
                    break;
                case Keys.OemQuestion:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '?';
                    else
                        currentText += '/';
                    break;
                case Keys.OemSemicolon:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += ':';
                    else
                        currentText += ';';
                    break;
                case Keys.OemQuotes:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '"';
                    else
                        currentText += '\'';
                    break;
                case Keys.OemBackslash:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '|';
                    else
                        currentText += '\\';
                    break;
                case Keys.OemOpenBrackets:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '{';
                    else
                        currentText += '[';
                    break;
                case Keys.OemCloseBrackets:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '}';
                    else
                        currentText += ']';
                    break;
                case Keys.OemMinus:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '_';
                    else
                        currentText += '-';
                    break;
                case Keys.OemPlus:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '+';
                    else
                        currentText += '=';
                    break;

                case Keys.Space:
                    currentText += " ";
                    break;
                case Keys.Back:
                    if (currentText.Length > 0)
                        currentText = currentText.Substring(0, currentText.Length - 1);
                    break;
                case Keys.Delete:
                    currentText = "";
                    break;

                case Keys.Up:
                    if (seekIndex < commandHistory.Count - 1)
                    {
                        seekIndex++;
                        currentText = string.Join(" ", commandHistory[seekIndex]);
                    }
                    break;
                case Keys.Down:
                    if (seekIndex > -1)
                    {
                        seekIndex--;
                        if (seekIndex == -1)
                            currentText = "";
                        else
                            currentText = string.Join(" ", commandHistory[seekIndex]);
                    }
                    break;

                case Keys.Tab:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (tabIndex == -1)
                        {
                            tabSearch = currentText;
                            FindLastTab();
                        }
                        else
                        {
                            tabIndex--;
                            if (tabIndex < 0 || (tabSearch != "" && sorted[tabIndex].IndexOf(tabSearch) != 0))
                                FindLastTab();
                        }
                    }
                    else
                    {
                        if (tabIndex == -1)
                        {
                            tabSearch = currentText;
                            FindFirstTab();
                        }
                        else
                        {
                            tabIndex++;
                            if (tabIndex >= sorted.Count || (tabSearch != "" && sorted[tabIndex].IndexOf(tabSearch) != 0))
                                FindFirstTab();
                        }
                    }
                    if (tabIndex != -1)
                        currentText = sorted[tabIndex];
                    break;

                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                    ExecuteFunctionKeyAction((int)(key - Keys.F1));
                    break;

                case Keys.Enter:
                    if (currentText.Length > 0)
                        EnterCommand();
                    break;

                case Keys.Oem8:
                case Keys.OemTilde:
                    Open = canOpen = false;
                    break;
            }
        }

        private void EnterCommand()
        {
            string[] data = currentText.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (commandHistory.Count == 0 || commandHistory[0] != currentText)
                commandHistory.Insert(0, currentText);
            drawCommands.Insert(0, new Line(currentText, Color.Aqua));
            currentText = "";
            seekIndex = -1;

            string[] args = new string[data.Length - 1];
            for (int i = 1; i < data.Length; i++)
                args[i - 1] = data[i];
            ExecuteCommand(data[0], args);
        }

        private void FindFirstTab()
        {
            for (int i = 0; i < sorted.Count; i++)
            {
                if (tabSearch == "" || sorted[i].IndexOf(tabSearch) == 0)
                {
                    tabIndex = i;
                    break;
                }
            }
        }

        private void FindLastTab()
        {
            for (int i = 0; i < sorted.Count; i++)
                if (tabSearch == "" || sorted[i].IndexOf(tabSearch) == 0)
                    tabIndex = i;
        }

        public void ExecuteFunctionKeyAction(int num)
        {
            FunctionKeyActions[num]?.Invoke();
        }

        public void ExecuteCommand(string command, string[] args)
        {
            if (commands.ContainsKey(command))
                commands[command].Action(args);
            else
                Log("Command '" + command + "' not found! Type 'help' for list of commands", Color.Yellow);
        }

        internal void Render()
        {
            if (Enabled)
            {
                int screenWidth = Engine.Width;
                int screenHeight = Engine.Height;

                RenderManager.SpriteBatch.Begin();

                RenderManager.Rect(10, screenHeight - 50, screenWidth - 20, 40, Color.Black * OPACITY);
                if (underscore)
                {
                    //RenderManager.SpriteBatch.DrawString(RenderManager.DefaultFont, ">" + currentText + "_", new Vector2(20, screenHeight - 42), Color.White);
                }
                else
                {
                    //RenderManager.SpriteBatch.DrawString(RenderManager.DefaultFont, ">" + currentText, new Vector2(20, screenHeight - 42), Color.White);
                }

                if (drawCommands.Count > 0)
                {
                    int height = 10 + (30 * drawCommands.Count);
                    RenderManager.Rect(10, screenHeight - height - 60, screenWidth - 20, height, Color.Black * OPACITY);
                    for (int i = 0; i < drawCommands.Count; i++)
                    {
                        //RenderManager.SpriteBatch.DrawString(RenderManager.DefaultFont, drawCommands[i].Text, new Vector2(20, screenHeight - 92 - (30 * i)), drawCommands[i].Color);
                    }
                }

                RenderManager.SpriteBatch.End();
            }
        }

        public void Log(object obj, Color color)
        {
            string str = obj.ToString();

            //Newline splits
            if (str.Contains("\n"))
            {
                var all = str.Split('\n');
                foreach (var line in all)
                    Log(line, color);
                return;
            }

            //Split the string if you overlow horizontally
            int maxWidth = Engine.Instance.Window.ClientBounds.Width - 40;
            //while (RenderManager.DefaultFont.MeasureString(str).X > maxWidth)
            //{
            //    int split = -1;
            //    for (int i = 0; i < str.Length; i++)
            //    {
            //        if (str[i] == ' ')
            //        {
            //            if (RenderManager.DefaultFont.MeasureString(str.Substring(0, i)).X <= maxWidth)
            //                split = i;
            //            else
            //                break;
            //        }
            //    }

            //    if (split == -1)
            //        break;

            //    drawCommands.Insert(0, new Line(str.Substring(0, split), color));
            //    str = str.Substring(split + 1);
            //}

            drawCommands.Insert(0, new Line(str, color));

            //Don't overflow top of window
            int maxCommands = (Engine.Instance.Window.ClientBounds.Height - 100) / 30;
            while (drawCommands.Count > maxCommands)
                drawCommands.RemoveAt(drawCommands.Count - 1);
        }

        public void Log(object obj)
        {
            Log(obj, Color.White);
        }

    }

    public class Command : Attribute
    {
        public string Name;
        public string Help;

        public Command(string name, string help)
        {
            Name = name;
            Help = help;
        }
    }
}
