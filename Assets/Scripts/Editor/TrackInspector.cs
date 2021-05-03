using UnityEngine;
using UnityEditor;
using ScienceGameJam;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;

namespace ScienceGameJam.UnityEditor
{
    [CustomEditor(typeof(Track))]
    public class TrackInspector : Editor
    {
        Track _target;

        public void OnEnable()
        {
            if (target != null)
                _target = (Track)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUI.Button(GUILayoutUtility.GetRect(0, int.MaxValue, 20, 20), "Curve Lines"))
            {
                this._target.CurveLineWithBezier();
            }

            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            if (this._target.line == null)
                return;

            if (PrefabStageUtility.GetCurrentPrefabStage() != null && this._target.TrainHandler == null)
                return;

            if (PrefabStageUtility.GetCurrentPrefabStage() == null && !this._target.line.useWorldSpace)
                return;

            Vector3 offset1 = Vector3.up * 10;

            //get the position of the line point
            Vector3 positionStart = _target.line.GetPosition(0);
            Vector3 positionStop = _target.line.GetPosition(_target.line.positionCount - 1);

            //if the line is object space (!= worldspace) then we have to calculte the world space
            Vector3 calculatedWorldSpacePositionStart = Vector3.zero;
            Vector3 calculatedWorldSpacePositionStop = Vector3.zero;
            if (!this._target.line.useWorldSpace)
            {
                calculatedWorldSpacePositionStart = _target.TrainHandler.transform.TransformPoint(positionStart);
                calculatedWorldSpacePositionStop = _target.TrainHandler.transform.TransformPoint(positionStop);
            }
            

            if (_target.line.positionCount > 1)
            {
                Vector3 positionHandlePositionStart;
                Vector3 positionHandlePositionStop;
                if (_target.line.useWorldSpace)
                {
                    positionHandlePositionStart = positionStart;
                    positionHandlePositionStop = positionStop;
                }
                else
                {
                    positionHandlePositionStart = calculatedWorldSpacePositionStart;
                    positionHandlePositionStop = calculatedWorldSpacePositionStop;
                }

                //get the new world space position of the handle
                Vector3 newPositionHandlePositionStart = Handles.PositionHandle(positionHandlePositionStart, Quaternion.identity);
                Vector3 newPositionHandlePositionStop = Handles.PositionHandle(positionHandlePositionStop, Quaternion.identity);

                //if the line is object space (!= worldspace) then we have to calculte the object space to set the new position
                Vector3 newCalculatedObjectSpacePositionStart = Vector3.zero;
                Vector3 newCalculatedObjectSpacePositionStop = Vector3.zero;
                if (!this._target.line.useWorldSpace)
                {
                    newCalculatedObjectSpacePositionStart = _target.TrainHandler.transform.InverseTransformPoint(newPositionHandlePositionStart);
                    newCalculatedObjectSpacePositionStop = _target.TrainHandler.transform.InverseTransformPoint(newPositionHandlePositionStop);
                }

                Vector3 newPositionStart;
                Vector3 newPositionStop;
                if (_target.line.useWorldSpace)
                {
                    newPositionStart = newPositionHandlePositionStart;
                    newPositionStop = newPositionHandlePositionStop;
                }
                else
                {
                    newPositionStart = newCalculatedObjectSpacePositionStart;
                    newPositionStop = newCalculatedObjectSpacePositionStop;
                }

                HandleUtility.AddControl(0, 1);
                HandleUtility.AddControl(1, 1);

                Handles.color = Color.magenta;
                Handles.SphereHandleCap(0, positionHandlePositionStart, Quaternion.identity, Board.Instance.EditorPreset.TrackBallSize, EventType.Repaint);
                Handles.SphereHandleCap(1, positionHandlePositionStop, Quaternion.identity, Board.Instance.EditorPreset.TrackBallSize, EventType.Repaint);
                Vector3 diff = newPositionStop - _target.line.GetPosition(_target.line.positionCount - 2);

                if (newPositionStart != positionStart)
                {
                    this._target.Disconnect(true);

                    bool snapped = false;
                    //detect if another handle is nearby ?
                    if (this._target.TrainHandler != null)
                    {
                        if (this._target.TrainHandler.TrainHandlerType == TrainHandlerType.Card)
                        {
                            //if the trainhandler is a card, it means that we should stick to the hand of the card only !
                            foreach(Hand hand in this._target.TrainHandler.AllHands)
                            {
                                //if the the hand is not already occupied
                                if(hand.TrainHandler != null)
                                {
                                    float dist;
                                    dist = ((Vector2)hand.transform.position - (Vector2)newPositionHandlePositionStart).magnitude;
                                    if (dist < Board.Instance.EditorPreset.HandSnapDistance)
                                    {
                                        Undo.RecordObject(this._target, "TrackSnap change");
                                        //Debug.Log(dist + " " + hand);
                                        this._target.ConnectToHand(hand, true, true);
                                        snapped = true;

                                        /*SerializedObject obj = new SerializedObject(this._target);
                                        obj.FindProperty("_handAtBeginning").objectReferenceValue = hand;
                                        obj.ApplyModifiedProperties();
                                        Undo.RecordObject(this._target, "Changing the component on myObject");*/
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //if there is not train handler, it means that we should stick to the hand of a box only !
                        foreach(Box box in Board.Instance.Boxes)
                        {
                            foreach (Hand hand in box.AllHands)
                            {
                                //if the the hand is not already occupied
                                if (hand.TrainHandler != null)
                                {
                                    float dist = ((Vector2)hand.transform.position - (Vector2)newPositionStart).magnitude;
                                    if (dist < Board.Instance.EditorPreset.HandSnapDistance)
                                    {
                                        Undo.RecordObject(this._target, "TrackSnap change");
                                        //Debug.Log(dist + " " + hand);
                                        this._target.ConnectToHand(hand, true, true);
                                        snapped = true;
                                    }
                                }
                            }
                        }                        
                    }

                    if(!snapped)
                        this._target.line.SetPosition(0, newPositionStart);
                    else
                    {
                        EditorUtility.SetDirty(this._target.gameObject);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(this._target);
                        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                        {
                            //EditorSceneManager.MarkSceneDirty(this._target.gameObject.scene); //This used to happen automatically from SetDirty
                        }
                    }
                        
                }
                    
                if (newPositionStop != positionStop)
                {
                    this._target.Disconnect(false);

                    bool snapped = false;
                    //detect if another handle is nearby ?
                    if (this._target.TrainHandler != null)
                    {
                        if (this._target.TrainHandler.TrainHandlerType == TrainHandlerType.Card)
                        {
                            //if the trainhandler is a card, it means that we should stick to the hand of the card only !
                            foreach (Hand hand in this._target.TrainHandler.AllHands)
                            {
                                //if the the hand is not already occupied
                                if (hand.TrainHandler != null)
                                {
                                    float dist;
                                    dist = ((Vector2)hand.transform.position - (Vector2)newPositionHandlePositionStop).magnitude;
                                    if (dist < Board.Instance.EditorPreset.HandSnapDistance)
                                    {
                                        Undo.RecordObject(this._target, "TrackSnap change");
                                        //Debug.Log(dist + " " + hand);
                                        this._target.ConnectToHand(hand, false, true);
                                        snapped = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //if there is not train handler, it means that we should stick to the hand of a box only !
                        foreach (Box box in Board.Instance.Boxes)
                        {
                            if(box != null)
                            {
                                foreach (Hand hand in box.AllHands)
                                {
                                    //if the the hand is not already occupied
                                    if (hand.TrainHandler != null)
                                    {
                                        float dist = ((Vector2)hand.transform.position - (Vector2)newPositionStop).magnitude;
                                        if (dist < Board.Instance.EditorPreset.HandSnapDistance)
                                        {
                                            Undo.RecordObject(this._target, "TrackSnap change");
                                            //Debug.Log(dist + " " + hand);
                                            this._target.ConnectToHand(hand, false, true);
                                            snapped = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!snapped)
                        this._target.line.SetPosition(this._target.line.positionCount - 1, newPositionStop);
                    else
                    {
                        EditorUtility.SetDirty(this._target.gameObject);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(this._target);
                        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                        {
                            //EditorSceneManager.MarkSceneDirty(this._target.gameObject.scene); //This used to happen automatically from SetDirty
                        }
                    }
                }
            }
        }
    }
}