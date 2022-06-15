using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Networking.Transport.Utilities;
using System.Linq;
using UnityEngine.UI;

namespace UnityMultiplayerGame {

    public delegate void ServerMessageHandler(Server server, NetworkConnection con, MessageHeader header);
    public delegate void ClientMessageHandler(Client client, MessageHeader header);

    public enum NetworkMessageType
    {
        HANDSHAKE,
        HANDSHAKE_RESPONSE,
        CHAT_MESSAGE,
        CHAT_QUIT,
        NETWORK_SPAWN,
        NETWORK_DESTROY,
        PING,
        PONG, 
        READY,
        OTHER_READY,
        START,
        START_CONFIRM,
        BUTTON_SELECTED,
        BUTTON_REQUEST,
        SCORE,
    }

    public enum MessageType
	{
        MESSAGE, 
        JOIN,
        QUIT
	}

    public class PingPong
	{
        public float lastSendTime = 0;
        public int status = -1;
        public string name = ""; // because of weird issues...
	}

    public static class NetworkMessageInfo
	{
        public static Dictionary<NetworkMessageType, System.Type> TypeMap = new Dictionary<NetworkMessageType, System.Type> {
            { NetworkMessageType.HANDSHAKE,                 typeof(HandshakeMessage) },
            { NetworkMessageType.HANDSHAKE_RESPONSE,        typeof(HandshakeResponseMessage) },
            { NetworkMessageType.CHAT_MESSAGE,              typeof(ChatMessage) },
            { NetworkMessageType.CHAT_QUIT,                 typeof(ChatQuitMessage) },
            { NetworkMessageType.NETWORK_SPAWN,             typeof(SpawnMessage) },
            { NetworkMessageType.NETWORK_DESTROY,           typeof(DestroyMessage) },
            { NetworkMessageType.PING,                      typeof(PingMessage) },
            { NetworkMessageType.PONG,                      typeof(PongMessage) },
            { NetworkMessageType.READY,                     typeof(ReadyMessage) },
            { NetworkMessageType.START,                     typeof(StartGameMessage) },
            {NetworkMessageType.START_CONFIRM,              typeof(StartConfirmMessage) },
            { NetworkMessageType.BUTTON_SELECTED,           typeof(ButtonPressedMessage) },
            { NetworkMessageType.BUTTON_REQUEST,            typeof(ButtonRequestMessage) },
            {NetworkMessageType.SCORE, typeof(ScoreMessage) }
        };
    }

    public class Server : MonoBehaviour
    {
        static readonly Dictionary<NetworkMessageType, ServerMessageHandler> NetworkMessageHandlers = new Dictionary<NetworkMessageType, ServerMessageHandler> {
            { NetworkMessageType.HANDSHAKE,         HandleClientHandshake },
            { NetworkMessageType.CHAT_MESSAGE,      HandleClientMessage },
            { NetworkMessageType.CHAT_QUIT,         HandleClientExit },
            { NetworkMessageType.PONG,              HandleClientPong },
            { NetworkMessageType.READY,             HandleReadyPlayer },
            { NetworkMessageType.BUTTON_SELECTED,   HandleButtonPress },
            { NetworkMessageType.START_CONFIRM,     HandleStartConfirm },
        };

        #region variables
        [SerializeField] private int _maxPlayers = 4;

        //Public networking variables
        public NetworkDriver Driver;
        public NetworkPipeline Pipeline;

        //Public variables
        public ChatCanvas Chat;
        public NetworkManager NetworkManager;
        public List<NetworkConnection> ActivePlayers = new List<NetworkConnection>();
        public List<NetworkConnection> TodoPlayers;
        [HideInInspector] public NetworkConnection CurrentPlayer;
        [HideInInspector] public int Round = 0; 
        [HideInInspector] public int RequiredButton { get; set; }
        [HideInInspector] public float CurrentPlayerScore = 0;

        //Private networking variables
        private NativeList<NetworkConnection> _connections;
        private Dictionary<NetworkConnection, string> _nameList = new Dictionary<NetworkConnection, string>();
        private Dictionary<NetworkConnection, NetworkedPlayer> _playerInstances = new Dictionary<NetworkConnection, NetworkedPlayer>();
        private Dictionary<NetworkConnection, PingPong> _pongDict = new Dictionary<NetworkConnection, PingPong>();

        //Pricate variables
        bool _sessionStarted = false;
        #endregion

        #region UnityFunctions
        void Start() {
            Init();
        }

        // Write this immediately after creating the above Start calls, so you don't forget
        //  Or else you well get lingering thread sockets, and will have trouble starting new ones!
        void OnDestroy() {
            ClearServerData();
        }

        void Update() {
            // This is a jobified system, so we need to tell it to handle all its outstanding tasks first
            Driver.ScheduleUpdate().Complete();

            CleanupConnections();

            AcceptNewConnections();

            ProcessData();

            CheckTimeout();
        }

        #endregion

        #region BackendServerFunctions
        private void Init()
        {
            // Create Driver
            Driver = NetworkDriver.Create(new ReliableUtility.Parameters { WindowSize = 32 });
            Pipeline = Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

            // Open listener on server port
            NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
            endpoint.Port = 1511;
            if (Driver.Bind(endpoint) != 0)
                Debug.Log("Failed to bind to port 1511");
            else
                Driver.Listen();

            _connections = new NativeList<NetworkConnection>(_maxPlayers, Allocator.Persistent);
        }

        private void CleanupConnections()
        {
            // Clean up connections, remove stale ones
            for (int i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                {
                    _connections.RemoveAtSwapBack(i);
                    // This little trick means we can alter the contents of the list without breaking/skipping instances
                    --i;
                }
            }
        }

        private void AcceptNewConnections()
        {
            if (_sessionStarted) return;

            // Accept new connections
            NetworkConnection c;
            while ((c = Driver.Accept()) != default(NetworkConnection))
            {
                _connections.Add(c);
                // Debug.Log("Accepted a connection");
            }
        }

        private void ProcessData()
        {
            DataStreamReader stream;
            for (int i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                    continue;

                // Loop through available events
                NetworkEvent.Type cmd;
                while ((cmd = Driver.PopEventForConnection(_connections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        // First UInt is always message type (this is our own first design choice)
                        NetworkMessageType msgType = (NetworkMessageType)stream.ReadUShort();

                        // Create instance and deserialize
                        MessageHeader header = (MessageHeader)System.Activator.CreateInstance(NetworkMessageInfo.TypeMap[msgType]);
                        header.DeserializeObject(ref stream);

                        if (NetworkMessageHandlers.ContainsKey(msgType))
                        {
                            try
                            {
                                NetworkMessageHandlers[msgType].Invoke(this, _connections[i], header);
                            }
                            catch
                            {
                                Debug.LogError($"Badly formatted message received: {msgType}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Unsupported message type received: {msgType}", this);
                        }
                    }
                }
            }
        }

        private void CheckTimeout()
        {
            // Ping Pong stuff for timeout disconnects
            for (int i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                    continue;

                if (_pongDict.ContainsKey(_connections[i]))
                {
                    if (Time.time - _pongDict[_connections[i]].lastSendTime > 5f)
                    {
                        _pongDict[_connections[i]].lastSendTime = Time.time;
                        if (_pongDict[_connections[i]].status == 0)
                        {
                            // Remove from all the dicts, save name / id for msg

                            // FIXME: for some reason, sometimes this isn't in the list?
                            if (_nameList.ContainsKey(_connections[i]))
                            {
                                _nameList.Remove(_connections[i]);
                            }

                            uint destroyId = _playerInstances[_connections[i]].networkId;
                            NetworkManager.DestroyWithId(destroyId);
                            _playerInstances.Remove(_connections[i]);

                            string name = _pongDict[_connections[i]].name;
                            _pongDict.Remove(_connections[i]);

                            // Disconnect this player
                            _connections[i].Disconnect(Driver);

                            // Build messages
                            string msg = $"{name} has been Disconnected (connection timed out)";
                            Chat.NewMessage(msg, ChatCanvas.leaveColor);

                            ChatMessage quitMsg = new ChatMessage
                            {
                                message = msg,
                                messageType = MessageType.QUIT
                            };

                            DestroyMessage destroyMsg = new DestroyMessage
                            {
                                networkId = destroyId
                            };

                            // Broadcast
                            SendBroadcast(quitMsg);
                            SendBroadcast(destroyMsg, _connections[i]);

                            // Clean up
                            _connections[i] = default;
                        }
                        else
                        {
                            _pongDict[_connections[i]].status--;
                            PingMessage pingMsg = new PingMessage();
                            SendUnicast(_connections[i], pingMsg);
                        }
                    }
                }
                else if (_nameList.ContainsKey(_connections[i]))
                { //means they've succesfully handshaked
                    PingPong ping = new PingPong
                    {
                        lastSendTime = Time.time,
                        status = 3,    // 3 retries
                        name = _nameList[_connections[i]]
                    };
                    _pongDict.Add(_connections[i], ping);

                    PingMessage pingMsg = new PingMessage();
                    SendUnicast(_connections[i], pingMsg);
                }
            }
        }

        private void ClearServerData()
        {
            Driver.Dispose();
            _connections.Dispose();
        }
        #endregion

        #region ServerMessaging
        public void SendUnicast( NetworkConnection connection, MessageHeader header, bool realiable = true ) {
            DataStreamWriter writer;
            int result = Driver.BeginSend(realiable ? Pipeline : NetworkPipeline.Null, connection, out writer);
            if (result == 0) {
                header.SerializeObject(ref writer);
                Driver.EndSend(writer);
            }
        }

        public void SendBroadcast(MessageHeader header, NetworkConnection toExclude = default, bool realiable = true) {
            for (int i = 0; i < _connections.Length; i++) {
                if (!_connections[i].IsCreated || _connections[i] == toExclude)
                    continue;

                SendUnicast(_connections[i], header, realiable);
            }
        }
        #endregion

        #region HandleMessages
        static void HandleClientHandshake(Server serv, NetworkConnection connection, MessageHeader header) {
            HandshakeMessage message = header as HandshakeMessage;

            // Add to list
            serv._nameList.Add(connection, message.name);
            string msg = $"{message.name} has joined the chat.";
            serv.Chat.NewMessage(msg, ChatCanvas.joinColor);

            ChatMessage chatMsg = new ChatMessage {
                messageType = MessageType.JOIN,
                message = msg
            };

            // Send all clients the chat message
            serv.SendBroadcast(chatMsg);

            // spawn a non-local, server player
            GameObject player;
            uint networkId = 0;
            if (serv.NetworkManager.SpawnWithId(NetworkSpawnObject.PLAYER, NetworkManager.NextNetworkID, out player)) {
                // Get and setup player instance
                NetworkedPlayer playerInstance = player.GetComponent<NetworkedPlayer>();
                playerInstance.isServer = true;
                playerInstance.isLocal = false;
                networkId = playerInstance.networkId;

                serv._playerInstances.Add(connection, playerInstance);

                // Send spawn local player back to sender
                HandshakeResponseMessage responseMsg = new HandshakeResponseMessage {
                    message = $"Welcome {message.name}!",
                    networkId = playerInstance.networkId
                };

                serv.SendUnicast(connection, responseMsg);
            }
            else {
                Debug.LogError("Could not spawn player instance");
            }

            // Send all existing players to this player
            foreach (KeyValuePair<NetworkConnection, NetworkedPlayer> pair in serv._playerInstances) {
                if (pair.Key == connection) continue;

                SpawnMessage spawnMsg = new SpawnMessage {
                    networkId = pair.Value.networkId,
                    objectType = NetworkSpawnObject.PLAYER
                };

                serv.SendUnicast(connection, spawnMsg);
            }

            // Send creation of this player to all existing players
            if (networkId != 0) {
                SpawnMessage spawnMsg = new SpawnMessage {
                    networkId = networkId,
                    objectType = NetworkSpawnObject.PLAYER
                };
                serv.SendBroadcast(spawnMsg, connection);
            }
            else {
                Debug.LogError("Invalid network id for broadcasting creation");
            }
        }

        static void HandleClientMessage(Server serv, NetworkConnection connection, MessageHeader header) {
            ChatMessage receivedMsg = header as ChatMessage;

            if (serv._nameList.ContainsKey(connection)) {
                string msg = $"{serv._nameList[connection]}: {receivedMsg.message}";
                serv.Chat.NewMessage(msg, ChatCanvas.chatColor);

                receivedMsg.message = msg;

                // forward message to all clients
                serv.SendBroadcast(receivedMsg);
            }
            else {
                Debug.LogError($"Received message from unlisted connection: {receivedMsg.message}");
            }
        }

        static void HandleClientExit(Server serv, NetworkConnection connection, MessageHeader header) {
            //Removed assignment because not used
            //ChatQuitMessage quitMsg = header as ChatQuitMessage;

            if (serv._nameList.ContainsKey(connection)) {
                string msg = $"{serv._nameList[connection]} has left the chat.";
                serv.Chat.NewMessage(msg, ChatCanvas.leaveColor);

                // Clean up
                serv._nameList.Remove(connection);
                // if you join and quit quickly, might not be in this dict yet
                if (serv._pongDict.ContainsKey(connection)) {
                    serv._pongDict.Remove(connection);
                }

                connection.Disconnect(serv.Driver);

                uint destroyId = serv._playerInstances[connection].networkId;
                Destroy(serv._playerInstances[connection].gameObject);
                serv._playerInstances.Remove(connection);

                // Build messages
                ChatMessage chatMsg = new ChatMessage {
                    message = msg,
                    messageType = MessageType.QUIT
                };

                DestroyMessage destroyMsg = new DestroyMessage {
                    networkId = destroyId
                };

                // Send Messages to all other clients
                serv.SendBroadcast(chatMsg, connection);
                serv.SendBroadcast(destroyMsg, connection);
            }
            else {
                Debug.LogError("Received exit from unlisted connection");
            }
        }

        static void HandleClientPong(Server serv, NetworkConnection connection, MessageHeader header) {
            // Debug.Log("PONG");
            serv._pongDict[connection].status = 3;   //reset retry count
        }

        static void HandleReadyPlayer(Server serv, NetworkConnection connection, MessageHeader header)
        {
            serv._playerInstances[connection].isReady = true;

            bool ready = true;
            serv.ActivePlayers.Clear();
            foreach (var player in serv._playerInstances.Keys)
            {
                if (player != default(NetworkConnection))
                {
                    if(ready) ready = serv._playerInstances[player].isReady;
                    serv.ActivePlayers.Add(player);
                }
            }
            Debug.Log(serv.ActivePlayers.Count);

            //StartGame
            if(ready && serv.ActivePlayers.Count >= 2)
            {
                serv._sessionStarted = true;

                int randomCount = Random.Range(0, serv.ActivePlayers.Count);
                Debug.Log(randomCount);

                StartGameMessage startMsg = new StartGameMessage
                {
                    networkId = serv._playerInstances[connection].networkId,
                    startPlayer = serv._playerInstances[serv.ActivePlayers[randomCount]].networkId
                };
                Debug.Log(startMsg.startPlayer + " " + startMsg.networkId);
                serv.SendBroadcast(startMsg);
                serv.Round = 1;
                Debug.Log("LETS GOOOO");

                //Maybe wait for a return
            }
            else
            {
                Debug.Log("Not ready yet");
            }
        }

        static void HandleStartConfirm(Server serv, NetworkConnection connection, MessageHeader header)
        {
            serv._playerInstances[connection].hasConfirmed = true;

            bool ready = true;
            serv.ActivePlayers.Clear();
            foreach (var player in serv._playerInstances.Keys)
            {
                if (player != default(NetworkConnection))
                {
                    if (ready) ready = serv._playerInstances[player].hasConfirmed;
                    serv.ActivePlayers.Add(player);
                }
            }
            Debug.Log(serv.ActivePlayers.Count);

            //StartGame
            if (ready && serv.ActivePlayers.Count >= 2)
            {
                int requiredButton = Random.Range(1, 10);
                serv.RequiredButton = requiredButton;
                serv.CurrentPlayerScore = 0;
                serv.TodoPlayers = serv.ActivePlayers;
                int randomCount = Random.Range(0, serv.TodoPlayers.Count);
                serv.CurrentPlayer = serv.TodoPlayers[randomCount];
                Debug.Log(randomCount);

                //StartFirstButton

                ButtonRequestMessage startMsg = new ButtonRequestMessage
                {
                    networkId = serv._playerInstances[serv.CurrentPlayer].networkId,
                    button = requiredButton
                };
                    Debug.Log(requiredButton);
                //Debug.Log(startMsg.startPlayer + " " + startMsg.networkId);
                serv.SendUnicast(serv.CurrentPlayer, startMsg);
                serv.TodoPlayers.Remove(serv.TodoPlayers[randomCount]);
                serv.Round = 1;
                Debug.Log("LETS GOOOO");

                //Maybe wait for a return
            }
            else
            {
                Debug.Log("Not ready yet");
            }
        }

        static void HandleButtonPress(Server serv, NetworkConnection connection, MessageHeader header)
        {
            if (connection != serv.CurrentPlayer) return;

            ButtonPressedMessage buttonMsg = header as ButtonPressedMessage;
            Debug.Log(serv.RequiredButton + "" + buttonMsg.button);
            if(serv.RequiredButton == buttonMsg.button)
            {
                //Check time approx

                //Add time score
                serv.CurrentPlayerScore += buttonMsg.time;
                //Send Next button


                if (serv.Round >= 3)
                {
                    //ReturnPlayerScore
                    ScoreMessage msg = new ScoreMessage { score = serv.CurrentPlayerScore };
                    serv.SendUnicast(serv.CurrentPlayer, msg);

                    //StartNextPlayer
                    if (serv.TodoPlayers.Count == 0)
                    {
                        //End game

                        return;
                    }

                    int requiredButton = Random.Range(1, 10);
                    serv.RequiredButton = requiredButton;
                    serv.CurrentPlayerScore = 0;
                    int randomCount = Random.Range(0, serv.TodoPlayers.Count);
                    serv.CurrentPlayer = serv.TodoPlayers[randomCount];
                    Debug.Log(randomCount);

                    //StartFirstButton

                    ButtonRequestMessage startMsg = new ButtonRequestMessage
                    {
                        networkId = serv._playerInstances[connection].networkId,
                        button = requiredButton
                    };
                    //Debug.Log(startMsg.startPlayer + " " + startMsg.networkId);
                    serv.SendUnicast(serv.CurrentPlayer, startMsg);
                    serv.TodoPlayers.Remove(serv.TodoPlayers[randomCount]);
                    serv.Round = 1;
                    return;
                }
                else
                {
                    int requiredButton = Random.Range(1, 10);
                    serv.RequiredButton = requiredButton;
                    ButtonRequestMessage startMsg = new ButtonRequestMessage
                    {
                        networkId = serv._playerInstances[connection].networkId,
                        button = requiredButton
                    };
                    Debug.Log(requiredButton);
                    serv.SendUnicast(serv.CurrentPlayer, startMsg);
                }
                serv.Round++;

                Debug.Log("Received " + buttonMsg.time);
            }
            else
            {
                Debug.Log("not right button pressed somehow");
            }
        }

        #endregion
    }
}