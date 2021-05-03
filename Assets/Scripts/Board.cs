using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ScienceGameJam.Mechanics.Preset;
using Assets.Scripts;
using System;
using TMPro;

public class Board : MonoBehaviour
{
    [Header("Object containers")]
    public GameObject TrainstationsContainer;
    public GameObject TracksContainer;
    public GameObject CardDeckContainer;
    public GameObject TrainsContainer;

    [Header("Prefabs")]
    public Train TrainPrefab;

    [Header("Board settings")]
    public int LevelNumber = 0;
    public int HandsCount;
    public int NumberOfTrains = 100;
    public bool UnlimitedTrains = false;
    public int TrainsBeginningSpeed = 700;

    [Header("Level elements")]
    public Box StartStation;
    public Box[] EndStations;
    public Box[] FailStations;

    [Header("Level transition")]
    public string NextLevel = "Menu_Lea";
    public string CurrentLevel = "Menu_Lea";

    [Header("Editor settings")]
    public EditorPreset EditorPreset;

    [Header("Runtime values")]
    public UIPanel UIPanel;
    public Canvas BoardCanvas;
    public List<Box> Boxes;
    public List<Card> Cards;
    public bool IsRunning;
    public List<Train> Trains;
    public int Score = 0;
    public int[] TrainArrivals;


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
        if (this.BoardCanvas == null)
        {
            this.BoardCanvas = this.gameObject.GetComponentInParent<Canvas>();
        }

        this.UnlimitedTrains = false;
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

        this.ClearBoxes();
        this.ClearCards();
        this.CollectAllBoxes();
        this.CollectAllCards();
    }

    public void InitUI(UIPanel uiPanel)
    {
        this.UIPanel = uiPanel;
        this.ResetScores();
        this.UIPanel.ResultsPanel.gameObject.SetActive(false);
    }

    public void ResetScores()
    {
        this.Score = 0;
        this.TrainArrivals = new int[this.HandsCount];
        if(this.UIPanel.ResultsPanel != null)
            this.UIPanel.ResultsPanel.Refresh(null, this.NumberOfTrains, 0);
    }

    public void SendManyTrains(int HowMany)
    {
        if (IsRunning)
            return;

        if (this.UIPanel.ResultsPanel != null)
            this.UIPanel.ResultsPanel.gameObject.SetActive(true);

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
        StartCoroutine(this._sendManyTrains(this.NumberOfTrains));
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

    private IEnumerator _sendManyTrains(int HowMany)
    {
        int j = 0;
        while (this.UnlimitedTrains)
        {
            this.SendTrain(j);
            j++;
            yield return new WaitForSecondsRealtime(0.06f);
        }
            
        for (int i = 0; i < HowMany; i++)
        {
            this.SendTrain(i);
            yield return new WaitForSecondsRealtime(0.06f);
            yield return i / (float)HowMany;
        }
        yield break;
    }

    public void SendTrain(int trainNumber)
    {
        Hand hand = this.StartStation.DecideOutput();
        if (hand != null || hand.ConnectedTrack == null)
        {
            Train train = GameObject.Instantiate<Train>(this.TrainPrefab, this.TrainsContainer.transform);
            train.name = "Train_" + trainNumber;
            train.speed = this.TrainsBeginningSpeed;

            train.PlaceOnHand(hand);
            this.Trains.Add(train);
        }
    }

    public bool IsStartStation(TrainHandler box)
    {
        if (this.StartStation == null)
        {
            Debug.LogWarning("There is no start station !!!!");
            return false;
        }

        return this.StartStation == box;
    }

    public bool IsEndStation(TrainHandler box)
    {
        if (this.EndStations == null)
        {
            Debug.LogWarning("There is no end station !!!!");
            return false;
        }
            
        return this.EndStations.Contains(box);
    }

    public bool IsFailStation(TrainHandler box)
    {
        if (this.FailStations == null)
            return false;

        return this.FailStations.Contains(box);
    }

    public bool IsWinStation(TrainHandler box)
    {
        if (this.FailStations == null)
            return true;

        return IsEndStation(box) && !IsFailStation(box);
    }

    internal void RegisterTrainArrival(Train train, Hand hand, TrainHandler trainHandler)
    {
        TrainArrivals[hand.Index]++;
        this.Score++;
        this.UIPanel.ResultsPanel.Refresh(TrainArrivals, NumberOfTrains, this.Score);

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

        if(!this.Boxes.Contains(box))
            this.Boxes.Add(box);
    }

    internal void RegisterCard(Card card)
    {
        if (this.Cards == null)
            this.Cards = new List<Card>();

        if (!this.Cards.Contains(card))
            this.Cards.Add(card);
    }

    public void ClearBoxes()
    {
        if (this.Boxes != null)
            this.Boxes.Clear();
    }

    public void ClearCards()
    {
        if (this.Cards != null)
            this.Cards.Clear();
    }

    public void CollectAllBoxes()
    {
        foreach (Transform child in this.TrainstationsContainer.transform)
        {
            Box box = child.GetComponent<Box>();
            if (box != null)
                this.RegisterBox(box);
        }
    }

    public void CollectAllCards()
    {
        foreach (Transform child in this.CardDeckContainer.transform)
        {
            Card card = child.GetComponentInChildren<Card>();
            if (card != null)
                this.RegisterCard(card);
        }
    }

}
