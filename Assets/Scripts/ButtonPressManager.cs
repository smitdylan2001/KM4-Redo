using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChatClientExample
{
    public class ButtonPressManager : MonoBehaviour
    {
        int _selectedButton = 11;
        Client _client;
        float _currentTime = 0;

        void Start()
        {
            _client = FindObjectOfType<Client>();
        }

        public void HighlightButton()
        {
            _currentTime = Time.time;
        }

        public void OnPress(int button)
        {
            if (button != _selectedButton) return;

            ButtonPressedMessage message = new ButtonPressedMessage { button = button, time = Time.time - _currentTime };
            _client.SendPackedMessage(message);
        }
    }
}