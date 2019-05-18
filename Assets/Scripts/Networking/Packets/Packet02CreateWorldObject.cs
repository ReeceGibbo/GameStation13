using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Packet02CreateWorldObject : Packet, INetSerializable {

    public int instanceID;
    public string UNIQUE_ID;
    public Vector3 position;
    public Vector3 eulerAngles;

    public Packet02CreateWorldObject(int instanceID, string UNIQUE_ID, Vector3 position, Vector3 eulerAngles, int maxObjects) {
        packetID = 02;
        this.instanceID = instanceID;
        this.UNIQUE_ID = UNIQUE_ID;
        this.position = position;
        this.eulerAngles = eulerAngles;
    }

    public Packet02CreateWorldObject() {
        packetID = 02;
    }

    public void ExecuteClientSide() {
        ClientObjects.Instance.SpawnObject(instanceID, UNIQUE_ID, position, eulerAngles);
    }

    public void ExecuteServerSide() {
        
    }

    public void Serialize(NetDataWriter writer) {
        writer.Reset();
        writer.Put(packetID);
        writer.Put(instanceID);
        writer.Put(UNIQUE_ID);
        writer.Put(position);
        writer.Put(eulerAngles);
    }

    public void Deserialize(NetDataReader reader) {
        instanceID = reader.GetInt();
        UNIQUE_ID = reader.GetString();
        position = reader.GetVector3();
        eulerAngles = reader.GetVector3();
    }
}
