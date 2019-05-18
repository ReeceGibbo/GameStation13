using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Packet04PlayerMove : Packet, INetSerializable {

    public Guid guid;
    public Vector3 position;
    public Vector3 eulerAngles;

    public Packet04PlayerMove(Guid guid, Vector3 position, Vector3 eulerAngles) {
        packetID = 04;
        this.guid = guid;
        this.position = position;
        this.eulerAngles = eulerAngles;
    }

    public Packet04PlayerMove() {
        packetID = 04;
    }

    public void ExecuteClientSide() {
        ClientObjects.Instance.MovePlayer(this);
    }

    public void ExecuteServerSide(NetPeer thisPeer, NetDataWriter writer) {
        ServerObjects.Instance.MovePlayer(this, thisPeer, writer);
    }

    public void Serialize(NetDataWriter writer) {
        writer.Reset();
        writer.Put(packetID);
        writer.Put(guid);
        writer.Put(position);
        writer.Put(eulerAngles);
    }

    public void Deserialize(NetDataReader reader) {
        guid = reader.GetGuid();
        position = reader.GetVector3();
        eulerAngles = reader.GetVector3();
    }
}
