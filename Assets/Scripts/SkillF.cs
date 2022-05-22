using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillF : Skill
{
    PhotonView pv;
    PlayerMove pm;
    Camera camera;
    public float tpLength = 3;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        pm = GetComponent<PlayerMove>();
        if (pv.IsMine)
            camera = Camera.main;
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

        pm.TP(transform.position + dir);
    }

}
