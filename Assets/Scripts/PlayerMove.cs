//#define test
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviour
{
    public GameObject knife;
    public GameObject clickParticle;
    public PhotonView pv;
    public GameObject qInticator;
    public LayerMask movableLayer;
    public float moveSpeed;
    public Transform throwPosition;
    private Transform tr;
    private Rigidbody rb;
    private Camera camera;
    private Animator anim;
    private NavMeshAgent navAgent;

    public bool canMove = true;
    private Vector3 destination;

    public bool useSmartKey;
    public bool prevState;
    public bool canRotate = true;
    public bool canSkill = true;

    public Skill Q,D,F;

    void Awake()
    {
        if (pv.IsMine)
        {
            camera = Camera.main;
            this.useSmartKey = PlayerSettings.instance.useSmartKey;
            gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        }
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();

        navAgent.speed = moveSpeed;
        canMove = true;
        canRotate = true;
    }

    void Start()
    {
        if (pv.IsMine)
        {
            Q.SetUI();
            D.SetUI();
            F.SetUI();
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit rayHit;
                if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out rayHit, float.MaxValue, movableLayer))
                {
                    GameObject go = Instantiate(clickParticle, rayHit.point, Quaternion.identity);
                    go.transform.forward = rayHit.normal;
                    pv.RPC("MoveTo", RpcTarget.All, rayHit.point);
                }
            }
            else if (Input.GetMouseButton(1))
            {
                RaycastHit rayHit;
                if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out rayHit, float.MaxValue, movableLayer))
                {
                    pv.RPC("MoveTo", RpcTarget.All, rayHit.point);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                pv.RPC("RPC_TP", RpcTarget.All, tr.position);
                pv.RPC("Stop", RpcTarget.All);
            }

            if (canSkill)
            {
                Q.CheckInput();
                D.CheckInput();
                F.CheckInput();
            }

            #if test
            navAgent.speed = moveSpeed;
            #endif
        }
    }

    Quaternion targetRot;
    public float rotSpeed;
    [PunRPC]
    private void MoveTo(Vector3 dest)
    {
        print(canMove);
        if(canMove)
        {
            navAgent.isStopped = false;
            float angle = Vector3.SignedAngle(tr.forward, dest - tr.position, Vector3.up);
            if (Mathf.Abs(angle) > 90)
            {
                // tr.eulerAngles += tr.up * angle;
                targetRot = Quaternion.Euler(tr.eulerAngles + tr.up * angle);
                StopCoroutine("SmoothRotate");
                StartCoroutine("SmoothRotate");
            }
            anim.SetBool("Move", true);
        }

        navAgent.SetDestination(dest);
        destination = dest;

        if (!checking)
        {
            StartCoroutine(CheckReachedDestination());
        }
    }
    bool checking;
    IEnumerator CheckReachedDestination()
    {
        checking = true;
        yield return null;
        while(navAgent.remainingDistance > 0.1f)
        {
            yield return null;
        }
        anim.SetBool("Move", false);
        checking = false;
    }

    IEnumerator SmoothRotate()
    {
        print("Start Rotate");
        while (Quaternion.Angle(tr.rotation, targetRot) > 0.1f)
        {
            while (!canRotate)
                yield return null;
            tr.rotation = Quaternion.RotateTowards(tr.rotation, targetRot, Time.deltaTime * rotSpeed);
            yield return null;
        }
        tr.rotation = targetRot;
        print("Stop Rotate");
    }

    public void TP(Vector3 position)
    {
        pv.RPC("RPC_TP", RpcTarget.All, position);
    }
    [PunRPC]
    private void RPC_TP(Vector3 position)
    {
        tr.position = position;
    }

    [PunRPC]
    private void Stop()
    {
        navAgent.isStopped = true;
        prevState = true;
        anim.SetBool("Move", false);
    }

    public void CanMove()
    {
        navAgent.isStopped = prevState;
        navAgent.updateRotation = true;
        canMove = true;
        canRotate = true;
    }
}
