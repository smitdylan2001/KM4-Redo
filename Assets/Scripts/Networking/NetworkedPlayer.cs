using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMultiplayerGame
{
	public class NetworkedPlayer : NetworkedBehaviour
	{
		public bool IsLocal = false;
		public bool IsServer = false;
		public bool IsReady = false;
		public bool HasConfirmed = false;
	}
}