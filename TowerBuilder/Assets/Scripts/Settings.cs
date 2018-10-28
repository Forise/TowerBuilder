using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObject/Settings")]
public class Settings : ScriptableObject
{
    [HideInInspector]
    public float yStep = 1f;
    public float buildScaleSpeed = 1;
    public float waveAnimTime = 0.1f;
    public float scaleFault = 0.05f;
    public bool blockInputWhileWaveAnimationPlaying = false;
}
