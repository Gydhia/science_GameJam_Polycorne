using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
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

        public bool IsWorldTrack
        {
            get
            {
                return this.TrainHandler == null;
            }
        }

        public event TrackNavigationEventData.Event OnTrainExited; // event
        public event TrackNavigationEventData.Event OnTrainEntered; // event


        public void Start()
        {
            line = GetComponent<LineRenderer>();
            this.CurrentCrossingTrains = new List<Train>();

            if(this.HandAtBeginning != null)
                this.HandAtBeginning.RegisterTrack(this, false);

            if (this.HandAtEnd != null)
                this.HandAtEnd.RegisterTrack(this, true);

            // reset track color
            if (Application.isPlaying && this.line.material != Board.Instance.EditorPreset.MaterialTrack)
            {
                this.line.material = Board.Instance.EditorPreset.MaterialTrack;
            }
        }

        public void Update()
        {
            if(this.HandAtEnd != null)
                this.HandAtEnd.SnapTrack();

            if (this.HandAtBeginning != null)
                this.HandAtBeginning.SnapTrack();
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
                this._handAtBeginning = hand;
            else
                this._handAtEnd = hand;

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

                if (!Application.isPlaying)
                {
                    if (PrefabStageUtility.GetCurrentPrefabStage() != null && this.TrainHandler == null)
                    {
                        this.line.material = Board.Instance.EditorPreset.MaterialTrackError;
                        return;
                    }

                    if (this.HandAtEnd != null && this.HandAtBeginning != null)
                        this.line.material = Board.Instance.EditorPreset.MaterialTrackValid;
                    else
                        this.line.material = Board.Instance.EditorPreset.MaterialTrackNotConnected;
                }

                Vector3 offset1 = Vector3.up * 10;
                Vector3 offset2 = Vector3.up * 10 + Vector3.left * 100;

                // if we're not on a prefab and the track is a world track
                // 
                if (((PrefabStageUtility.GetCurrentPrefabStage() != null && !this.IsWorldTrack)
                    || (PrefabStageUtility.GetCurrentPrefabStage() == null && this.IsWorldTrack)))
                {
                    // display beginning of track
                    GUIContent contentBegin = new GUIContent(name + " START");
                    GUIStyle styleBegin = new GUIStyle(GUI.skin.box);
                    styleBegin.alignment = TextAnchor.MiddleCenter;

                    Vector2 sizeBegin = styleBegin.CalcSize(contentBegin);
                    Vector3 posBegin = this.line.GetPosition(0);
                    if (!this.IsWorldTrack)
                        this.TrainHandler.transform.TransformPoint(posBegin);

                    Handles.BeginGUI();
                    Vector2 pos2DBegin = HandleUtility.WorldToGUIPoint(posBegin) - Vector2.up * 30;
                    GUI.Box(new Rect(pos2DBegin.x - (sizeBegin.x + 20) / 2.0f, pos2DBegin.y - 10, sizeBegin.x + 20, sizeBegin.y + 10), contentBegin, styleBegin);
                    Handles.EndGUI();


                    // display end of track
                    GUIContent contentEnd = new GUIContent(name + " END");
                    GUIStyle styleEnd = new GUIStyle(GUI.skin.box);
                    styleEnd.alignment = TextAnchor.MiddleCenter;

                    Vector2 sizeEnd = styleEnd.CalcSize(contentEnd);
                    Vector3 posEnd = this.line.GetPosition(this.line.positionCount - 1);
                    if (!this.IsWorldTrack)
                        this.TrainHandler.transform.TransformPoint(posEnd);

                    Handles.BeginGUI();
                    Vector2 pos2DEnd = HandleUtility.WorldToGUIPoint(posEnd) - Vector2.up * 30;
                    GUI.Box(new Rect(pos2DEnd.x - (sizeEnd.x + 20)/2.0f, pos2DEnd.y - 10, sizeEnd.x + 20, sizeEnd.y + 10), contentEnd, styleEnd);
                    Handles.EndGUI();

                    //Handles.Label(this.line.GetPosition(0) + offset1, name + " START", new GUIStyle() { fontSize = 12, });
                    //Handles.Label(this.line.GetPosition(this.line.positionCount - 1) + offset2, name + " END", new GUIStyle() { fontSize = 12 });
                }

                if (Application.isPlaying
                    && PrefabStageUtility.GetCurrentPrefabStage() == null && this.IsWorldTrack)
                {
                    // draw the number of train label
                    GUIContent content = new GUIContent(this.CurrentCrossingTrains.Count.ToString());
                    GUIStyle style = new GUIStyle(GUI.skin.box);

                    style.alignment = TextAnchor.MiddleCenter;
                    Vector2 size = style.CalcSize(content);

                    int position = this.line.positionCount / 2;
                    Vector3 pos = this.line.GetPosition(position);
                    if (!this.IsWorldTrack)
                        this.TrainHandler.transform.TransformPoint(pos);

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
        }
#endif

        internal void StartWatchingTrain(Train train)
        {
            //train.OnArrivedAtEndOfTracks += Train_OnArrivedAtEndOfTracks;
            //train.OnArrivedAtBeginningOfTracks += Train_OnArrivedAtStartOfTracks;
        }

        public void FireTrainExited(Train train, Hand viaHand)
        {
            this.CurrentCrossingTrains.Remove(train);
            train.currentTrack = null;

            // if the track is not a world track: it means that it is part of a trainhandler
            // therefor: notify it
            if (!this.IsWorldTrack)
            {
                this.TrainHandler.FireTrainExited(train, viaHand);
            }
            // it is a world, so no trainhandler to warn about exit. But
            else
            {
                viaHand.TrainHandler.FireTrainEntered(train, viaHand);
            }

            if (this.OnTrainExited != null)
                this.OnTrainExited.Invoke(new TrackNavigationEventData(train, this, viaHand));
        }

        public void FireTrainEntered(Train train, Hand viaHand)
        {
            if(train != null && !this.CurrentCrossingTrains.Contains(train))
                this.CurrentCrossingTrains.Add(train);
            train.currentTrack = this;

            train.Previoushand = viaHand;
            if (this.HandAtBeginning == viaHand)
                train.NextHand = this.HandAtEnd;
            else
                train.NextHand = this.HandAtBeginning;

            if (this.TrainHandler != null)
                this.TrainHandler.FireTrainEntered(train, viaHand);

            if (this.OnTrainEntered != null)
                this.OnTrainEntered.Invoke(new TrackNavigationEventData(train, this, viaHand));
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

            if (this.TrainHandler != null && !this.TrainHandler.Tracks.Contains(this))
                this.TrainHandler.RegisterTrack(this);
        }

        public void UnregisterTrainHandler(TrainHandler trainHandler)
        {
            if (this.TrainHandler != null && this.TrainHandler.Tracks.Contains(this))
                this.TrainHandler.UnregisterTrack(this);

            if (this.TrainHandler == trainHandler)
                this.TrainHandler = null;
        }
    }
}