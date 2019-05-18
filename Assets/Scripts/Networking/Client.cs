using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client : MonoBehaviour, INetEventListener {

    public static Client Instance { get; set; }

    private NetManager client;

    void Awake() {
        Instance = this;
    }

    void Start() {
        client = new NetManager(this);
        client.UpdateTime = 15;
        client.Start();

        Debug.Log("Client has been started");
        Debug.Log("Client attempting connect");
        client.Connect("127.0.0.1", 9000, "sample_app");
    }

    void Update() {
        client.PollEvents();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
        int packetID = reader.GetInt();

        switch (packetID) {
            case 2:
                Packet02CreateWorldObject worldPacket = new Packet02CreateWorldObject();
                worldPacket.Deserialize(reader);
                worldPacket.ExecuteClientSide();
                break;
            case 3:
                Packet03CreatePlayer createPlayer = new Packet03CreatePlayer();
                createPlayer.Deserialize(reader);
                createPlayer.ExecuteClientSide();
                break;
            case 4:
                Packet04PlayerMove playerMove = new Packet04PlayerMove();
                playerMove.Deserialize(reader);
                playerMove.ExecuteClientSide();
                break;
        }
    }

    void OnDestroy() {
        if (client != null)
            client.Stop();
    }

    public void SendDataToServer(NetDataWriter writer, DeliveryMethod method) {
        client.SendToAll(writer, method);
    }

    public void OnPeerConnected(NetPeer peer) {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode) {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) {
        if (messageType == UnconnectedMessageType.BasicMessage && client.PeersCount == 0 && reader.GetInt() == 1) {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            client.Connect(remoteEndPoint, "sample_app");
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {

    }

    public void OnConnectionRequest(ConnectionRequest request) {

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }
}
