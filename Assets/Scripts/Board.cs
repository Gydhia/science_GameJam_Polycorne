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
        public GameObject TrainstationsContainer;
        public GameObject TracksContainer;
        public GameObject CardDeckContainer;
        public GameObject TrainsContainer;

        public int HandsCount;
        public List<Box> Boxes;
        public List<Card> Cards;

        public Canvas UICanvas;
        public Canvas BoardCanvas;
        public ResultsPanel ResultsPanel;

        public int[] TrainArrivals;

        public TrainHandler StartStation;
        public TrainHandler[] EndStations;
        public TrainHandler[] FailStations;
        public int NumberOfTrains = 100;
        public int Score = 0;
        public List<Train> Trains;
        
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
            this.Trains = new List<Train>();
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
                    if (CameraBoard.GetComponent<AudioListener>() != null)
                        CameraBoard.GetComponent<AudioListener>().enabled = false;
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

            if (this.ResultsPanel != null)
                this.ResultsPanel.gameObject.SetActive(true);

            //disable dragndropon all cards !
            foreach (DragnDrop drag in GameObject.FindObjectsOfType<DragnDrop>())
            {
                drag.DragEnabled = false;
            }
            foreach (CardSpace card in GameObject.FindObjectsOfType<CardSpace>())
            {
                card.DropEnabled = false;
            }

            this.ResetScores();
            IsRunning = true;
            StartCoroutine(this.sendManyTrains(this.NumberOfTrains));
            StartCoroutine(this.WaitTillNoMoreTrain());
        }

        private IEnumerator WaitTillNoMoreTrain()
        {
            yield return new WaitForSeconds(1);
            while (Trains.Any())
                yield return 0;
            if (SoundController.Instance != null)
            {
                SoundController.Instance.PlaySound(SoundController.SoundNames.LevelCompletion);
                SoundController.Instance.PlayMusic(SoundController.MusicNames.MainTheme);
            }
            Debug.Log("FINI");
            IsRunning = false;

            //enable dragndropon all cards !
            foreach (DragnDrop drag in GameObject.FindObjectsOfType<DragnDrop>())
            {
                drag.DragEnabled = true;
            }
            foreach (CardSpace card in GameObject.FindObjectsOfType<CardSpace>())
            {
                card.DropEnabled = true;
            }

            GameUI.Instance.FireEndPopup();
        }

        private IEnumerator sendManyTrains(int HowMany)
        {
            /*while (true)
            {
                this.SendTrain();
                yield return new WaitForSecondsRealtime(0.06f);
            }*/
            for (int i = 0; i < HowMany; i++)
            {
                this.SendTrain();
                yield return new WaitForSecondsRealtime(0.06f);
                yield return i / (float)HowMany;
            }
            yield break;
        }

        public void SendTrain()
        {
            var hand = this.StartStation.DecideOutput();
            if (hand != null || hand.ConnectedTrack == null)
            {
                Train train = GameObject.Instantiate<Train>(Resources.Load<Train>("prefabs/train"), this.TrainsContainer.transform);
                train.PlaceOnHand(hand);
                this.Trains.Add(train);
            }
        }

        public bool IsEnd(TrainHandler box)
        {
            if (this.EndStations == null)
            {
                Debug.LogWarning("There is no end station !!!!");
                return false;
            }
            
            return this.EndStations.Contains(box);
        }

        public bool IsFail(TrainHandler box)
        {
            if (this.FailStations == null)
                return false;

            return this.FailStations.Contains(box);
        }

        internal void RegisterTrainArrival(Train train, Hand hand, TrainHandler trainHandler)
        {
            TrainArrivals[hand.Index]++;
            this.Score++;
            this.ResultsPanel.Refresh(TrainArrivals, NumberOfTrains, this.Score);

            if(trainHandler is Box)
            {
                Box box = trainHandler as Box;
                if (box.WinningGare != null)
                    box.WinningGare.Animator.SetTrigger("Hit");
            }
        }

        internal void RegisterBox(Box box)
        {
            if (this.Boxes == null)
                this.Boxes = new List<Box>();

            this.Boxes.Add(box);
        }

        internal void RegisterCard(Card card)
        {
            if (this.Cards == null)
                this.Cards = new List<Card>();

            this.Cards.Add(card);
        }

    }
}
