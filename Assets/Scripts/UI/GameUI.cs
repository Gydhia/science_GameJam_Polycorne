using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.Scripts.UI
{
    public class GameUI : MonoBehaviour
    {
        public PlayableDirector director;
        public GameObject EndPopup;
        public GameObject DialogPopup;
        private static GameUI _gameUIInstance;

        public static GameUI Instance
        {
            get
            {
                if (GameUI._gameUIInstance == null)
                    GameUI._gameUIInstance = GameObject.FindObjectOfType<GameUI>();
                return GameUI._gameUIInstance;
            }
        }

        public void StartTimeline()
        {
            director.Play();
        }

        public void OnClickCloseButton()
        {
            DialogPopup.SetActive(false);
            EndPopup.SetActive(false);
        }

        public void OnClickRestartButton()
        {
            
        }

        public void FireEndPopup()
        {
            EndPopup.SetActive(true);
            director.Stop();
        }
    }
}
