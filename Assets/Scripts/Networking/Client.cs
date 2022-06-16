using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine.UI;
using Unity.Networking.Transport.Utilities;
using UnityEngine.SceneManagement;

namespace UnityMultiplayerGame
{
    public class Client : MonoBehaviour
    {
        static readonly Dictionary<NetworkMessageType, ClientMessageHandler> NetworkMessageHandlers = new Dictionary<NetworkMessageType, ClientMessageHandler> {
            { NetworkMessageType.HANDSHAKE_RESPONSE,        HandleServerHandshakeResponse },
            { NetworkMessageType.NETWORK_SPAWN,             HandleNetworkSpawn },             // uint networkId, uint objectType
            { NetworkMessageType.NETWORK_DESTROY,           HandleNetworkDestroy },           // uint networkId
            { NetworkMessageType.CHAT_MESSAGE,              HandleChatMessage },
            { NetworkMessageType.PING,                      HandlePing },
            { NetworkMessageType.START,                     HandleGameStart },
            { NetworkMessageType.BUTTON_REQUEST,            HandleButtonRequest },
            {NetworkMessageType.SCORE,                      HandleScoreReceive }
        };

        #region Variables
        //Static variables
        public static string ServerIP;
        public static string ClientName = "";

        //Public networking variables
        public NetworkDriver Driver;
        public NetworkPipeline Pipeline;
        public NetworkConnection Connection;
        public NetworkManager NetworkManager;

        //Public variables
        public bool Done { get; private set; }

        //Private variables
        private bool _connected = false;
        private float _startTime = 0;
        #endregion

        #region UnityEvents
        // Start is called before the first frame update
        void Start() {
            Init();
        }

        // No collections list this time...
        void OnApplicationQuit() {
            Disconnect();
        }

        void OnDestroy() {
            ClearConnection();
        }

        void Update() {
            Driver.ScheduleUpdate().Complete();

            CheckConnection();

            ProcessData();
        }
        #endregion

        #region BackendServerFunctions
        private void Init()
        {
            _startTime = Time.time;
            // Create connection to server IP
            Driver = NetworkDriver.Create(new ReliableUtility.Parameters { WindowSize = 32 });
            Pipeline = Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

            Connection = default(NetworkConnection);

            var endpoint = NetworkEndPoint.Parse(ServerIP, 9000, NetworkFamily.Ipv4);
            endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = 1511;
            Connection = Driver.Connect(endpoint);
        }

        private void Disconnect()
        {
            // Disconnecting on application exit currently (to keep it simple)
            if (Connection.IsCreated)
            {
                Connection.Disconnect(Driver);
                Connection = default(NetworkConnection);
            }
        }

        private void ClearConnection()
        {
            Driver.Dispose();
        }

        private void CheckConnection()
        {
            if (!_connected && Time.time - _startTime > 5f)
            {
                SceneManager.LoadScene(0);
            }

            if (!Connection.IsCreated)
            {
                if (!Done)
                    Debug.Log("Something went wrong during connect");
                return;
            }
        }

        private void ProcessData()
        {
            NetworkEvent.Type cmd;
            while ((cmd = Connection.PopEvent(Driver, out DataStreamReader stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    _connected = true;
                    Debug.Log("We are now connected to the server");

                    // TODO: Create handshake message
                    var header = new HandshakeMessage
                    {
                        name = ClientName
                    };
                    SendPackedMessage(header);
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    Done = true;

                    // First UInt is always message type (this is our own first design choice)
                    NetworkMessageType msgType = (NetworkMessageType)stream.ReadUShort();

                    MessageHeader header = (MessageHeader)System.Activator.CreateInstance(NetworkMessageInfo.TypeMap[msgType]);
                    header.DeserializeObject(ref stream);

                    if (NetworkMessageHandlers.ContainsKey(msgType))
                    {
                        NetworkMessageHandlers[msgType].Invoke(this, header);
                    }
                    else
                    {
                        Debug.LogWarning($"Unsupported message type received: {msgType}", this);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                    Connection = default(NetworkConnection);
                }
            }
        }
        #endregion

        #region ClientFunctions
        /*Old Messaging functions
        public InputField input;

        // UI FUNCTIONS (0 refs)
        public void SendMessage() {
            ChatMessage chatMsg = new ChatMessage {
                message = input.text
            };
            if ( _connected ) SendPackedMessage(chatMsg);
            input.text = "";
        }
        */

        public void ExitChat() {
            ChatQuitMessage chatQuitMsg = new ChatQuitMessage();
            if (_connected) SendPackedMessage(chatQuitMsg);
            SceneManager.LoadScene(0);
        }
        // END UI FUNCTIONS
        

        public void OnReady()
        {
            SendPackedMessage(new ReadyMessage());
        }
        #endregion

        public void SendPackedMessage( MessageHeader header ) {
            DataStreamWriter writer;
            int result = Driver.BeginSend(Pipeline, Connection, out writer);

            // non-0 is an error code
            if (result == 0) {
                header.SerializeObject(ref writer);
                Driver.EndSend(writer);
            }
            else {
                Debug.LogError($"Could not wrote message to driver: {result}", this);
            }
        }

        #region HandleMessages
        static void HandleServerHandshakeResponse(Client client, MessageHeader header) {
            HandshakeResponseMessage response = header as HandshakeResponseMessage;

            if (client.NetworkManager.SpawnWithId(NetworkSpawnObject.PLAYER, response.networkId, out GameObject obj))
            {
                NetworkedPlayer player = obj.GetComponent<NetworkedPlayer>();
                player.IsLocal = true;
                player.IsServer = false;
            }
            else
            {
                Debug.LogError("Could not spawn player!");
            }
        }

        static void HandleNetworkSpawn(Client client, MessageHeader header) {
            SpawnMessage spawnMsg = header as SpawnMessage;
            if (!client.NetworkManager.SpawnWithId(spawnMsg.objectType, spawnMsg.networkId, out _))
            {
                Debug.LogError($"Could not spawn {spawnMsg.objectType} for id {spawnMsg.networkId}!");
            }
        }

        static void HandleNetworkDestroy(Client client, MessageHeader header) {
            DestroyMessage destroyMsg = header as DestroyMessage;
            if (!client.NetworkManager.DestroyWithId(destroyMsg.networkId)) {
                Debug.LogError($"Could not destroy object with id {destroyMsg.networkId}!");
            }
        }

        static void HandleChatMessage(Client client, MessageHeader header) {
            ChatMessage chatMsg = header as ChatMessage;
        }

        static void HandlePing(Client client, MessageHeader header) {
            Debug.Log("PING");

            PongMessage pongMsg = new PongMessage();
            client.SendPackedMessage(pongMsg);
        }

        static void HandleGameStart(Client client, MessageHeader header)
        {
            StartGameMessage posMsg = header as StartGameMessage;

            StartConfirmMessage msg = new StartConfirmMessage();
            client.SendPackedMessage(msg);

            //Show waiting screen
        }

        static void HandleButtonRequest(Client client, MessageHeader header)
        {
            ButtonPressManager.Instance.ShowButtons();

            ButtonRequestMessage msg = header as ButtonRequestMessage;

            ButtonPressManager.Instance.ReceiveButton(msg.button);
        }

        static void HandleScoreReceive(Client client, MessageHeader header)
        {
            ScoreMessage msg = header as ScoreMessage;

            DatabaseUtils.Instance.SendScoreToDatabase((int)(msg.score * 1000)); //Time in ms

            ScoreManager.Instance.ShowScore(msg.score);
        }
        #endregion
    }
}