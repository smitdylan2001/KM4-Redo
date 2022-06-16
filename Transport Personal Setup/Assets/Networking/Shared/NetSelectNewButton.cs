using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetSelectNewButton : NetMessage
{
    public float X, Y;

    public NetSelectNewButton(uint id, float x, float y)
    {
        Type = MessageType.BUTTON_PRESSED;
        X = x;
        Y = y;
    }

    public NetSelectNewButton(DataStreamReader reader)
    {
        Type = MessageType.BUTTON_PRESSED;
        Deserialize(reader);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already handled in server class

        X = reader.ReadFloat();
        Y = reader.ReadFloat();
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Type);
        writer.WriteFloat(X);
        writer.WriteFloat(Y);
    }

    public override void ReceiveOnClient()
    {
        base.ReceiveOnClient();
        Debug.Log("Client:: " + X + " :: " + Y);
    }

    public override void ReceiveOnServer(BaseServer server)
    {
        Debug.Log("Server:: " + X + " :: " + Y);
        server.BroadcastMessage(this);
    }
}
