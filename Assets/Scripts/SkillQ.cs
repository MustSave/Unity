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
    public LayerMask rayLayer;
    private Status status;


    private void Awake() {
        pv = GetComponent<PhotonView>();
        pm = GetComponent<PlayerMove>();
        navAgent = GetComponent<NavMeshAgent>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        status = GetComponent<Status>();

        if (pv.IsMine)
        {
            camera = Camera.main;
            this.useSmartKey = PlayerSettings.instance.useSmartKey;
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
                        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, rayLayer);

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
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, rayLayer);

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
        pm.canRotate = false;
        pm.prevState = navAgent.isStopped;
        navAgent.isStopped = true;

        
        navAgent.updateRotation = false;
        tr.LookAt(point);

        //anim.SetBool("Move", false);
        anim.SetTrigger("Q");
        status.Hp -= 50;
    }

    public void ThrowQ_Event()
    {
        if (pv.IsMine)
        {
            GameObject go = PhotonNetwork.Instantiate("knife", tr.position + tr.up * 1.5f, Quaternion.LookRotation(tr.forward, tr.up));
            go.layer = gameObject.layer;
            go.transform.GetChild(0).gameObject.layer = gameObject.layer;
            go.GetComponent<Knife>().status = this.status;
        }
    }
}
