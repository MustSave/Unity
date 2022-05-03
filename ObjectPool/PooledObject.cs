using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField]
    bool useObjectPool = true;
    // Start is called before the first frame update
    private void OnDisable()
    {
        if (useObjectPool) ObjectPooler.ReturnToPool(gameObject);
    }
}
