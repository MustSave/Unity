using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    Camera cam;
    Transform tr;
    Transform camTr;

    private void Awake() {
        cam = Camera.main;
        tr = GetComponent<Transform>();
        camTr = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        tr.rotation = camTr.rotation;
    }
}
