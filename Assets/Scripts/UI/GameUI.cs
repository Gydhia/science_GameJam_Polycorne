using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameUI : MonoBehaviour
    {
        public PlayableDirector director;
        public GameObject EndPopup;
        public GameObject DialogPopup;
        public GameObject PercentScore;
        public GameObject PointScore;
        public GameObject FeufolletArrivedScore;
        public GameObject FeufolletLaunchedScore;
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
            if (SoundController.Instance != null)
                SoundController.Instance.StopMusic();
            SceneManager.LoadScene(Board.Instance.CurrentLevel);
            SceneManager.LoadScene("BackgroundScene", LoadSceneMode.Additive);
        }

        public void OnClickNextButton()
        {
            if (SoundController.Instance != null)
                SoundController.Instance.StopMusic();
            SceneManager.LoadScene(Board.Instance.NextLevel);
            SceneManager.LoadScene("BackgroundScene", LoadSceneMode.Additive);
        }

        public void FireEndPopup()
        {
            EndPopup.SetActive(true);
            PercentScore.GetComponent<TextMeshProUGUI>().text =
                ((Board.Instance.Score / Board.Instance.NumberOfTrains) * 100).ToString() + "% de réussite";
            PointScore.GetComponent<TextMeshProUGUI>().text = Board.Instance.Score.ToString() + " pts";
            FeufolletArrivedScore.GetComponent<TextMeshProUGUI>().text = Board.Instance.Score.ToString() + " feu follets arrivés";
            FeufolletLaunchedScore.GetComponent<TextMeshProUGUI>().text = Board.Instance.NumberOfTrains.ToString() + " feu follets partis";
            director.Stop();
        }
    }
}
