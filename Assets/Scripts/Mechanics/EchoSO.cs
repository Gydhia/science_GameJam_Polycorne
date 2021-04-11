using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundController;

[ExecuteAlways]
[CreateAssetMenu(fileName = "New Echo", menuName = "FeuFollet/Echo")]
public class EchoSO : ScriptableObject
{
    [Range(0, 1)]
    public float DecayRatio;
    [Range(0, 1)]
    public float DryMix;
}
