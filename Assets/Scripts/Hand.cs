#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using ScienceGameJam;

public class Hand : MonoBehaviour
{
    public TrainHandler TrainHandler;

    [SerializeField]
    private Track _connectedTrack;
    public Track ConnectedTrack
    {
        get
        {
            return this._connectedTrack;
        }
    }

    [SerializeField]
    private bool _connectEndOfTrack = false;
    public bool ConnectEndOfTrack
    {
        get
        {
            return this._connectEndOfTrack;
        }
    }

    [SerializeField]
    private Vector3 _worldPosition;
    public int Index;
    public bool LeftHand = false;

    public void Update()
    {
        this._worldPosition = this.transform.position;
        //this.SnapTrack();
        //this.GenerateConnectedTrack();
    }

    public void UnregisterTrack()
    {
        this._connectedTrack = null;
    }

    public void RegisterTrack(Track track, bool connectToEnd)
    {
        if (track == null)
            return;

        this._connectedTrack = track;
        this._connectEndOfTrack = connectToEnd;
    }

    public void RegisterTrainHandler(TrainHandler trainHandler)
    {
        this.TrainHandler = trainHandler;
    }

    public void GenerateConnectedTrack()
    {
        if (this.ConnectedTrack != null)
        {
            this.ConnectedTrack.ConnectToHand(this, ConnectEndOfTrack);
        }
    }

    public void SnapTrack(bool force = false)
    {
        if (this.ConnectedTrack != null)
        {
            if (ConnectEndOfTrack)
            {
                if ((this.TrainHandler != null && this.TrainHandler.AreTracksAutoSnapped) || force) {
                    Vector3 newPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.ConnectedTrack.line.GetPosition(this.ConnectedTrack.line.positionCount - 1).z);
                    if (!this.ConnectedTrack.line.useWorldSpace)
                    {
                        newPosition = this.TrainHandler.transform.InverseTransformPoint(newPosition);
                        newPosition.z = 0;
                    }
                        

                    this.ConnectedTrack.line.SetPosition(this.ConnectedTrack.line.positionCount - 1, newPosition);
                }
            }
            else
            {
                if ((this.TrainHandler != null && this.TrainHandler.AreTracksAutoSnapped) || force)
                {
                    Vector3 newPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.ConnectedTrack.line.GetPosition(this.ConnectedTrack.line.positionCount - 1).z);
                    if (!this.ConnectedTrack.line.useWorldSpace)
                    {
                        newPosition = this.TrainHandler.transform.InverseTransformPoint(newPosition);
                        newPosition.z = 0;
                    }
                        

                    this.ConnectedTrack.line.SetPosition(0, newPosition);
                }
                    
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (Board.Instance == null || Board.Instance.EditorPreset == null)
                return;

            if (this.TrainHandler == null)
            {
                Handles.color = Color.red;
            }
            else
            {
                if (this.TrainHandler != null && this.ConnectedTrack != null)
                    Handles.color = Color.green;
                else if (this.TrainHandler.TrainHandlerType == TrainHandlerType.Box)
                    Handles.color = Color.blue;
                else if (this.TrainHandler.TrainHandlerType == TrainHandlerType.Card)
                    Handles.color = Color.cyan;               
            }

            Handles.SphereHandleCap(0, this.transform.position, this.transform.rotation, Board.Instance.EditorPreset.HandBallSize, EventType.Repaint);
            Handles.CircleHandleCap(0, this.transform.position, this.transform.rotation, Board.Instance.EditorPreset.HandSnapDistance, EventType.Repaint);
        }
    }
#endif
}
