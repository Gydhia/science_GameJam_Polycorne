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
        public Board Board;
        
        public PlayableDirector director;
        public GameObject EndPopup;
        public GameObject DialogPopup;
        public GameObject PercentScore;
        public GameObject PointScore;
        public GameObject FeufolletArrivedScore;
        public GameObject FeufolletLaunchedScore;
        public GameObject LoseText;
        public GameObject LoseText2;
        public GameObject WinText;
        public GameObject WinText2;
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

        public void Start()
        {
            this.Board = GameObject.FindObjectOfType<Board>();
        }

        public void StartTimeline()
        {
            director.Play();
        }

        public void OnClickCloseButton()
        {
            DialogPopup.SetActive(false);
            EndPopup.SetActive(false);

            if (this.Board != null)
            {
                if (this.Board.TrainstationsContainer != null)
                    this.Board.TrainstationsContainer.SetActive(true);
                if (this.Board.CardDeckContainer != null)
                    this.Board.CardDeckContainer.SetActive(true);
                if (this.Board.TracksContainer != null)
                    this.Board.TracksContainer.SetActive(true);

                if (this.Board.ResultsPanel != null)
                    this.Board.ResultsPanel.gameObject.SetActive(false);
            }
        }

        public void OnClickRestartButton()
        {
            if (this.Board != null)
            {
                if (this.Board.TrainstationsContainer != null)
                    this.Board.TrainstationsContainer.SetActive(true);
                if (this.Board.CardDeckContainer != null)
                    this.Board.CardDeckContainer.SetActive(true);
                if (this.Board.TracksContainer != null)
                    this.Board.TracksContainer.SetActive(true);

                if (this.Board.ResultsPanel != null)
                    this.Board.ResultsPanel.gameObject.SetActive(false);
            }

            if (SoundController.Instance != null)
                SoundController.Instance.StopMusic();

            SceneManager.LoadScene(Board.Instance.CurrentLevel);
            SceneManager.LoadScene("BackgroundScene", LoadSceneMode.Additive);
        }

        public void OnClickNextButton()
        {
            if(this.Board != null)
            {
                if (this.Board.TrainstationsContainer != null)
                    this.Board.TrainstationsContainer.SetActive(true);
                if (this.Board.CardDeckContainer != null)
                    this.Board.CardDeckContainer.SetActive(true);
                if (this.Board.TracksContainer != null)
                    this.Board.TracksContainer.SetActive(true);

                if (this.Board.ResultsPanel != null)
                    this.Board.ResultsPanel.gameObject.SetActive(false);
            }
            

            if (SoundController.Instance != null)
                SoundController.Instance.StopMusic();
            SceneManager.LoadScene(Board.Instance.NextLevel);
            SceneManager.LoadScene("BackgroundScene", LoadSceneMode.Additive);
        }

        public void FireEndPopup()
        {
            if (this.Board != null)
            {
                if (this.Board.TrainstationsContainer != null)
                    this.Board.TrainstationsContainer.SetActive(false);
                if (this.Board.CardDeckContainer != null)
                    this.Board.CardDeckContainer.SetActive(false);
                if (this.Board.TracksContainer != null)
                    this.Board.TracksContainer.SetActive(false);
            }
            EndPopup.SetActive(true);
            PercentScore.GetComponent<TextMeshProUGUI>().text =
                Board.Instance.Score.ToString() + "% de réussite";
            PointScore.GetComponent<TextMeshProUGUI>().text = Board.Instance.Score.ToString() + " pts";
            FeufolletArrivedScore.GetComponent<TextMeshProUGUI>().text = Board.Instance.Score.ToString() + " feu follets arrivés";
            FeufolletLaunchedScore.GetComponent<TextMeshProUGUI>().text = Board.Instance.NumberOfTrains.ToString() + " feu follets partis";
            if(Board.Instance.Score >= 50)
            {
                this.WinText.SetActive(true);
                this.WinText2.SetActive(true);
                this.LoseText.SetActive(false);
                this.LoseText2.SetActive(false);
            }
            else
            {
                this.WinText.SetActive(false);
                this.WinText2.SetActive(false);
                this.LoseText.SetActive(true);
                this.LoseText2.SetActive(true);
            }
            director.Stop();
        }
    }
}
