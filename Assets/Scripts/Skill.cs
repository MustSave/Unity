using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float coolTime;
    protected float remainTime = 0;

    public virtual void CheckInput()
    {
    }

    public bool CanUseSkill()
    {
        if (remainTime == 0)
        {
            return true;
        }
        return false;
    }

    public void UseSkill()
    {
        remainTime = coolTime;
        StartCoroutine(CalcRemainTime());
    }

    protected IEnumerator CalcRemainTime()
    {
        while (remainTime > 0)
        {
            yield return null;
            remainTime -= Time.deltaTime;
        }
        remainTime = 0;
    }
}
