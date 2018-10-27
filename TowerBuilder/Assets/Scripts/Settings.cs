using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObject/Settings")]
public class Settings : ScriptableObject
{
    [HideInInspector]
    public float yStep = 1f;
    public float scaleSpeed = 1;
    public float scaleFault = 0.5f;
    [HideInInspector]
    public Vector3 endAnimationScale = new Vector3(1.5f, 1, 1.5f);
}
