using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetOtherPlayerJoined : NetMessage
{
    public NetOtherPlayerJoined(uint id)
    {
        Type = MessageType.OTHER_PLAYER_JOIN;
        ID = id;
    }

    public NetOtherPlayerJoined(DataStreamReader reader)
    {
        Type = MessageType.OTHER_PLAYER_JOIN;
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
        BaseClient.otherPlayers.Add(ID);
        Debug.Log("Client:: " + ID);
    }

    public override void ReceiveOnServer(BaseServer server)
    {
        Debug.Log("Server:: " + ID);
        server.BroadcastMessage(this);
    }
}
