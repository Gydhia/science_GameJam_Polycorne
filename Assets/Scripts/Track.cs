using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ScienceGameJam
{
    [ExecuteInEditMode]
    public class Track : MonoBehaviour
    {
        public LineRenderer line;
        public int vertexCount = 10;
        public float radiusCut = 20f;

        public List<Train> CurrentCrossingTrains;
        public TrainHandler TrainHandler;

        [SerializeField]
        private Hand _handAtBeginning;
        public Hand HandAtBeginning
        {
            get
            {
                return _handAtBeginning;
            }
        }

        [SerializeField]
        private Hand _handAtEnd;
        public Hand HandAtEnd
        {
            get
            {
                return _handAtEnd;
            }
        }

        public event NavigationNotification OnTrainArrivedAtEnd; // event
        public event NavigationNotification OnTrainArrivedAtStart; // event


        public void Start()
        {
            line = GetComponent<LineRenderer>();
            CurrentCrossingTrains = new List<Train>();
        }

        public bool Disconnect(bool beginning)
        {
            bool ok = false;

            /*if (TrainHandler != null)
                TrainHandler.UnregisterTrack(this);*/

            if (beginning)
            {
                if (_handAtBeginning != null)
                {
                    _handAtBeginning.UnregisterTrack();
                    _handAtBeginning = null;
                }
            }
            else
            {
                if (_handAtEnd != null)
                {
                    _handAtEnd.UnregisterTrack();
                    _handAtEnd = null;
                }
            }


            return ok;
        }

        public bool ConnectToHand(Hand hand, bool beginning, bool force = false)
        {
            bool ok = false;

            if (hand == null)
                return ok;

            hand.RegisterTrack(this, !beginning);
            if (beginning)
                _handAtBeginning = hand;
            else
                _handAtEnd = hand;

            /*if (hand.TrainHandler != null)
                TrainHandler.RegisterTrack(this);*/

            hand.SnapTrack(force);

            return ok;
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (this.line == null)
                    return;

                if (this.line.GetComponent<Track>() == null || this.line.GetComponent<Track>().TrainHandler == null)
                {
                    this.line.sharedMaterial.color = Color.red;
                    return;
                }

                if(this.HandAtEnd != null && this.HandAtBeginning != null)
                    this.line.sharedMaterial.color = Color.green;
                else
                    this.line.sharedMaterial.color = Color.yellow;

                Vector3 offset1 = Vector3.up * 10;
                Vector3 offset2 = Vector3.up * 10 + Vector3.left * 100;

                Handles.Label(this.line.GetPosition(0) + offset1, name + " START", new GUIStyle() { fontSize = 12, });
                Handles.Label(this.line.GetPosition(this.line.positionCount - 1) + offset2, name + " END", new GUIStyle() { fontSize = 12 });


                //draw the number of train label
                GUIContent content = new GUIContent(CurrentCrossingTrains.Count.ToString());
                GUIStyle style = new GUIStyle(GUI.skin.box);

                style.alignment = TextAnchor.MiddleCenter;
                Vector2 size = style.CalcSize(content);

                int position = this.line.positionCount / 2;
                Vector3 pos = this.line.GetPosition(position);
                if (!this.line.useWorldSpace)
                {
                    Track track = this.line.gameObject.GetComponent<Track>();
                    if (track != null && track.TrainHandler != null)
                        track.TrainHandler.transform.TransformPoint(pos);
                    else
                        return;
                }
                    
                if (this.line.positionCount == 2)
                {
                    pos = (line.GetPosition(1) + line.GetPosition(0)) / 2;
                }

                Handles.BeginGUI();

                Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos) - Vector2.up * 30;
                GUI.Box(new Rect(pos2D.x - 10, pos2D.y - 10, 2.0f * size.x, size.y + 10), content, style);

                Handles.EndGUI();
            }
        }
#endif

        internal void StartWatchingTrain(Train train)
        {
            //train.OnArrivedAtEndOfTracks += Train_OnArrivedAtEndOfTracks;
            //train.OnArrivedAtBeginningOfTracks += Train_OnArrivedAtStartOfTracks;
        }

        public void Train_OnArrivedAtEndOfTracks(Train Train)
        {
            CurrentCrossingTrains.Remove(Train);
            if (TrainHandler != null)
                TrainHandler.CurrentCrossingTrains.Remove(Train);
            StopWatchingTrain(Train);

            // forward event to whomever is connected to this track
            if (OnTrainArrivedAtEnd != null)
                OnTrainArrivedAtEnd.Invoke(Train, HandAtEnd);
        }
        public void Train_OnArrivedAtStartOfTracks(Train Train)
        {
            CurrentCrossingTrains.Remove(Train);
            if (TrainHandler != null)
                TrainHandler.CurrentCrossingTrains.Remove(Train);
            StopWatchingTrain(Train);
            // forward event to whomever is connected to this track
            if (OnTrainArrivedAtStart != null)
                OnTrainArrivedAtStart.Invoke(Train, HandAtBeginning);
        }

        internal void StopWatchingTrain(Train train)
        {
            //train.OnArrivedAtEndOfTracks -= Train_OnArrivedAtEndOfTracks;
            //train.OnArrivedAtEndOfTracks -= Train_OnArrivedAtStartOfTracks;
        }
        public void CurveLineWithBezier()
        {
            Vector3[] linePoints = new Vector3[line.positionCount];
            line.GetPositions(linePoints);

            if (linePoints.Length <= 2) return;

            List<Vector3> preparedLinePoints = new List<Vector3>();

            preparedLinePoints.Add(linePoints[0]);
            for (int i = 1; i < linePoints.Length - 1; i++)
            {
                float mag1 = Vector3.Magnitude(linePoints[i] - linePoints[i - 1]);
                preparedLinePoints.Add(Vector3.Lerp(linePoints[i - 1], linePoints[i], (mag1 - radiusCut) / mag1));


                preparedLinePoints.Add(linePoints[i]);

                float mag2 = Vector3.Magnitude(linePoints[i] - linePoints[i + 1]);
                preparedLinePoints.Add(Vector3.Lerp(linePoints[i], linePoints[i + 1], radiusCut / mag2));
            }
            preparedLinePoints.Add(linePoints[linePoints.Length - 1]);


            List<Vector3> finalPointList = new List<Vector3>();
            finalPointList.Add(preparedLinePoints.First());
            for (int i = 1; i < preparedLinePoints.Count - 2; i += 3)
            {
                Vector3 point1 = preparedLinePoints[i];
                Vector3 point2 = preparedLinePoints[i + 1];
                Vector3 point3 = preparedLinePoints[i + 2];

                List<Vector3> pointList = new List<Vector3>();
                for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
                {
                    var tangentLine1 = Vector3.Lerp(point1, point2, ratio);
                    var tangentLine2 = Vector3.Lerp(point2, point3, ratio);
                    var bezierPoint = Vector3.Lerp(tangentLine1, tangentLine2, ratio);
                    pointList.Add(bezierPoint);
                }
                finalPointList.AddRange(pointList);
            }
            finalPointList.Add(preparedLinePoints.Last());

            line.positionCount = finalPointList.Count();
            line.SetPositions(finalPointList.ToArray());
        }

        public void RegisterTrainHandler(TrainHandler trainHandler)
        {
            if (this.TrainHandler != trainHandler)
                this.TrainHandler = trainHandler;
        }
    }
}