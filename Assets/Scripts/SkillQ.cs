using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class SkillQ : Skill
{
    PhotonView pv;
    PlayerMove pm;
    public GameObject qIndicator;
    public GameObject knife;
    bool useSmartKey;
    NavMeshAgent navAgent;
    Transform tr;
    Animator anim;
    Camera camera;


    private void Awake() {
        pv = GetComponent<PhotonView>();
        pm = GetComponent<PlayerMove>();
        navAgent = GetComponent<NavMeshAgent>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();

        if (pv.IsMine)
        {
            camera = Camera.main;
        }

        coolTime = 4;
    }

    public override void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            {
                if (CanUseSkill())
                {
                    if (useSmartKey)
                    {
                        RaycastHit hit;
                        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit);

                        UseSkill();
                        pv.RPC("ThrowQ", RpcTarget.All, hit.point);
                    }
                    else
                    {
                        StartCoroutine(IEThrowQ());
                    }
                }
            }
    }

    IEnumerator IEThrowQ()
    {
        qIndicator.SetActive(true);

        while(true)
        {
            RaycastHit hit;
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit);

            Vector3 dir = Vector3.ProjectOnPlane(hit.point - tr.position, tr.up);
            qIndicator.transform.forward = dir;

            if (Input.GetMouseButtonDown(0))
            {
                qIndicator.SetActive(false);

                UseSkill();
                pv.RPC("ThrowQ", RpcTarget.All, hit.point);
                yield break;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                qIndicator.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }

    [PunRPC]
    private void ThrowQ(Vector3 point)
    {
        pm.canMove = false;
        pm.prevState = navAgent.isStopped;
        navAgent.isStopped = true;

        
        navAgent.updateRotation = false;
        tr.LookAt(point);

        //anim.SetBool("Move", false);
        anim.SetTrigger("Q");
    }

    public void ThrowQ_Event()
    {
        Instantiate(knife, tr.position + tr.up * 1.5f, Quaternion.LookRotation(tr.forward, tr.up));
    }
}
