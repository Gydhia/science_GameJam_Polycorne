﻿using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class UIPanel : MonoBehaviour
{
    public ResultsPanel ResultsPanel;
    public TextMeshPro LevelText;

    [Header("Runtime values")]
    public Board Board;

    public void Start()
    {
        ResultsPanel ResultsPanel = GameObject.FindObjectOfType<ResultsPanel>(true);
        if (ResultsPanel != null)
            this.ResultsPanel = ResultsPanel;

        if (this.LevelText != null)
        {
            TextMeshPro levelText = GameObject.FindGameObjectWithTag("LevelText").GetComponent<TextMeshPro>();
            this.LevelText = levelText;
        }
        if (this.LevelText != null)
            this.LevelText.text = "Level " + this.Board.LevelNumber;

        //this is a security when we start directly the level from the editor (scene are not loaded in the same order sometinme)
        if (this.Board == null)
            this.Board = Board.Instance;

        if (this.Board == null)
            Debug.LogError("There is no Board available... Aborting UIPanel init.");
        else
            this.Board.InitUI(this);
    }

    public void ClickOnStart(int train)
    {
        if(SoundController.Instance != null) {
            SoundController.Instance.PlaySound(SoundController.SoundNames.Play);
            SoundController.Instance.PlayMusic(SoundController.MusicNames.ActionTheme);
        }
        this.Board.SendManyTrains(train);
        this.GetComponent<GameUI>().StartTimeline();
    }
}
