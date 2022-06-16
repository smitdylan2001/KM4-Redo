using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMultiplayerGame
{
    public class NetworkManager : MonoBehaviour
    {
        private static uint _nextNetworkId = 0;
        public static uint NextNetworkID => ++_nextNetworkId;

        [SerializeField] private NetworkSpawnInfo _spawnInfo;
        private Dictionary<uint, GameObject> _networkedReferences = new Dictionary<uint, GameObject>();

        public bool GetReference(uint id, out GameObject obj)
        {
            obj = null;
            if (_networkedReferences.ContainsKey(id))
            {
                obj = _networkedReferences[id];
                return true;
            }
            return false;
        }

        public bool SpawnWithId(NetworkSpawnObject type, uint id, out GameObject obj)
        {
            obj = null;
            if (_networkedReferences.ContainsKey(id))
            {
                return false;
            }
            else
            {
                // assuming this doesn't crash...
                obj = Instantiate(_spawnInfo.prefabList[(int)type]);

                if (!obj.TryGetComponent<NetworkedBehaviour>(out var beh))
                {
                    beh = obj.AddComponent<NetworkedBehaviour>();
                }
                beh.NetworkId = id;

                _networkedReferences.Add(id, obj);

                return true;
            }
        }

        public bool DestroyWithId(uint id)
        {
            if (_networkedReferences.ContainsKey(id))
            {
                Destroy(_networkedReferences[id]);
                _networkedReferences.Remove(id);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}