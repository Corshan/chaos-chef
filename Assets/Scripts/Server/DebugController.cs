using System;
using System.Collections;
using System.Collections.Generic;
using LobbySystem;
using UnityEngine;
using UnityEngine.InputSystem;

struct DebugLog
{
    public float time;
    public string condition;
    public string stackTrace;
    public LogType type;
    public DebugLog(string condition, string stackTrace, LogType type, float time)
    {
        this.condition = condition;
        this.stackTrace = stackTrace;
        this.type = type;
        this.time = time;
    }
}



public class DebugController : MonoBehaviour
{
    string _input;
    bool showHelp = false;
    bool showDebug = true;
    Vector2 helpScroll;
    Vector2 debugScroll;
    int prevLogCount = 0;
    public static DebugCommand CREATE_SERVER;
    public static DebugCommand START_SHIFT;
    public static DebugCommand END_SHIFT;
    public static DebugCommand STOP_SERVER;
    public static DebugCommand RESTART_SERVER;
    public static DebugCommand HELP;
    public static DebugCommand DEBUG;
    public static DebugCommand MUTE;
    public static DebugCommand<int> SET_TIMER;
    public static DebugCommand<int> SET_CASH_EARNED;
    public static DebugCommand<int> SET_CASH_AMOUNT;
    public static DebugCommand<int> SET_MUSIC_VOLUME;
    public static DebugCommand<int> SET_SFX_VOLUME;

    public List<object> commandList;
    private List<DebugLog> _logs = new();

    private void Awake()
    {
        CREATE_SERVER = new DebugCommand("create_server", "Starts the game server.", "create_server", () =>
        {
            LobbyManager.Singleton.CreateServer();
        });

        STOP_SERVER = new DebugCommand("stop_server", "Destroys the game server", "stop_server", () =>
        {
            LobbyManager.Singleton.DestroyServer();
        });

        RESTART_SERVER = new DebugCommand("restart_server", "Restarts game server", "restart_server", () =>
        {
            LobbyManager.Singleton.RestartServer();
        });

        SET_TIMER = new DebugCommand<int>("set_timer", "Sets how long each round lasts", "set_timer <timer_amount>", (value) =>
        {
            GameManager.Singleton.SetTimer((float)value);
        });

        SET_CASH_EARNED = new DebugCommand<int>("set_cash_earned", "Set the total amount of cashed earned", "set_cash_earned <cash_amount>", (value) =>
        {
            GameManager.Singleton.SetCashEarned(value);
        });

        SET_CASH_AMOUNT = new DebugCommand<int>("set_cash_amount", "Set the amount eanred per order", "set_cash_amount <cash_amount>", (value) =>
        {
            GameManager.Singleton.SetCashAmount(value);
        });

        SET_MUSIC_VOLUME = new DebugCommand<int>("set_music_volume", "Set the volume of music", "set_music_volume <volume_amount>", (value) =>
        {
            AudioManager.Singleton.ChangeMusicVolume(value);
        });

        SET_SFX_VOLUME = new DebugCommand<int>("set_sfx_volume", "Set the volume of sound effects", "set_sfx_volume <volume_amount>", (value) =>
        {
            AudioManager.Singleton.ChangeSFXVolume(value);
        });

        HELP = new DebugCommand("help", "Shows all commands", "help", () =>
        {
            showHelp = !showHelp;
        });

        DEBUG = new DebugCommand("debug", "Shows debug window", "debug", () =>
        {
            showDebug = !showDebug;
        });

        MUTE = new DebugCommand("mute", "Mute all audio", "mute", () =>
        {
            AudioManager.Singleton.ChangeMusicVolume(-80);
            AudioManager.Singleton.ChangeSFXVolume(-80);
        });

        START_SHIFT = new DebugCommand("start_shift", "Starts the round", "start_shift", () =>
        {
            GameManager.Singleton.StartShift();
        });

        END_SHIFT = new DebugCommand("end_shift", "End the round", "end_shift", () =>
        {
            GameManager.Singleton.EndShift();
        });


        commandList = new List<object> {
            CREATE_SERVER,
            STOP_SERVER,
            RESTART_SERVER,
            SET_TIMER,
            SET_MUSIC_VOLUME,
            SET_SFX_VOLUME,
            HELP,
            DEBUG,
            MUTE,
            SET_CASH_AMOUNT,
            SET_CASH_EARNED,
            START_SHIFT,
            END_SHIFT,
            new DebugCommand("","","", ()=>{}),
        };

        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        _logs.Add(new DebugLog(condition, stackTrace, type, Time.time));
    }

    private void OnGUI()
    {
        float y = 0f;

        if (showHelp)
        {
            y = ShowHelp(Screen.width, Screen.height * 0.05f, y, 3);
        }

        if (showDebug) ShowDebug(0, Screen.height, Screen.width, Screen.height * 0.05f, 4);

        TextField(Screen.width, Screen.height * 0.05f, y);
    }

    private void HandleInput()
    {
        string[] properties = _input.Split(" ");

        for (int i = 0; i < commandList.Count; i++)
        {
            var commandbase = commandList[i] as DebugCommandBase;

            if (_input.Contains(commandbase.CommandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null)
                {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }

    public void OnReturn(InputValue value)
    {
        HandleInput();
        _input = "";
    }

    private float ShowHelp(float width, float height, float y, float mul)
    {
        GUI.skin.label.fontSize = (int)(height * 0.5f);
        GUI.Box(new Rect(0, y, width, (height * mul) + (height * 0.3f)), "");

        Rect viewport = new Rect(0, 0, width - 30, GUI.skin.label.fontSize * commandList.Count);

        helpScroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, (height * mul)), helpScroll, viewport);

        for (int i = 0; i < commandList.Count; i++)
        {
            var command = commandList[i] as DebugCommandBase;
            string label = $"{command.CommandFormat} - {command.CommandDescription}";

            Rect labelRect = new Rect(5, GUI.skin.label.fontSize * i, viewport.width - 100, GUI.skin.label.fontSize + (GUI.skin.label.fontSize * 0.6f));

            GUI.Label(labelRect, label);
        }

        GUI.EndScrollView();
        return y += (height * mul) + (height * 0.2f);
    }

    private void ShowDebug(float x, float y, float width, float height, float mul)
    {
        GUI.skin.label.fontSize = (int)(height / 2);
        GUI.Box(new Rect(x, y - (height * mul), width, (height * mul) - (height * 0.2f)), "");

        Rect viewport = new Rect(x, y - (height * mul), width - 30, GUI.skin.label.fontSize * _logs.Count);

        debugScroll = GUI.BeginScrollView(new Rect(0, y - (height * mul), Screen.width, (height * mul) - (height * 0.2f)), debugScroll, viewport);

        _logs.Sort(SortTime);

        for (int i = 0; i < _logs.Count; i++)
        {
            var log = _logs[i];

            if (log.type == LogType.Warning) continue;

            string label = $"{log.condition}";

            Rect labelRect = new Rect(width * 0.01f, (y - (height * mul)) + (GUI.skin.label.fontSize * i), viewport.width - 100, GUI.skin.label.fontSize + (GUI.skin.label.fontSize * 0.5f));

            GUI.Label(labelRect, label);
        }

        if (prevLogCount != _logs.Count)
        {
            debugScroll = new Vector2(0, 0);
            prevLogCount = _logs.Count;
        }

        GUI.EndScrollView();
    }

    private int SortTime(DebugLog x, DebugLog y)
    {
        if (x.time - y.time == 0) return 0;
        else if (x.time - y.time < 0) return 1;
        else return -1;
    }

    private void TextField(float width, float height, float y)
    {
        GUI.skin.textField.fontSize = (int)(height * 0.5f);
        GUI.Box(new Rect(0, y, width, height), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        _input = GUI.TextField(new Rect(10f, y + 5f, width - 20f, height), _input);
    }
}
