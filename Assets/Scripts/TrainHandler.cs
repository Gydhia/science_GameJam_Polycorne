using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

public abstract class TrainHandler : MonoBehaviour
{
    /// <summary>
    /// Define if the tracks shoud be auto-snapped to the hands
    /// </summary>
    public bool AreTracksAutoSnapped = false;

    /// <summary>
    /// 
    /// </summary>
    public abstract TrainHandlerType TrainHandlerType
    {
        get;
    }

    

    [Header("Hand autocreation elements")]
    public Hand HandPrefab;
    public Transform HandsContainer;

    [Header("Hands instances")]
    public Hand[] HandsLeft;
    public Hand[] HandsRight;
    public IEnumerable<Hand> AllHands => this.HandsLeft.Concat(this.HandsRight);

    [Header("Runtime values")]
    public List<Train> CurrentCrossingTrains;
    public List<Track> Tracks;
    public System.Random rand;

    public event TrainHandlerNavigationEventData.Event OnTrainExited; // event
    public event TrainHandlerNavigationEventData.Event OnTrainEntered; // event


    public void Awake()
    {
        if (this.Tracks == null)
            this.Tracks = new List<Track>();

        if (this.HandsLeft == null)
            this.HandsLeft = new Hand[Board.Instance.HandsCount];

        if (this.HandsRight == null)
            this.HandsRight = new Hand[Board.Instance.HandsCount];

        foreach(Hand hand in this.AllHands)
        {
            hand.RegisterTrainHandler(this);
            if (hand.ConnectedTrack != null)
            {
                // register only tracks that belong to this trainhandler
                if(!hand.ConnectedTrack.IsWorldTrack)
                    this.RegisterTrack(hand.ConnectedTrack);

                // but always register to track that are either connected to this trainhandler or belong to this train handler
                /*hand.ConnectedTrack.OnTrainEntered += _onTrainEntered;
                hand.ConnectedTrack.OnTrainExited += _onTrainExited;*/
            }
        }
    }

    public virtual void Start()
    {
        this.CurrentCrossingTrains = new List<Train>();

        this.rand = new System.Random();
    }

    public void RegisterTrack(Track track)
    {
        if (track != null && !this.Tracks.Contains(track))
            this.Tracks.Add(track);

        if (track.TrainHandler != this)
            track.RegisterTrainHandler(this);
    }

    public void UnregisterTrack(Track track)
    {
        if (track != null && this.Tracks.Contains(track))
            this.Tracks.Remove(track);

        if (track.TrainHandler == this)
            track.UnregisterTrainHandler(this);
    }

    private void _onTrainExited(TrackNavigationEventData data)
    {
        throw new NotImplementedException();
    }

    private void _onTrainEntered(TrackNavigationEventData data)
    {
        throw new NotImplementedException();
    }


    public void FireTrainEntered(Train train, Hand viaHand)
    {
        bool isTrainAllowedToEnter = this._fireTrainEntered(train, viaHand);

        if (isTrainAllowedToEnter)
        {
            if (train != null && !this.CurrentCrossingTrains.Contains(train))
                this.CurrentCrossingTrains.Add(train);
            train.currentTrainHandler = this;

            if (this.OnTrainEntered != null)
                this.OnTrainEntered.Invoke(new TrainHandlerNavigationEventData(train, this, viaHand));
        }
    }

    protected abstract bool _fireTrainEntered(Train train, Hand viaHand);

    public virtual void FireTrainExited(Train train, Hand viaHand)
    {
        this.CurrentCrossingTrains.Remove(train);

        this._fireTrainExited(train, viaHand);

        train.currentTrainHandler = null;

        if (this.OnTrainExited != null)
            this.OnTrainExited.Invoke(new TrainHandlerNavigationEventData(train, this, viaHand));
    }

    protected abstract void _fireTrainExited(Train train, Hand viaHand);

    public void GenerateHandsOwner()
    {
        foreach (Hand hand in this.AllHands)
        {
            hand.RegisterTrainHandler(this);
        }
    }

    public abstract void RegenerateHands();

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (Event.current.type == EventType.Repaint)
        {
            // display handler name
            GUIContent contentName = new GUIContent(this.name);
            GUIStyle styleName = new GUIStyle(GUI.skin.box);
            styleName.alignment = TextAnchor.MiddleCenter;
            Vector2 sizeName = styleName.CalcSize(contentName);

            Handles.BeginGUI();
            Vector3 posName = this.transform.position;
            posName.y += this.transform.GetComponent<RectTransform>().rect.height / 2f;
            Vector2 pos2DName = HandleUtility.WorldToGUIPoint(posName) - Vector2.up * 40;
            GUI.Box(new Rect(pos2DName.x - (sizeName.x + 20) / 2.0f, pos2DName.y - 10, sizeName.x + 20, sizeName.y + 10), contentName, styleName);
            Handles.EndGUI();

            if (Application.isPlaying
                && PrefabStageUtility.GetCurrentPrefabStage() == null && this.TrainHandlerType != TrainHandlerType.Card)
            {
                // draw the number of train label
                GUIContent content = new GUIContent(this.CurrentCrossingTrains.Count.ToString());
                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.alignment = TextAnchor.MiddleCenter;
                Vector2 size = style.CalcSize(content);

                Vector3 pos = this.transform.position;
                pos.y += this.transform.GetComponent<RectTransform>().rect.height / 2f;
                Handles.BeginGUI();
                Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos) - Vector2.up * 70;
                GUI.Box(new Rect(pos2D.x - 10, pos2D.y - 10, 2.0f * size.x, size.y + 10), content, style);
                Handles.EndGUI();
            }
        }
    }
#endif
}

public enum TrainHandlerType
{
    Card = 0,
    Box = 1
}
