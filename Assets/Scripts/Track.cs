using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{
    public LineRenderer line { get; set; }
    public int vertexCount = 10;
    public float radiusCut = 20f;

    public Hand HandAtBeginning;
    public Hand HandAtEnd;

    public event NavigationNotification OnTrainArrivedAtEnd; // event
    public event NavigationNotification OnTrainArrivedAtStart; // event


    public void Start()
    {
        this.line = this.GetComponent<LineRenderer>();

    }

    public void OnDrawGizmos()
    {
        var _line = this.GetComponent<LineRenderer>();
        Vector3 offset1 = Vector3.up * 10;
        Vector3 offset2 = Vector3.up * 10 + Vector3.left * 100;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(_line.GetPosition(0) + offset1 , this.name + " START", new GUIStyle() { fontSize = 12 });
        UnityEditor.Handles.Label(_line.GetPosition(_line.positionCount - 1) + offset2, this.name + " END", new GUIStyle() { fontSize = 12 });
#endif
    }

    internal void StartWatchingTrain(Train train)
    {
        //train.OnArrivedAtEndOfTracks += Train_OnArrivedAtEndOfTracks;
        //train.OnArrivedAtBeginningOfTracks += Train_OnArrivedAtStartOfTracks;
    }

    public void Train_OnArrivedAtEndOfTracks(Train Train)
    {
        this.StopWatchingTrain(Train);
        // forward event to whomever is connected to this track
        if (this.OnTrainArrivedAtEnd != null)
            this.OnTrainArrivedAtEnd.Invoke(Train, this.HandAtEnd);
    }
    public void Train_OnArrivedAtStartOfTracks(Train Train)
    {
        this.StopWatchingTrain(Train);
        // forward event to whomever is connected to this track
        if (this.OnTrainArrivedAtStart != null)
            this.OnTrainArrivedAtStart.Invoke(Train, this.HandAtBeginning);
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
        for (int i = 1; i < linePoints.Length - 1; i++) {
            float mag1 = Vector3.Magnitude(linePoints[i] - linePoints[i - 1]);
            preparedLinePoints.Add(Vector3.Lerp(linePoints[i - 1], linePoints[i],(mag1 - radiusCut) / mag1));


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
}