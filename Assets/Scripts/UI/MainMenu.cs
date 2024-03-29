﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject CreditsPopup;
        public GameObject HelpPopup;
        public GameObject Title;
        public GameObject Buttons;


        private void Start()
        {
            Screen.SetResolution(1920, 1080, true);
            
            if (SoundController.Instance != null) 
                SoundController.Instance.PlayMusic(SoundController.MusicNames.MenuTheme);

            Board board = Board.Instance;
            if (board != null)
            {
                
                if (board.UIPanel.ResultsPanel != null)
                    board.UIPanel.ResultsPanel.gameObject.SetActive(false);
            }

            SceneManager.UnloadSceneAsync("BackgroundScene");
        }

        public void OnClickPlayButton()
        {
            SceneManager.LoadScene(Board.Instance.NextLevel);
            SceneManager.LoadScene("BackgroundScene", LoadSceneMode.Additive);
        }

        public void OnClickCreditsButton()
        {
            Title.SetActive(false);
            Buttons.SetActive(false);
            CreditsPopup.SetActive(true);
        }

        public void OnClickHelpButton()
        {
            Title.SetActive(false);
            Buttons.SetActive(false);
            HelpPopup.SetActive(true);
        }

        public void OnClickCloseButton()
        {
            Title.SetActive(true);
            Buttons.SetActive(true);
            CreditsPopup.SetActive(false);
            HelpPopup.SetActive(false);
        }

        public void OnClickExitButton()
        {
            Application.Quit();
        }
    }
}
