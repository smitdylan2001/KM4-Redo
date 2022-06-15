using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DatabaseUtils : MonoBehaviour
{
	private static DatabaseUtils _instance;
	public static DatabaseUtils Instance { get {
			if (_instance == null) FindObjectOfType<DatabaseUtils>();
			return _instance;
		}}

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

	IEnumerator SignIn(bool isServer)
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
		}

		if (SessionID == "0") ;//TODO: error

		url = $"https://studenthome.hku.nl/~dylan.smit/Database/user_login.php?PHPSESSID={SessionID}&username={_emailInput.text}&pw={_passwordInput.text}"; //https://studenthome.hku.nl/~dylan.smit/Database/user_login.php?username=test@test.nl&pw=testPass
		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			yield return www.SendWebRequest();

			if (www.error == null)
			{
				PlayerID = int.Parse(www.downloadHandler.text);
				Debug.Log(PlayerID);
			}
		}

		if (PlayerID == 0) ;//TODO: error

		if (isServer) SceneManager.LoadScene(1);
		else SceneManager.LoadScene(2);
	}
	#endregion

	public void SendScoreToDatabase(int score)
    {

		
    }

	IEnumerator SendScore(int score)
    {
		var url = $"https://studenthome.hku.nl/~dylan.smit/Database/insert_score.php?PHPSESSID={SessionID}&score={score}&id={PlayerID}";
		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			yield return www.SendWebRequest();

			if (www.error == null)
			{
				int succeded = int.Parse(www.downloadHandler.text);
				if (succeded == 0) ;//TODO: error
			}
		}
	}
}
