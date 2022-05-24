using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CameraManager camManager;
    public Vector3 movDir;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (camManager)
        {
            camManager.movVec += movDir;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (camManager)
        {
            camManager.movVec -= movDir;
        }
    }

}
