﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts
{
    public class Board : MonoBehaviour
    {
        public GameObject Trainstation;
        public GameObject Tracks;
        public GameObject card_deck;

        public int HandsCount;
        public BoxSO[] Boxes;
        public Canvas UICanvas;
        public Canvas BoardCanvas;
        public ResultsPanel ResultsPanel;

        public int[] TrainArrivals;

        public TrainHandler StartStation;
        public TrainHandler[] EndStations;
        public int NumberOfTrains = 100;
        public int Score = 0;
        public List<Train> trains;
        
        private static Board _boardInstance;
        public string NextLevel = "Menu_Lea";
        public string CurrentLevel = "Menu_Lea";

        public bool IsRunning;
        public static Board Instance
        {
            get
            {
                if (Board._boardInstance == null)
                    Board._boardInstance = GameObject.FindObjectOfType<Board>();
                return Board._boardInstance;
            }
        }

        public void Start()
        {
            trains = new List<Train>();
            GameObject[] test = GameObject.FindGameObjectsWithTag("CameraBoard");
            if(test.Length > 0) 
            {
                Camera CameraBoard = (Camera)test[0].GetComponent<Camera>();
                
                GameObject[] test2 = GameObject.FindGameObjectsWithTag("CameraMain");
                if (test2.Length > 0)
                {
                    Camera CameraMain = (Camera)test2[0].GetComponent<Camera>();

                    UniversalAdditionalCameraData cameraData = CameraMain.GetUniversalAdditionalCameraData();
                    cameraData.cameraStack.Add(CameraBoard);
                }
            }
            if (SoundController.Instance != null && this.EndStations.Count() > 0) {
                SoundController.Instance.PlayMusic(SoundController.MusicNames.MainTheme);
            }
                
            this.ResetScores();
        }

        public void ResetScores()
        {
            this.Score = 0;
            this.TrainArrivals = new int[this.HandsCount];
            if(this.ResultsPanel != null)
                    this.ResultsPanel.Refresh(null, this.NumberOfTrains, 0);
        }

        public void SendManyTrains(int HowMany)
        {
            if (IsRunning)
                return;
            this.ResetScores();
            IsRunning = true;
            StartCoroutine(this.sendManyTrains(this.NumberOfTrains));
            StartCoroutine(this.WaitTillNoMoreTrain());
        }

        private IEnumerator WaitTillNoMoreTrain()
        {
            yield return new WaitForSeconds(1);
            while (trains.Any())
                yield return 0;
            if (SoundController.Instance != null)
            {
                SoundController.Instance.PlaySound(SoundController.SoundNames.LevelCompletion);
                SoundController.Instance.PlayMusic(SoundController.MusicNames.MainTheme);
            }
            Debug.Log("FINI");
            IsRunning = false;

            GameUI.Instance.FireEndPopup();
        }

        private IEnumerator sendManyTrains(int HowMany)
        {
            for (int i = 0; i < HowMany; i++)
            {
                this.SendTrain();
                yield return i / (float)HowMany;
            }
            yield break;
        }

        public void SendTrain()
        {
            var hand = this.StartStation.DecideOutput();
            if (hand != null || hand.ConnectedTrack == null)
            {
                var train = GameObject.Instantiate<Train>(Resources.Load<Train>("prefabs/train"));
                train.PlaceOnHand(hand);
                this.trains.Add(train);
            }
        }

        public bool IsEnd(TrainHandler box)
        {
            return this.EndStations.Contains(box);
        }

        internal void RegisterTrainArrival(Train train, Hand hand)
        {
            TrainArrivals[hand.Index]++;
            this.Score++;
            this.ResultsPanel.Refresh(TrainArrivals, NumberOfTrains, this.Score);

        }

    }
}
