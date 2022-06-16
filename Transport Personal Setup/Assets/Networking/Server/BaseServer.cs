using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Collections.Generic;

public class BaseServer : MonoBehaviour
{
    public NetworkDriver Driver;
    protected NativeList<NetworkConnection> _connections;
    private Dictionary<NetworkConnection, NetworkedPlayer> _playerInstances = new Dictionary<NetworkConnection, NetworkedPlayer>();

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
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 5522;
        if(Driver.Bind(endpoint) != 0)
        {
            Debug.Log("There was an error binding to port " + endpoint.Port);
            return;
        }
        Driver.Listen();

        //Init driver list
        _connections = new NativeList<NetworkConnection>(4, Allocator.Persistent);

    }
    public virtual void UpdateServer()
    {
        Driver.ScheduleUpdate().Complete();
        CleanupConnections();
        AcceptConnections();
        UpdateMessagePump();    
    }
    public virtual void Shutdown()
    {
        Driver.Dispose();
        _connections.Dispose();
    }
    #endregion

    #region PrivateServerCalls
    private void CleanupConnections()
    {
        for (int i = 0; i < _connections.Length; i++)
        {
            if (!_connections[i].IsCreated)
            {
                _connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptConnections()
    {
        NetworkConnection c;
        while ((c = Driver.Accept()) != default(NetworkConnection))
        {
            _connections.Add(c);

            _playerInstances.Add(c, new NetworkedPlayer());

            foreach (NetworkConnection connection in _connections)
            {
                //if (connection == c) SendMessageToClient(connection, new NetPlayerJoined(c));
                //else SendMessageToClient(connection, new NetOtherPlayerJoined(c));
            }

            Debug.Log("Accepted a connection");
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i = 0; i < _connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = Driver.PopEventForConnection(_connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    _connections[i] = default(NetworkConnection);
                }
            }
        }
    }
    #endregion

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
            default:
                Debug.LogError("No MessageType found for " + type.ToString());
                break;
        }

        msg.ReceiveOnServer(this);
    }

    #region NetworkImplementation
    public virtual void BroadcastMessage(NetMessage msg)
    {
        foreach (NetworkConnection connection in _connections)
        {
            if (!connection.IsCreated) continue;

            SendMessageToClient(connection, msg);
        }
    }

    public virtual void SendMessageToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        Driver.BeginSend(connection, out writer);

        msg.Serialize(ref writer);

        Driver.EndSend(writer);
    }

    public void AddPlayer() { 
    }
    #endregion
}
