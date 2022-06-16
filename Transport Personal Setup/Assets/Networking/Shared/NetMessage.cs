using Unity.Networking.Transport;

public abstract class NetMessage
{
    public uint ID;
    public MessageType Type;

    public abstract void Serialize(ref DataStreamWriter writer);

    public abstract void Deserialize(DataStreamReader reader);

    public virtual void ReceiveOnServer(BaseServer server)
    {

    }

    public virtual void ReceiveOnClient()
    {

    }
}
