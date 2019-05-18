using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Packet03CreatePlayer : Packet, INetSerializable {

    public Guid guid;
    public Vector3 position;
    public Vector3 eulerAngles;
    public bool ourPlayer;

    public Packet03CreatePlayer(Guid guid, Vector3 position, Vector3 eulerAngles, bool ourPlayer) {
        packetID = 03;
        this.guid = guid;
        this.position = position;
        this.eulerAngles = eulerAngles;
        this.ourPlayer = ourPlayer;
    }

    public Packet03CreatePlayer() {
        packetID = 03;
    }

    public void ExecuteClientSide() {
        ClientObjects.Instance.AddPlayer(this);
    }

    public void ExecuteServerSide(NetPeer thisPeer, NetDataWriter writer) {
        // We should probably send this "NEW" player to our connected clients too!
        ServerObjects.Instance.AddPlayer(this, thisPeer, writer);
    }

    public void Serialize(NetDataWriter writer) {
        writer.Reset();
        writer.Put(packetID);
        writer.Put(guid);
        writer.Put(position);
        writer.Put(eulerAngles);
        writer.Put(ourPlayer);
    }

    public void Deserialize(NetDataReader reader) {
        guid = reader.GetGuid();
        position = reader.GetVector3();
        eulerAngles = reader.GetVector3();
        ourPlayer = reader.GetBool();
    }
}
