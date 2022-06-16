using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Collections.Generic;

public class BaseClient : MonoBehaviour
{
    public NetworkDriver Driver;
    protected NetworkConnection _connection;

    public static uint id;
    public static List<uint> otherPlayers = new List<uint>();

#if UNITY_EDITOR
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        UpdateServer();
    }

    private void OnDestroy()
    {
        Shutdown();
    }
#endif

    #region VirtualServerCalls
    public virtual void Init()
    {
        //Init driver
        Driver = NetworkDriver.Create();
        _connection = default(NetworkConnection);
        NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4;

        endpoint.Port = 5522;
        _connection = Driver.Connect(endpoint);

    }
    public virtual void UpdateServer()
    {
        Driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();
    }
    public virtual void Shutdown()
    {
        Driver.Dispose();
    }
    #endregion

    #region PrivateServerCalls
    private void CheckAlive()
    {
        if (!_connection.IsCreated)
        {
            Debug.LogError("Connection lost to server");
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;

        NetworkEvent.Type cmd;
        while ((cmd = _connection.PopEvent(Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                _connection = default(NetworkConnection);
            }
        }
    }

    public virtual void OnData(DataStreamReader stream)
    {
        NetMessage msg = null;
        MessageType type = (MessageType)stream.ReadByte();

        switch (type)
        {
            case (MessageType.CHAT_MESSAGE):
                msg = new NetChatMessage(stream);
                break;
            case (MessageType.BUTTON_PRESSED):
                msg = new NetButtonPressed(stream);
                break;
            case (MessageType.NEW_BUTTON):
                msg = new NetSelectNewButton(stream);
                break;
            case (MessageType.PLAYER_JOIN):
                msg = new NetPlayerJoined(stream);
                break;
            case (MessageType.OTHER_PLAYER_JOIN):
                msg = new NetOtherPlayerJoined(stream);
                break;
            default:
                Debug.LogError("No MessageType found for " + type.ToString());
                break;
        }

        msg.ReceiveOnClient();
    }
    #endregion

    #region NetworkImplementation
    public virtual void SendMessageToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        Driver.BeginSend(_connection, out writer);

        msg.Serialize(ref writer);

        Driver.EndSend(writer);
    }
    #endregion
}
