using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Packet00Connect : Packet {

    public string username;
    public string password;

    public Packet00Connect(string username, string password) {
        packetID = 00;
        this.username = username;
        this.password = password;
    }

    public Packet00Connect(NetDataReader reader) {
        this.username = reader.GetString();
        this.password = reader.GetString();
    }

    public void ExecuteClientSide() {

    }

    public void ExecuteServerSide() {
        
    }

    public NetDataWriter ToWriter(NetDataWriter writer) {
        writer.Put(packetID);
        writer.Put(username);
        writer.Put(password);
        return writer;
    }

}
