using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    [SerializeField]
    EventInvoker ei;
    private void Awake() {
        ei.onCustomEvent.AddListener(UpdateAmmoHUD);
    }

    private void UpdateAmmoHUD(int a, int b)
    {
        // do something
        print("Event Invoked argument1 : " + a + ", argument2 : " + b);
    }
}
