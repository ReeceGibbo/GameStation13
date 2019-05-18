using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class CommandDecider : MonoBehaviour {

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

    bool isServer = false;

    void Awake() {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++) {
            Debug.Log("ARG " + i + ": " + args[i]);
            if (args[i] == "-server") {
                isServer = true;
            }
        }

        if (!isServer) {
            SceneManager.LoadScene("Client", LoadSceneMode.Single);
            Destroy(gameObject);
            return;
        } else {
            SceneManager.LoadScene("Server", LoadSceneMode.Single);
        }
    }

#endif
}
