using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetReadyMessage : NetMessage
{
    public NetReadyMessage(uint id)
    {
        Type = MessageType.PLAYER_READY;
        ID = id;
    }

    public NetReadyMessage(DataStreamReader reader)
    {
        Type = MessageType.PLAYER_READY;
        Deserialize(reader);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already handled in server class
        ID = reader.ReadUInt();
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Type);
        writer.WriteUInt(ID);
    }

    public override void ReceiveOnClient()
    {
        Debug.Log("Client:: " + ID);
    }

    public override void ReceiveOnServer(BaseServer server)
    {
        Debug.Log("Server:: " + ID);
    }
}
