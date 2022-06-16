using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UnityMultiplayerGame
{
    public class ScoreManager : MonoBehaviour
    {
        #region Singleton
        private static ScoreManager _instance;

        public static ScoreManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ScoreManager>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }
        #endregion

        [SerializeField] Text _text;

        void Start()
        {
            _text.text = string.Empty;
        }

        public void ShowScore(float score)
        {
            _text.text = "Score: " + score.ToString("F4");
            ButtonPressManager.Instance.HideButtons(true);
        }
    }
}