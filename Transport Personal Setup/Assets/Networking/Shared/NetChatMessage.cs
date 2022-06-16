using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetChatMessage : NetMessage
{
    public FixedString128Bytes Message { get; set; }

    public NetChatMessage(string message)
    {
        Type = MessageType.CHAT_MESSAGE;
        Message = message;
    }

    public NetChatMessage(DataStreamReader reader)
    {
        Type = MessageType.CHAT_MESSAGE;
        Deserialize(reader);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        //First byte already handled in server class

        Message = reader.ReadFixedString128();
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Type);
        writer.WriteFixedString128(Message);
    }

    public override void ReceiveOnClient()
    {
        base.ReceiveOnClient();
        Debug.Log("Server:: " + Message);
    }

    public override void ReceiveOnServer(BaseServer server)
    {
        Debug.Log("Server:: " + Message);
    }
}
