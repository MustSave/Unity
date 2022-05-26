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

    private void Start() 
    {
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            cam = Camera.main;

            // var s = GameObject.FindObjectsOfType<PanelTest>();
            // foreach (var a in s)
            // {
            //     a.camManager = this;
            // }

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

            yield return null;
        }
    }

    IEnumerator ScreenMove()
    {
        for (float f = 0; f < 0.5f; f+=Time.deltaTime)
        {
            CamToCharacter();
            yield return null;
        }
        //CamToCharacter();
        while(true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                CamToCharacter();

                yield return null;
                continue;
            }

            a();
            cam.transform.position += movVec.normalized * Time.deltaTime * camSpeed;
            yield return null;
        }

    }

    private void CamToCharacter()
    {
        float dist = Vector3.Distance(cam.transform.position, transform.position);
        Vector3 dir = Vector3.ProjectOnPlane(cam.transform.forward * dist, Vector3.up);

        Vector3 camPosition = transform.position - dir;
        camPosition.y = cam.transform.position.y;

        cam.transform.position = camPosition;
    }

    public float boardThickness = 25;
    void a()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.y >= Screen.height - boardThickness)
            movVec.z = -1;
        else if (mousePosition.y <= boardThickness)
            movVec.z = 1;
        else
            movVec.z = 0;

        if (mousePosition.x >= Screen.width - boardThickness)
            movVec.x = -1;
        else if (mousePosition.x <= boardThickness)
            movVec.x = 1;
        else
            movVec.x = 0;
    }
}
