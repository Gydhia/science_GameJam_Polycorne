using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Board : MonoBehaviour
    {
        public int HandsCount;
        public BoxSO[] Boxes;
        public Canvas UICanvas;
        public ResultsPanel ResultsPanel;

        public int[] TrainArrivals;

        public TrainHandler StartStation;
        public TrainHandler EndStation;
        public int NumberOfTrains = 100;

        private static Board _boardInstance;
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
            this.ResetScores();
        }

        public void ResetScores()
        {
            this.TrainArrivals = new int[this.HandsCount];
            this.ResultsPanel.Refresh(null, this.NumberOfTrains);
        }

        public void SendManyTrains(int HowMany)
        {
            this.ResetScores();
            StartCoroutine(this.sendManyTrains(100));
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
            var train = GameObject.Instantiate<Train>(Resources.Load<Train>("prefabs/train"));
            int trackID = this.StartStation.DecideOutput();
            train.PlaceOnHand(this.StartStation.HandsRight.ElementAt(trackID));
        }


        internal void RegisterTrainArrival(Train train, Hand hand)
        {
            TrainArrivals[hand.Index]++;
            this.ResultsPanel.Refresh(TrainArrivals, NumberOfTrains);
        }

    }
}
