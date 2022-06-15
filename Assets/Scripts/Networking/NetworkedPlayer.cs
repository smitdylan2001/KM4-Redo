using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMultiplayerGame
{

	public class NetworkedPlayer : NetworkedBehaviour
	{
		public bool isLocal = false;
		public bool isServer = false;
		public bool isReady = false;
		public bool hasConfirmed = false;
		public Camera localCamera;

		float refireTimer = 0;
		bool canJump = true;
		bool _isReady = false;
		float yVel = 0;

		Client client;
		Server server;

		private void Start() {
			if (isLocal) {
				GetComponentInChildren<Camera>().enabled = true;
				if ( Camera.main ) {
					Camera.main.enabled = false;
				}

				client = FindObjectOfType<Client>();
			}
			if ( isServer ) {
				server = FindObjectOfType<Server>();
			}
		}

		public void SetReady(bool ready)
        {
			_isReady = ready;
		}
	}
}