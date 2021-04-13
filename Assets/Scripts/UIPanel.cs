using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIPanel : MonoBehaviour
    {
        public Board Board;
        public ResultsPanel ResultsPanel;

        public void Awake()
        {
            this.Board = GameObject.FindObjectOfType<Board>();

            ResultsPanel ResultsPanel = GameObject.FindObjectOfType<ResultsPanel>(true);
            if (ResultsPanel != null)
            {
                this.ResultsPanel = ResultsPanel;
                if (this.Board != null)
                {
                    this.Board.ResultsPanel = ResultsPanel;
                    this.Board.ResultsPanel.gameObject.SetActive(false);
                }
            }
        }

        public void Start()
        {
            //this is a security when we start directly the level from the editor (scene are not loaded in the same order sometinme)
            if(this.Board == null)
            {
                this.Board = GameObject.FindObjectOfType<Board>();

                ResultsPanel ResultsPanel = GameObject.FindObjectOfType<ResultsPanel>(true);
                if (ResultsPanel != null)
                {
                    this.ResultsPanel = ResultsPanel;
                    if (this.Board != null)
                    {
                        this.Board.ResultsPanel = ResultsPanel;
                        this.Board.ResultsPanel.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void ClickOnStart(int train)
        {
            if(SoundController.Instance != null) {
                SoundController.Instance.PlaySound(SoundController.SoundNames.Play);
                SoundController.Instance.PlayMusic(SoundController.MusicNames.ActionTheme);
            }
            this.Board.SendManyTrains(train);
            this.GetComponent<GameUI>().StartTimeline();
        }
    }
}
