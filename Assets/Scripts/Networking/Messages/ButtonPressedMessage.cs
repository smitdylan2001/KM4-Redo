using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityMultiplayerGame
{
	public class ButtonPressedMessage : MessageHeader
	{
		public override NetworkMessageType Type
		{
			get
			{
				return NetworkMessageType.BUTTON_SELECTED;
			}
		}

		public uint networkId;
		public int button;
		public int firstNumber  { get { return button / 10; } }
		public int secondNumber { get { return button % 10; } }
		public float time;

		public override void SerializeObject(ref DataStreamWriter writer)
		{
			// very important to call this first
			base.SerializeObject(ref writer);
			writer.WriteInt(button);
			writer.WriteFloat(time);
		}

		public override void DeserializeObject(ref DataStreamReader reader)
		{
			// very important to call this first
			base.DeserializeObject(ref reader);
			button = reader.ReadInt();
			time = reader.ReadFloat();
		}
	}
}