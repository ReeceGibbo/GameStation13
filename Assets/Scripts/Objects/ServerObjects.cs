using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ServerObjects : MonoBehaviour {

    public static ServerObjects Instance { get; set; }

    [Header("Player Prefabs")]
    public GameObject serverPlayer;

    [Header("World Parent")]
    public GameObject worldParent;

    [Header("Spawn Point")]
    public GameObject spawnPoint;

    // Objects that are in the world
    private Dictionary<int, GameObject> spawnedObjects = new Dictionary<int, GameObject>();
    public List<WorldObject> prefabObjects = new List<WorldObject>();

    // Players that are in the world
    private Dictionary<Guid, Player> clientPlayers = new Dictionary<Guid, Player>();

    // Used for getting children - SERVER STUFF
    private List<GameObject> listOfChildren = new List<GameObject>();

    void Awake() {
        Instance = this;
    }

    public void SetupServerMap() {
        // The server already has items in the scene - store thems
        GetChildRecursive(worldParent);

        for (int x = 0; x < listOfChildren.Count; x++) {
            spawnedObjects.Add(listOfChildren[x].GetInstanceID(), listOfChildren[x]);
        }
    }

    /**
     *  SENDING OBJECTS TO CLIENT, SPAWNING OBJECTS
     **/
    public void SendObjectsToClient(NetPeer peer, NetDataWriter writer) {
        StartCoroutine(SendObjectsToClient_Coroutine(peer, writer));
    }

    private IEnumerator SendObjectsToClient_Coroutine(NetPeer peer, NetDataWriter writer) {
        Dictionary<int, GameObject>.Enumerator enumerator = spawnedObjects.GetEnumerator();

        while (enumerator.MoveNext()) {
            int instanceID = enumerator.Current.Key;
            GameObject obj = enumerator.Current.Value;
            int maxAmount = spawnedObjects.Count;

            WorldObject objWorld = obj.GetComponent<WorldObject>();

            if (obj != null && objWorld != null) {
                Packet02CreateWorldObject createObject = new Packet02CreateWorldObject(instanceID, objWorld.UNIQUE_ID, obj.transform.position, obj.transform.eulerAngles, maxAmount);
                // Create and send data
                createObject.Serialize(writer);
                peer.Send(writer, DeliveryMethod.ReliableUnordered);
            }

            yield return null;
        }

        enumerator.Dispose();
        yield return 0;
    }

    /**
     *  SPAWNING AND SENDING PLAYERS
     **/

    public void SendPlayersToClient(NetPeer peer, NetDataWriter writer) {
        Dictionary<Guid, Player> currentPlayers = clientPlayers;

        foreach (Player player in currentPlayers.Values) {
            Packet03CreatePlayer createPlayer = new Packet03CreatePlayer(player.guid, player.gameObject.transform.position, player.gameObject.transform.eulerAngles, false);
            createPlayer.Serialize(writer);
            peer.Send(writer, DeliveryMethod.ReliableUnordered);
        }
    }

    public void AddPlayer(Packet03CreatePlayer packet, NetPeer thisPeer, NetDataWriter writer) {
        GameObject newPlayer = Instantiate(serverPlayer, packet.position, Quaternion.Euler(packet.eulerAngles), worldParent.transform);
        Player player = newPlayer.GetComponent<Player>();
        player.guid = packet.guid;
        player.ipAddress = thisPeer.EndPoint.Address.ToString();
        player.port = thisPeer.EndPoint.Port;

        clientPlayers.Add(player.guid, player);
        Debug.Log("New player spawned.. " + clientPlayers.Count + " players in server.");

        // If this is the server - we should send this new player to our connected clients - apart from our new one
        foreach (NetPeer peer in Server.Instance.connectedPeers) {
            if (peer != thisPeer) {
                Packet03CreatePlayer createPlayer = packet;
                createPlayer.ourPlayer = false;
                createPlayer.Serialize(writer);
                peer.Send(writer, DeliveryMethod.ReliableUnordered);
            }
        }
    }

    public void MovePlayer(Packet04PlayerMove packet, NetPeer thisPeer, NetDataWriter writer) {
        Player player = clientPlayers[packet.guid];

        player.transform.position = packet.position;
        player.transform.eulerAngles = packet.eulerAngles;

        // Send this player's movement to all connected clients
        foreach (NetPeer peer in Server.Instance.connectedPeers) {
            if (peer != thisPeer) {
                Packet04PlayerMove playerMove = packet;
                playerMove.Serialize(writer);
                peer.Send(writer, DeliveryMethod.Unreliable);
            }
        }
    }

    public void RemovePlayer() {

    }

    private void GetChildRecursive(GameObject obj) {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform) {
            if (null == child)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            listOfChildren.Add(child.gameObject);
            GetChildRecursive(child.gameObject);
        }
    }
}
