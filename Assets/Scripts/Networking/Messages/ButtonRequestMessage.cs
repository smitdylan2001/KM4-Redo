using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityMultiplayerGame
{
	public class ButtonRequestMessage : MessageHeader
	{
		public override NetworkMessageType Type
		{
			get
			{
				return NetworkMessageType.BUTTON_REQUEST;
			}
		}

		public uint networkId;
		public int button;

		public override void SerializeObject(ref DataStreamWriter writer)
		{
			// very important to call this first
			base.SerializeObject(ref writer);
			writer.WriteUInt(networkId);
			writer.WriteInt(button);
		}

		public override void DeserializeObject(ref DataStreamReader reader)
		{
			// very important to call this first
			base.DeserializeObject(ref reader);
			networkId = reader.ReadUInt();
			button = reader.ReadInt();
		}
	}
}