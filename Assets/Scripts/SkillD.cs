using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class SkillD : Skill
{
    PhotonView pv;
    NavMeshAgent navAgent;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public override void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (CanUseSkill())
            {
                UseSkill();
                pv.RPC("UseGhost", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void UseGhost()
    {
        StartCoroutine(Ghost());
    }
    IEnumerator Ghost()
    {
        float prevSpeed = navAgent.speed;
        navAgent.speed *= 1.3f;
        yield return new WaitForSecondsRealtime(10);
        navAgent.speed = prevSpeed;
    }
}
