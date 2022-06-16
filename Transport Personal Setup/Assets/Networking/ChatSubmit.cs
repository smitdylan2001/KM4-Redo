using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatSubmit : MonoBehaviour
{
    public void OnSubmitClick()
    {
        NetChatMessage chatMessage = new NetChatMessage(GetComponent<TMP_InputField>().text);
        FindObjectOfType<BaseClient>().SendMessageToServer(chatMessage);
    }
}
