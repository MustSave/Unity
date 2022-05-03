using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    protected Grabber holder;
    public Grabber Holder => holder;

    public virtual void Catch(Grabber holder)
    {
        this.holder = holder;
    }
    public virtual void Release()
    {
        if (holder == null) return;

        holder = null;
    }
}
