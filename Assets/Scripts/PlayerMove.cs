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

    public Skill Q,D,F;

    void Awake()
    {
        if (pv.IsMine)
        {
            camera = Camera.main;
        }
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();

        navAgent.speed = moveSpeed;
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
                    pv.RPC("MoveTo", RpcTarget.All, rayHit.point);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                pv.RPC("RPC_TP", RpcTarget.All, tr.position);
                pv.RPC("Stop", RpcTarget.All);
            }

            Q.CheckInput();
            D.CheckInput();
            F.CheckInput();

            #if test
            navAgent.speed = moveSpeed;
            #endif
        }

        CheckDeistination();

    }

    private void CheckDeistination()
    {
        if (navAgent.remainingDistance < 0.01)
        {
            navAgent.isStopped = true;
            //pv.RPC("RPC_TP", RpcTarget.All, destination);
            anim.SetBool("Move", false);
        }
    }

    [PunRPC]
    private void MoveTo(Vector3 dest)
    {
        if(canMove)
        {
            navAgent.isStopped = false;
            float angle = Vector3.SignedAngle(tr.forward, dest - tr.position, Vector3.up);
            if (Mathf.Abs(angle) > 90)
            {
                tr.eulerAngles += tr.up * angle;
            }
            anim.SetBool("Move", true);
        }

        navAgent.SetDestination(dest);
        destination = dest;
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
    }

    private void OnTriggerEnter(Collider other) 
    {
        
    }
}
