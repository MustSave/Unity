using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class EventInvoker : MonoBehaviour
{
    public CustomEvent onCustomEvent = new CustomEvent();

    private void Start() {
        onCustomEvent.Invoke(1, 2);
    }
}
