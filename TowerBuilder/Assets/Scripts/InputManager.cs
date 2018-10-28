using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    #region Fields
    #region Singleton
    private static InputManager instance;
    private static object _lock = new object();
    protected bool renewable = false;

    public static InputManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InputManager>();
                    Object[] instances = FindObjectsOfType<InputManager>();

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
    private bool isInputBlocked = false;
    public delegate void MouseDownDelegate();
    public event MouseDownDelegate OnMouseDown;
    private bool isHold = false;
    #endregion

    #region Properties
    public bool IsHold
    {
        get { return isHold; }
        set { isHold = value; }
    }

    public bool IsInputBlocked
    {
        get { return isInputBlocked; }
        private set { isInputBlocked = value; }
    }
    #endregion

    void Start()
    {
        IsHold = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isInputBlocked)
        {            
            StartCoroutine(IsHoldCoroutine());
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopAllCoroutines();
            IsHold = false;
        }
    }
    #region Methods
    public void BlockInput()
    {
        IsInputBlocked = true;
    }

    public void UnblockInput()
    {
        IsInputBlocked = false;
    }

    private IEnumerator IsHoldCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        OnMouseDown();
        IsHold = true;
    }
    #endregion
}
