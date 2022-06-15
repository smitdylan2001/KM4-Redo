using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityMultiplayerGame
{
    public class ButtonPressManager : MonoBehaviour
    {
        #region Singleton
        private static ButtonPressManager _instance;

        public static ButtonPressManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ButtonPressManager>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            _instance=this;
        }
        #endregion

        [SerializeField] Image[] _buttons;
        [SerializeField] GameObject _exitButton;
        int _selectedButton = 0;
        Client _client;
        float _currentTime = 0;

          
        void Start()
        {
            _client = FindObjectOfType<Client>();
            HideButtons();
        }

        public void ReceiveButton(int button)
        {
            _currentTime = Time.time;
            _selectedButton = button;
            HighlightButton(_selectedButton);
        }

        public void HighlightButton(int button)
        {
            _buttons[button - 1].color = Color.black;
        }

        public void DehighlightButton(int button)
        {
            if(button == _selectedButton)
            {
                _buttons[button - 1].color = Color.white;
            }
        }

        public void OnPress(int button)
        {
            if (button != _selectedButton) return;

            ButtonPressedMessage message = new ButtonPressedMessage { button = button, time = Time.time - _currentTime };
            _client.SendPackedMessage(message);
        }

        public void HideButtons(bool showExit = false)
        {
            foreach(var button in _buttons)
            {
                button.gameObject.SetActive(false);
            }
            if (showExit) ShowExitButton();
        }
        public void ShowButtons()
        {
            foreach (var button in _buttons)
            {
                button.gameObject.SetActive(true);
            }
            //Remove waiting screen
        }

        private void ShowExitButton()
        {
            _exitButton.SetActive(true);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}