using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Player : MonoBehaviour {

    // Unique ID
    public Guid guid;

    // Serversided only - person who owns this object
    public string ipAddress;
    public int port;

    // Movement coroutine for clients
    private Coroutine coroutine = null;

    public void MovePlayer(Vector3 position, Vector3 eulerAngles) {
        if (coroutine == null) {
            coroutine = StartCoroutine(MoveToNewPosition(position, eulerAngles));
        } else {
            StopCoroutine(coroutine);
            coroutine = StartCoroutine(MoveToNewPosition(position, eulerAngles));
        }
    }

    private IEnumerator MoveToNewPosition(Vector3 position, Vector3 eulerAngles) {
        while (true) {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(eulerAngles), Time.deltaTime * 10);

            yield return new WaitForEndOfFrame();
        }
    }

}
