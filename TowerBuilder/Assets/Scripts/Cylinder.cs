using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder : MonoBehaviour
{
    #region Fields
    private Vector3 maxScale = new Vector3(1.1f, 1, 1.1f);
    private Vector3 minScale = new Vector3(0.1f, 1, 0.1f);
    [SerializeField]
    private Material loseMaterial;
    [SerializeField]
    private Material baseMaterial;
    public bool isPerfect = false;
    #endregion

    #region Methods
    public void StartScale(Vector3 endScale)
    {
        StartCoroutine(SmoothScale(endScale));
    }

    public void StopScale()
    {
        StopAllCoroutines();
    }

    public void SetLoseMaterial()
    {
        gameObject.GetComponent<MeshRenderer>().material = loseMaterial;
    }

    public void SetBaseMaterial()
    {
        gameObject.GetComponent<MeshRenderer>().material = baseMaterial;
    }

    private IEnumerator SmoothScale(Vector3 endScale)
    {
        bool isScaling = true;
        while (isScaling)
        {
            if (transform.localScale.x < maxScale.x && transform.localScale.z < maxScale.z && GameplayManager.Instance.IsHold)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, endScale, GameplayManager.Instance.settings.scaleSpeed * Time.deltaTime);
                yield return transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
            }
                
            else
            {
                SetLoseMaterial();
                GameplayManager.Instance.CheckCylinderScale();
                yield return isScaling = false;
            }
        }
    }
    #endregion
}
