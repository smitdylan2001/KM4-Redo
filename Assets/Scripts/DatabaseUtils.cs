using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityMultiplayerGame
{
	public class DatabaseUtils : MonoBehaviour
	{
		private static DatabaseUtils _instance;
		public static DatabaseUtils Instance
		{
			get
			{
				if (_instance == null) FindObjectOfType<DatabaseUtils>();
				return _instance;
			}
		}

		static public int PlayerID;
		static public string SessionID;

		[SerializeField] private InputField _emailInput, _passwordInput;

		private void Start()
		{
			if (_instance == null) _instance = this;
			else Destroy(this);
		}

		#region SignIn
		public void StartSignIn(bool isServer)
		{
			StartCoroutine(SignIn(isServer));
		}

		private IEnumerator SignIn(bool isServer)
		{
			string url = "https://studenthome.hku.nl/~dylan.smit/Database/server_login.php?id=2&pw=Admin2";

			using (UnityWebRequest www = UnityWebRequest.Get(url))
			{
				yield return www.SendWebRequest();

				if (www.error == null)
				{
					SessionID = www.downloadHandler.text;
					Debug.Log(SessionID);
				}
				else
				{
					Debug.LogError(www.error);
				}
			}

			if (SessionID == "0")
			{
				Debug.LogError("Could not sign into server");
				yield break;
			}

			url = $"https://studenthome.hku.nl/~dylan.smit/Database/user_login.php?PHPSESSID={SessionID}&username={_emailInput.text}&pw={_passwordInput.text}"; //https://studenthome.hku.nl/~dylan.smit/Database/user_login.php?username=test@test.nl&pw=testPass
			using (UnityWebRequest www = UnityWebRequest.Get(url))
			{
				yield return www.SendWebRequest();

				if (www.error == null)
				{
					PlayerID = int.Parse(www.downloadHandler.text);
					Debug.Log(PlayerID);
				}
				else
				{
					Debug.LogError(www.error);
				}
			}

			if (PlayerID == 0)
			{
				Debug.LogError("Could not sign into user");
				yield break;
			}

			if (isServer) SceneManager.LoadScene(1);
			else SceneManager.LoadScene(2);
		}
		#endregion

		#region ScoreSubmit
		public void SendScoreToDatabase(int score)
		{
			StartCoroutine(SendScore(score));
		}

		private IEnumerator SendScore(int score)
		{
			int succeded = 0;

			var url = $"https://studenthome.hku.nl/~dylan.smit/Database/insert_score.php?PHPSESSID={SessionID}&score={score}&id={PlayerID}";
			using (UnityWebRequest www = UnityWebRequest.Get(url))
			{
				yield return www.SendWebRequest();

				if (www.error == null)
				{
					succeded = int.Parse(www.downloadHandler.text);
				}
				else
				{
					Debug.LogError(www.error);
				}
			}

			if (succeded == 1)
			{
				Debug.Log("Submitted score " + score);
			}
			else
			{
				Debug.LogError("Could not commit score");
			}
		}
		#endregion
	}
}