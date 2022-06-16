using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Networking.Transport;

namespace UnityMultiplayerGame
{
    public abstract class MessageHeader
    {
        private static uint _nextID = 0;
        public static uint NextID => ++_nextID;
        public abstract NetworkMessageType Type { get; }
        public uint ID { get; private set; } = NextID;

        public virtual void SerializeObject(ref DataStreamWriter writer) {
            writer.WriteUShort((ushort)Type);
            writer.WriteUInt(ID);
        }

        public virtual void DeserializeObject(ref DataStreamReader reader) {
            ID = reader.ReadUInt();
        }
    }
}