using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSubmit : MonoBehaviour
{
    public int SelectedButton = 11;

    public void SendButtonToServer(int button)
    {
        if (button != SelectedButton) return;

        int first = button / 10;
        NetButtonPressed message = new NetButtonPressed(10, first, button % 10);
        FindObjectOfType<BaseClient>().SendMessageToServer(message);
    }
}
