using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundController;

[ExecuteAlways]
[CreateAssetMenu(fileName = "New Reverb", menuName = "FeuFollet/Reverb")]
public class ReverbSO : ScriptableObject
{
    public AudioReverbPreset ReverbPreset;
}
