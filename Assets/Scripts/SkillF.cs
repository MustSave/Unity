using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class SkillF : Skill
{
    PhotonView pv;
    PlayerMove pm;
    Camera camera;
    NavMeshAgent navAgent;

    public float tpLength = 3;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        pm = GetComponent<PlayerMove>();
        navAgent = GetComponent<NavMeshAgent>();
        if (pv.IsMine)
            camera = Camera.main;
    }

    private void Update() {
        Debug.DrawRay(transform.position, transform.forward * tpLength, Color.red);
    }

    public override void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CanUseSkill())
            {
                UseSkill();
                //pv.RPC("UseGhost", RpcTarget.All);
            }
        }
    }

    protected void UseSkill()
    {
        base.UseSkill();

        RaycastHit hit;
        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit);

        Vector3 dir = Vector3.ProjectOnPlane(hit.point - transform.position, transform.up);
        dir = Vector3.ClampMagnitude(dir, tpLength);


        //pm.TP(transform.position + dir);
        pv.RPC("OnSkill", RpcTarget.All, dir);
    }

    [PunRPC]
    private void OnSkill(Vector3 dir)
    {
        // if (effectSound)audioSource.PlayOneShot(effectSound);
        SoundManager.instance.PlayOneShot("Flash");
        navAgent.Warp(transform.position + dir);
        transform.forward = dir;
        navAgent.ResetPath();
    }

}
