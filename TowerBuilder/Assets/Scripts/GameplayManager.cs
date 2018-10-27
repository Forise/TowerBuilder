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
            if(!IsHold)
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
        newCylinder.gameObject.transform.position = new Vector3(newCylinderPos.x, (settings.yStep * tower.Count), newCylinderPos.z);

        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + settings.yStep, cam.transform.position.z);

        tower.Add(newCylinder);
        newCylinder.StartScale(settings.endAnimationScale);
    }

    public void CheckCylinderScale()
    {
        if (tower[tower.Count - 1].transform.localScale.x > 1 || tower[tower.Count - 1].transform.localScale.z > 1)
        {
            StopGame();
        }
        else if (tower[tower.Count - 1].transform.localScale.x > 1 - settings.scaleFault || 
            tower[tower.Count - 1].transform.localScale.z > 1 - settings.scaleFault)
        {
            tower[tower.Count - 1].isPerfect = true;
        }
    }
    #endregion
}
