using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    #region Fields
    #region Singleton
    private static GameplayManager instance;
    private static object _lock = new object();
    protected bool renewable = false;

    public static GameplayManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameplayManager>();
                    Object[] instances = FindObjectsOfType<GameplayManager>();

                    if (instances.Length > 1)
                    {
                        return instance;
                    }
                }
                return instance;
            }
        }
    }
    #endregion
    public Settings settings;
    [SerializeField]
    private CylinderPool pool;
    [SerializeField]
    private Cylinder baseCylinder;
    private List<Cylinder> tower = new List<Cylinder>();
    private bool isGameOver = true;
    private Camera cam;
    private Vector3 baseCamPos;
    private bool isHold = false;
    private Cylinder lastCylinder;
    #endregion

    private void Start()
    {
        settings.yStep = baseCylinder.transform.localScale.y * 2;        
        cam = FindObjectOfType<Camera>();
        baseCamPos = cam.transform.position;
        InputManager.Instance.OnMouseDown += MouseDown;
        StartGame();
    }

    private void OnDestroy()
    {
        try
        {
            InputManager.Instance.OnMouseDown -= MouseDown;
        }
        catch(System.Exception ex)
        {
            Debug.LogWarning(ex, this);
        }
    }

    #region Methods
    private void MouseDown()
    {
        if (!InputManager.Instance.IsInputBlocked)
        {
            if (isGameOver)
                RestartGame();
            else if (!InputManager.Instance.IsHold)
            {
                InputManager.Instance.IsHold = true;
                BuildCylinder();
            }
        }
    }

    private void StartGame()
    {
        isGameOver = false;
        tower = new List<Cylinder>();
        tower.Add(baseCylinder);
    }

    private void StopGame()
    {
        for(int i = tower.Count - 1; i >= 1; i--)
        {
            pool.Push(tower[i]);
        }
        cam.transform.position = baseCamPos;
    }

    private void RestartGame()
    {
        StopGame();
        StartGame();
    }

    private void BuildCylinder()
    {
        Cylinder newCylinder = pool.Pull();
        newCylinder.gameObject.transform.parent = pool.transform;

        var newCylinderPos = newCylinder.gameObject.transform.position;
        newCylinder.gameObject.transform.position = new Vector3(newCylinderPos.x, settings.yStep * tower.Count, newCylinderPos.z);

        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + settings.yStep, cam.transform.position.z);

        tower.Add(newCylinder);
        lastCylinder = newCylinder;            
        newCylinder.StartBuildScale(tower[tower.Count - 2].transform.localScale);
    }

    public void CheckCylinderScale()
    {
        lastCylinder.StopAllCoroutines();
        //if Lose
        if (lastCylinder.transform.lossyScale.x > tower[tower.Count-2].transform.lossyScale.x || lastCylinder.transform.lossyScale.z > tower[tower.Count - 2].transform.lossyScale.z)
        {
            StartCoroutine(GameOver());
        }
        else if (lastCylinder.transform.localScale.x >= tower[tower.Count - 2].transform.lossyScale.x - settings.scaleFault &&
            lastCylinder.transform.localScale.z >= tower[tower.Count - 2].transform.lossyScale.z - settings.scaleFault)
        {
            lastCylinder.isPerfect = true;
            StartCoroutine(TowerWaveAnim());
        }
    }

    private IEnumerator TowerWaveAnim()
    {
        if (settings.blockInputWhileWaveAnimationPlaying)
            InputManager.Instance.BlockInput();
        float waveAnimDelay = 0.2f;
        Vector3 toBigger;
        Vector3 toLesser;
        if (lastCylinder.isPerfect)
        {
            toBigger = new Vector3(lastCylinder.transform.localScale.x + 0.4f,
                                            lastCylinder.transform.localScale.y,
                                            lastCylinder.transform.localScale.z + 0.4f);
            toLesser = new Vector3(toBigger.x - 0.2f, toBigger.y, toBigger.z - 0.2f);
            lastCylinder.StartWaveAnim(toBigger, toLesser);


            for (int i = tower.Count - 2; i >= 0; i--)
            {
                yield return new WaitForSeconds(waveAnimDelay);
                if (tower[i] != null)
                {
                    toBigger = new Vector3(tower[i].transform.localScale.x + 0.3f,
                                                tower[i].transform.localScale.y,
                                                tower[i].transform.localScale.z + 0.3f);
                    if (tower[i].isPerfect)
                    {
                        //to start scale
                        toLesser = new Vector3(tower[i].transform.localScale.x, tower[i].transform.localScale.y, tower[i].transform.localScale.z);
                    }
                    else
                    {
                        toLesser = new Vector3(toBigger.x * 0.8f, toBigger.y, toBigger.z * 0.8f);
                    }
                    tower[i].StartWaveAnim(toBigger, toLesser);
                }
            }
        }
        if(settings.blockInputWhileWaveAnimationPlaying)
            InputManager.Instance.UnblockInput();
        yield return null;
    }

    private IEnumerator GameOver()
    {
        isGameOver = true;
        InputManager.Instance.BlockInput();
        lastCylinder.SetLoseMaterial();
        yield return new WaitForSeconds(0.5f);
        lastCylinder.gameObject.SetActive(false);
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -tower.Count - 1);
        InputManager.Instance.UnblockInput();
    }
    #endregion
}
