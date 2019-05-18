using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Server : MonoBehaviour, INetEventListener, INetLogger {

    public static Server Instance { get; set; }

    private NetManager server;
    private NetDataWriter dataWriter;

    public List<NetPeer> connectedPeers = new List<NetPeer>();

    void Awake() {
        Instance = this;
    }

    void Start() {
        NetDebug.Logger = this;
        dataWriter = new NetDataWriter();

        server = new NetManager(this);
        server.UpdateTime = 15;
        server.Start(9000);

        Debug.Log("Server has been started");

        ServerObjects.Instance.SetupServerMap();
        Debug.Log("Server map setup complete");
    }

    void Update() {
        server.PollEvents();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
        int packetID = reader.GetInt();

        switch (packetID) {
            case 0:
                Packet00Connect connect = new Packet00Connect(reader);
                Debug.Log(connect.username + " | " + connect.password);
                break;
            case 4:
                Packet04PlayerMove playerMove = new Packet04PlayerMove();
                playerMove.Deserialize(reader);
                playerMove.ExecuteServerSide(peer, dataWriter);
                break;
        }
    }

    void OnDestroy() {
        NetDebug.Logger = null;
        if (server != null)
            server.Stop();
    }

    public void OnPeerConnected(NetPeer peer) {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
        connectedPeers.Add(peer);

        NetDataWriter peerWriter = new NetDataWriter();

        // Send all objects to client
        ServerObjects.Instance.SendObjectsToClient(peer, peerWriter);

        // Then send them all players that are already in the game
        ServerObjects.Instance.SendPlayersToClient(peer, peerWriter);

        // Then create OUR player and send them our player
        Packet03CreatePlayer newClientPlayer = new Packet03CreatePlayer(Guid.NewGuid(), ServerObjects.Instance.spawnPoint.transform.position, Vector3.zero, true);
        newClientPlayer.Serialize(peerWriter);
        peer.Send(peerWriter, DeliveryMethod.ReliableUnordered);

        // Send this new player to server clients as well
        newClientPlayer.ExecuteServerSide(peer, peerWriter);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode) {
        Debug.Log("[SERVER] error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType) {
        if (messageType == UnconnectedMessageType.Broadcast) {
            Debug.Log("[SERVER] Received discovery request. Send discovery response");
            NetDataWriter resp = new NetDataWriter();
            resp.Put(1);
            server.SendUnconnectedMessage(resp, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
    }

    public void OnConnectionRequest(ConnectionRequest request) {
        request.AcceptIfKey("sample_app");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        connectedPeers.Remove(peer);
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args) {
        Debug.LogFormat(str, args);
    }
}
