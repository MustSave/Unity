using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    Transform camTransform;
    public float scrollSpeed;
    public float camMoveSpeed = 1;
    public float camSpeed = 10;
    private bool symmetry;
    [Range(30, 100f)] public float minFov;
    [Range(30, 100f)]public float maxFov;

    public Vector3 movVec;
    private Vector3 camFwd;

    private void Awake() {
        instance = this;
        camTransform = Camera.main.transform;
        symmetry = PlayerSettings.instance.IsSymmetryCamera;

        if (symmetry)
             Camera.main.transform.eulerAngles = new Vector3(45, 0, 0);

        camFwd = camTransform.forward;
    }

    Transform tr;
    public void SetCamera(Transform tr)
    {
        this.tr = tr;

        StartCoroutine(Zoom());
        StartCoroutine(ScreenMove());
    }

    IEnumerator Zoom() 
    {
        while (true)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

            if (scroll == 0)
            {
                yield return null;
                continue;
            }

            // float tmp = cam.fieldOfView + scroll;
            // cam.fieldOfView = Mathf.Clamp(tmp, minFov, maxFov);
            Vector3 position = camTransform.position + camFwd * scroll * Time.deltaTime;
            if (position.y > maxFov || position.y < minFov)
            {
                yield return null;
                continue;
            }
            camTransform.position = position;

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

        while(true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                CamToCharacter();

                yield return null;
                continue;
            }

            a();
            camTransform.position += movVec.normalized * Time.deltaTime * camSpeed;
            yield return null;
        }
    }

    private void CamToCharacter()
    {
        float dist = Vector3.Distance(camTransform.position, tr.position);
        Vector3 dir = Vector3.ProjectOnPlane(camFwd * dist, Vector3.up);

        Vector3 camPosition = tr.position - dir;
        camPosition.y = camTransform.position.y;

        camTransform.position = camPosition;
    }

    public float boardThickness = 25;
    void a()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.y >= Screen.height - boardThickness)
            movVec.z = symmetry ? 1 : -1;
        else if (mousePosition.y <= boardThickness)
            movVec.z = symmetry ? -1 : 1;
        else
            movVec.z = 0;

        if (mousePosition.x >= Screen.width - boardThickness)
            movVec.x = symmetry ? 1 : -1;
        else if (mousePosition.x <= boardThickness)
            movVec.x = symmetry ? -1 : 1;
        else
            movVec.x = 0;
    }
}
