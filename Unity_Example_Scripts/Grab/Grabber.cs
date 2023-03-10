using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    // protected GameObject grabObject;
    [SerializeField]
    protected Grabbable holding;
    public Grabbable Holding => holding;

    public virtual void Grab(Grabbable grabbable)
    {
        if (grabbable.Holder != null)
        {
            grabbable.Release();
        }

        grabbable.Catch(this);
        holding = grabbable;
        holding.transform.parent = transform;
    }

    public virtual void Release()
    {
        if (holding == null) return;

        holding.transform.parent = null;
        holding.Release();
        holding = null;
    }

}
