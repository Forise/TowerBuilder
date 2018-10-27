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

    #region Properties
    public bool IsHold
    {
        get { return isHold; }
        private set { isHold = value; }
    }
    #endregion

    private void Start()
    {
        settings.yStep = baseCylinder.transform.localScale.y * 2;
        IsHold = false;
        cam = FindObjectOfType<Camera>();
        baseCamPos = cam.transform.position;
        StartGame();
    }

    private void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isGameOver)
                StartGame();
            else if(!IsHold)
            {
                IsHold = true;
                BuildCylinder();                
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            IsHold = false;
        }
    }

    #region Methods
    private void StartGame()
    {
        isGameOver = false;
        tower = new List<Cylinder>();
        tower.Add(baseCylinder);
    }

    private void StopGame()
    {
        isGameOver = true;
        for(int i = tower.Count - 1; i >= 1; i--)
        {
            pool.Push(tower[i]);
        }
        cam.transform.position = baseCamPos;
    }

    private void BuildCylinder()
    {
        Cylinder newCylinder = pool.Pull();
        newCylinder.gameObject.transform.parent = tower[tower.Count - 1].transform;

        var newCylinderPos = newCylinder.gameObject.transform.position;
        newCylinder.gameObject.transform.position = new Vector3(newCylinderPos.x, settings.yStep * tower.Count, newCylinderPos.z);

        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + settings.yStep, cam.transform.position.z);

        tower.Add(newCylinder);
        lastCylinder = newCylinder;
        newCylinder.StartBuildScale(settings.endAnimationScale);
    }

    public void CheckCylinderScale()
    {
        lastCylinder.StopAllCoroutines();
        //if Lose
        if (lastCylinder.transform.localScale.x > 1 || lastCylinder.transform.localScale.z > 1)
        {
            StopGame();
        }
        else if (lastCylinder.transform.localScale.x >= 1 - settings.scaleFault &&
            lastCylinder.transform.localScale.z >= 1 - settings.scaleFault)
        {
            lastCylinder.isPerfect = true;
            StartCoroutine(TowerWaveAnim());
        }
    }

    private IEnumerator TowerWaveAnim()
    {
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
        yield return null;
    }
    #endregion
}
