using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderPool : MonoBehaviour
{
    #region Fields
    public Cylinder cylinderPrefab;

    private List<Cylinder> pool = new List<Cylinder>();
    #endregion

    #region Methods
    public Cylinder Pull()
    {
        if(pool.Count > 0)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].gameObject.activeInHierarchy)
                {
                    pool[i].gameObject.transform.localScale = new Vector3(0,1,0);
                    pool[i].gameObject.SetActive(true);
                    return pool[i];
                }
            }
        }

        Cylinder cylinder = Instantiate(cylinderPrefab);
        AddObjectToPool(cylinder);
        return cylinder;
    }

    public void Push(Cylinder cylinder)
    {
        if (cylinder.gameObject.activeInHierarchy)
        {
            try
            {
                Cylinder objectToPush = pool.Find(x => x == cylinder);

                if (objectToPush == null)
                    AddObjectToPool(objectToPush);

                objectToPush.gameObject.SetActive(false);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex, this);
            }
        }
    }

    public void AddObjectToPool(Cylinder cylinder)
    {
        pool.Add(cylinder);
    }
    #endregion
}