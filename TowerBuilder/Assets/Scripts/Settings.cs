using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObject/Settings")]
public class Settings : ScriptableObject
{
    public float yStep = 0.25f;
    public float scaleSpeed = 1;
    public float scaleFault = 0.5f;
    [HideInInspector]
    public Vector3 endAnimationScale = new Vector3(1.5f, 1.1f, 1.5f);
}
