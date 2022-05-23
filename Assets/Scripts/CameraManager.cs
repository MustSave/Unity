using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviour
{
    PhotonView pv;
    Camera cam;
    public float scrollSpeed;
    public float camMoveSpeed = 1;
    public float camSpeed = 10;
    [Range(30, 100f)] public float minFov;
    [Range(30, 100f)]public float maxFov;

    public Vector3 movVec;

    private void Awake() 
    {
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            cam = Camera.main;

            var s = GameObject.FindObjectsOfType<PanelTest>();
            foreach (var a in s)
            {
                a.camManager = this;
            }

            StartCoroutine(Zoom());
            StartCoroutine(ScreenMove());
        }
    }

    IEnumerator Zoom() 
    {
        while (true)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;

            if (scroll == 0)
            {
                yield return null;
                continue;
            }

            float tmp = cam.fieldOfView + scroll;
            cam.fieldOfView = Mathf.Clamp(tmp, minFov, maxFov);

            if (cam.fieldOfView == minFov || cam.fieldOfView == maxFov)
            {
                yield return null;
                continue;
            }

            if (scroll < 0 && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit rayHit))
            {
                Vector3 dir = Vector3.ProjectOnPlane(rayHit.point - cam.transform.position, cam.transform.forward);
                cam.transform.position += dir.normalized * Time.deltaTime * camMoveSpeed;
            }

            yield return null;
        }
    }

    IEnumerator ScreenMove()
    {
        while(true)
        {
            cam.transform.position += movVec.normalized * Time.deltaTime * camSpeed;
            print(movVec);
            yield return null;
        }
    }
}
