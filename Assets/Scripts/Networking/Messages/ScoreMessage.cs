using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityMultiplayerGame
{
	public class ScoreMessage : MessageHeader
	{
		public override NetworkMessageType Type
		{
			get
			{
				return NetworkMessageType.SCORE;
			}
		}

		public uint networkId;
		public float score;

		public override void SerializeObject(ref DataStreamWriter writer)
		{
			// very important to call this first
			base.SerializeObject(ref writer);
			writer.WriteUInt(networkId);
			writer.WriteFloat(score);
		}

		public override void DeserializeObject(ref DataStreamReader reader)
		{
			// very important to call this first
			base.DeserializeObject(ref reader);
			networkId = reader.ReadUInt();
			score = reader.ReadFloat();
		}
	}
}