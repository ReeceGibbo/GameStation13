using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class ServerConsole : MonoBehaviour {

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

    Windows.ConsoleInput input = new Windows.ConsoleInput();
    Windows.ConsoleWindow console = new Windows.ConsoleWindow();

    string strInput;

    void OnEnable() {
        DontDestroyOnLoad(gameObject);

        console.Initialize();
        console.SetTitle("Game Station 13 Server");

        input.OnInputText += OnInputText;

        Application.RegisterLogCallback(HandleLog);

        Debug.Log("Console Started");
    }

    void OnInputText(string obj) {
        //ConsoleSystem.Run(obj, true);
        if (obj == "stop") {
            Application.Quit();
        }
    }

    void HandleLog(string message, string stackTrace, LogType type) {
        if (type == LogType.Warning)
            System.Console.ForegroundColor = ConsoleColor.Yellow;
        else if (type == LogType.Error)
            System.Console.ForegroundColor = ConsoleColor.Red;
        else
            System.Console.ForegroundColor = ConsoleColor.White;

        // We're half way through typing something, so clear this line ..
        if (Console.CursorLeft != 0)
            input.ClearLine();

        System.Console.WriteLine(message);

        // If we were typing something re-add it.
        input.RedrawInputLine();
    }

    void Update() {
        if (input != null)
            input.Update();
    }

    void OnDestroy() {
        if (console != null)
            console.Shutdown();
    }

#endif
}
