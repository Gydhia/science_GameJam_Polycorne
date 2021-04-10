using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundController;

[ExecuteAlways]
[CreateAssetMenu(fileName = "New Music", menuName = "FeuFollet/GameMusic")]
public class MusicSO : ScriptableObject
{
    public MusicNames MusicID;

    /// <summary>
    /// First played line
    /// </summary>
    public ClipSO[] MainLines;
    /// <summary>
    /// Secondly played line
    /// </summary>
    public ClipSO[] MelodicLines;
    /// <summary>
    /// Thirdly played line
    /// </summary>
    public ClipSO[] AccompanimentLines;

    /// <summary>
    /// Bars to wait between : MainLines & MelodicLines -> MelodicLines & AccompanimentLines
    /// </summary>
    public bool[] BarsToWait = new bool[2];

}
