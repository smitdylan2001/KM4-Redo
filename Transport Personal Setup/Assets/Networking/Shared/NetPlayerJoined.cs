using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetPlayerJoined : NetMessage
{
    public NetPlayerJoined()
    {
        Type = MessageType.PLAYER_JOIN;
    }

    public NetPlayerJoined(DataStreamReader reader)
    {
        Type = MessageType.PLAYER_JOIN;
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
    }

    public override void ReceiveOnClient()
    {

    }

    public override void ReceiveOnServer(BaseServer server)
    {

    }
}
