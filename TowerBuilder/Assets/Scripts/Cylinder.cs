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
    public bool isScaling = false;
    public bool isPerfect = false;
    #endregion

    #region Methods
    public void StartBuildScale(Vector3 endScale)
    {
        StartCoroutine(SmoothBuildScale(endScale));
    }

    public void StartWaveAnim(Vector3 toBigger, Vector3 toLesser)
    {
        StartCoroutine(WaveAnim(toBigger, toLesser));
    }

    public void StopAllScale()
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

    private IEnumerator SmoothBuildScale(Vector3 endScale)
    {
        isScaling = true;
        while (isScaling)
        {
            if (transform.localScale.x < maxScale.x && transform.localScale.z < maxScale.z && GameplayManager.Instance.IsHold)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, endScale, GameplayManager.Instance.settings.buildScaleSpeed * Time.deltaTime);
                yield return transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z); //hold "y" scale
            }
                
            else
            {
                yield return isScaling = false;
                GameplayManager.Instance.CheckCylinderScale();                
            }
        }
    }

    private IEnumerator SmoothScaleTo(Vector3 endScale)
    {
        isScaling = true;
        float elapsedTime = 0;
        while (elapsedTime < GameplayManager.Instance.settings.waveAnimTime)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, elapsedTime / GameplayManager.Instance.settings.waveAnimTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
        isScaling = false;
    }

    private IEnumerator WaveAnim(Vector3 toBigger, Vector3 toLesser)
    {
        isScaling = true;
        yield return StartCoroutine(SmoothScaleTo(toBigger));
        while (isScaling)
            yield return null;
        yield return StartCoroutine(SmoothScaleTo(toLesser));
    }
    #endregion
}
