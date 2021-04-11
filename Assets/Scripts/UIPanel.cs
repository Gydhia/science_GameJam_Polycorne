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

        public void Start()
        {
            this.Board = GameObject.FindObjectOfType<Board>();

            GameObject[] test3 = GameObject.FindGameObjectsWithTag("ResultsPanel");
            if (test3.Length > 0)
            {
                ResultsPanel ResultsPanel = test3[0].GetComponent<ResultsPanel>();

                this.ResultsPanel = ResultsPanel;
                this.Board.ResultsPanel = ResultsPanel;
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
