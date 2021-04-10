using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundController;

[ExecuteAlways]
[CreateAssetMenu(fileName = "New Clip", menuName = "FeuFollet/Clip")]
public class ClipSO : ScriptableObject
{
    public AudioClip clip;
    public float BPM;

    public ReverbSO Reverb;
    public EchoSO Echo;
}
