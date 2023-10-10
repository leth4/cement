using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Console : MonoBehaviour
{
    private static List<Command> _commands = new();

    private string _command = "";
    private List<Log> _logs = new();

    private bool _isActive = false;
    private Vector2 _scrollPosition;

    public static void AddCommand(string prefix, string description, Action callback)
    {
        AddCommand(new Command()
        {
            Prefix = prefix,
            Description = description,
            Types = new Type[] { },
            Callback = _ => callback()
        });
    }

    public static void AddCommand<T1>(string prefix, string description, Action<T1> callback)
    {
        AddCommand(new Command()
        {
            Prefix = prefix,
            Description = description,
            Callback = o => callback((T1)o[0]),
            Types = new Type[] { typeof(T1) }
        });
    }

    public static void AddCommand<T1, T2>(string prefix, string description, Action<T1, T2> callback)
    {
        AddCommand(new Command()
        {
            Prefix = prefix,
            Description = description,
            Callback = o => callback((T1)o[0], (T2)o[1]),
            Types = new Type[] { typeof(T1), typeof(T2) }
        });
    }

    private static void AddCommand(Command command)
    {
        for (int i = 0; i < _commands.Count; i++)
        {
            if (_commands[i].Prefix == command.Prefix)
            {
                _commands[i] = command;
                return;
            }
        }
        _commands.Add(command);
    }

    private void OnEnable()
    {
        CreateDefaultCommands();
        Application.logMessageReceived += ReceiveUnityLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= ReceiveUnityLog;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            _isActive = !_isActive;
        }
    }

    private void HandleCommand()
    {
        if (_command == "") return;

        string[] words = _command.Split(" ");
        string prefix = words[0];
        string[] args = words.Skip(1).ToArray();
        _command = "";

        foreach (var command in _commands)
        {
            if (command.Prefix != prefix) continue;

            var invoked = Invoke(command, args);
            if (invoked) return;

            PushLog(command.Types.Length switch
            {
                1 => $"Command {command.Prefix} takes a [{command.Types[0].Name}] argument",
                2 => $"Command {command.Prefix} takes [{command.Types[0].Name}] and [{command.Types[1].Name}] arguments",
                _ => $"Command {command.Prefix} takes no arguments",
            }, LogType.Error);

            return;
        }

        PushLog($"Command '{prefix}' does not exist!", LogType.Error);
    }

    private bool Invoke(Command command, string[] args)
    {
        if (args.Length != command.Types.Length) return false;
        var objectArgs = new object[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            try
            {
                if (command.Types[i] == typeof(int)) objectArgs[i] = Int32.Parse(args[i]);
                else if (command.Types[i] == typeof(float)) objectArgs[i] = float.Parse(args[i]);
                else if (command.Types[i] == typeof(bool)) objectArgs[i] = float.Parse(args[i]);
                else if (command.Types[i] == typeof(string)) objectArgs[i] = args[i];
                else return false;
            }
            catch { return false; }
        }
        command.Callback.Invoke(objectArgs);
        return true;
    }

    private void OnGUI()
    {
        if (!_isActive) return;

        if (Event.current.keyCode == KeyCode.Escape) _isActive = false;
        if (Event.current.keyCode == KeyCode.Return)
        {
            HandleCommand();
            _scrollPosition.y = 10000;
        }

        var margin = 20f;
        var width = (Screen.width) - (margin * 2);
        var height = (Screen.height) - (margin * 2);
        var windowRect = new Rect(margin, margin, width, height);

        var texture = new Texture2D(1, 1);
        texture.SetPixels(new Color[] { new Color(0, 0, 0, 0.85f) });
        texture.Apply();

        var skin = GUI.skin.window;
        skin.normal.background = texture;

        GUILayout.BeginArea(windowRect, skin);
        DrawWindow();
        GUILayout.EndArea();
    }

    private void DrawWindow()
    {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUIStyle.none, GUIStyle.none);
        GUILayout.FlexibleSpace();
        foreach (var log in _logs)
        {
            GUI.color = log.Color;
            GUILayout.Label(log.Text);
        }
        GUILayout.EndScrollView();

        GUI.color = Color.white;
        GUI.SetNextControlName("CommandInput");
        _command = GUILayout.TextField(_command);
        GUI.FocusControl("CommandInput");
    }

    private void ReceiveUnityLog(string logString, string stackTrace, UnityEngine.LogType type)
    {
        PushLog(logString, type == UnityEngine.LogType.Error ? LogType.Error : LogType.Log);
    }

    private void PushLog(string text, LogType type = LogType.Message)
    {
        _logs.Add(new Log(text, type));
    }

    private void CreateDefaultCommands()
    {
        AddCommand<string>(
            prefix: "log",
            description: "Displays text in the console.",
            callback: t => Debug.Log(t)
        );

        AddCommand(
            prefix: "help",
            description: "Shows a list of commands",
            callback: () =>
            {
                string text = "";
                foreach (var command in _commands)
                {
                    var types = command.Types.Length switch
                    {
                        1 => $" [{command.Types[0].Name}]",
                        2 => $" [{command.Types[0].Name}] [{command.Types[1].Name}]",
                        _ => ""
                    };
                    text += $"{command.Prefix}{types} — {command.Description}\n";
                }
                PushLog(text.Substring(0, text.Length - 1));
            }
        );

        AddCommand(
            prefix: "clear",
            description: "Clears logs",
            callback: () => _logs = new()
        );

        AddCommand<float>(
            prefix: "timescale",
            description: "Changes the game's speed. Default is 1",
            callback: t =>
            {
                Time.timeScale = t;
                PushLog($"Timescale set to {t}");
            }
        );
    }

    private struct Command
    {
        public string Prefix;
        public string Description;
        public Action<object[]> Callback;
        public Type[] Types;
    }

    private struct Log
    {
        public string Text { get; private set; }
        public LogType Type { get; private set; }
        public Color Color { get; private set; }

        public Log(string text, LogType type)
        {
            Text = text;
            Type = type;

            Color = type switch
            {
                LogType.Message => Color.white,
                LogType.Log => new Color(0.7f, 0.7f, 0.7f),
                LogType.Error => new Color(1, 0.4f, 0.4f),
                _ => Color = Color.white
            };
        }
    }

    private enum LogType
    {
        Message,
        Log,
        Error
    }
}