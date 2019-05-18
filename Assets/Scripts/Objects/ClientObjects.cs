using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ClientObjects : MonoBehaviour {

    public static ClientObjects Instance { get; set; }

    [Header("Player Prefabs")]
    public GameObject clientPlayer;
    public GameObject serverPlayer;

    [Header("World Parent")]
    public GameObject worldParent;

    // Objects that are in the world
    private Dictionary<int, GameObject> spawnedObjects = new Dictionary<int, GameObject>();
    public List<WorldObject> prefabObjects = new List<WorldObject>();

    // Players that are in the world
    private Dictionary<Guid, Player> clientPlayers = new Dictionary<Guid, Player>();

    void Awake() {
        Instance = this;
    }

    /**
     *  SPAWNING OBJECTS RECEIVED BY THE SERVER
     **/

    public void SpawnObject(int instanceID, string UNIQUE_ID, Vector3 position, Vector3 eulerAngels) {
        // Make sure we don't already have the object
        if (spawnedObjects.ContainsKey(instanceID))
            return;

        for (int x = 0; x < prefabObjects.Count; x++) {
            if (prefabObjects[x].UNIQUE_ID == UNIQUE_ID) {
                // Instantiate this one
                spawnedObjects.Add(instanceID, Instantiate(prefabObjects[x].clientPrefab, position, Quaternion.Euler(eulerAngels), worldParent.transform));
            }
        }
    }

    /**
     *  SPAWNING AND MOVING PLAYERS
     **/

    public void AddPlayer(Packet03CreatePlayer packet) {
        if (packet.ourPlayer) {
            GameObject newPlayer = Instantiate(clientPlayer, packet.position, Quaternion.Euler(packet.eulerAngles), worldParent.transform);
            Player player = newPlayer.GetComponent<Player>();
            player.guid = packet.guid;

            clientPlayers.Add(packet.guid, player);
            Debug.Log("New player spawned.. " + clientPlayers.Count + " players in server.");
        }
        else {
            GameObject newPlayer = Instantiate(serverPlayer, packet.position, Quaternion.Euler(packet.eulerAngles), worldParent.transform);
            Player player = newPlayer.GetComponent<Player>();
            player.guid = packet.guid;

            clientPlayers.Add(packet.guid, player);
            Debug.Log("New player spawned.. " + clientPlayers.Count + " players in server.");
        }
    }

    public void MovePlayer(Packet04PlayerMove packet) {
        Player player = clientPlayers[packet.guid];

        player.MovePlayer(packet.position, packet.eulerAngles);

        //player.transform.position = packet.position;
        //player.transform.eulerAngles = packet.eulerAngles;
    }

    public void RemovePlayer() {

    }
}