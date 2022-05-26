using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skill : MonoBehaviour
{
    public float coolTime;
    protected float remainTime = 0;

    protected Image baseImage;
    protected TMP_Text remainText;
    protected Image fillImage;

    protected Color originColor;
    protected Color inactiveColor;

    public SkillKey skillKey;
    public AudioClip effectSound;
    public AudioSource audioSource;
    public virtual void CheckInput()
    {
    }

    public void SetUI()
    {
        GameObject go = UIManger.instance.skillList[(int)skillKey];
        baseImage = go.GetComponent<Image>();
        remainText = go.transform.GetChild(1).GetComponent<TMP_Text>();
        fillImage = go.transform.GetChild(0).GetComponent<Image>();

        Init_UI();
    }

    protected void Init_UI()
    {
        ColorUtility.TryParseHtmlString("#4F4F4F", out inactiveColor);
        originColor = baseImage.color;

        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Radial360;
        fillImage.fillOrigin = (int)Image.Origin360.Top;
        fillImage.fillClockwise = false;
        fillImage.fillAmount = 0;
    }

    protected void Set_Fill()
    {
        fillImage.fillAmount = remainTime / coolTime;

        string txt = remainTime < 10 ? remainTime.ToString("0.0") : remainTime.ToString("0");
        remainText.text = txt;
    }

    protected void End_CoolTime()
    {
        Set_Fill();
        remainText.gameObject.SetActive(false);
        baseImage.color = originColor;
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
        remainText.gameObject.SetActive(true);
        baseImage.color = inactiveColor;
        StartCoroutine(CalcRemainTime());
    }

    protected IEnumerator CalcRemainTime()
    {
        while (remainTime > 0)
        {
            yield return null;
            remainTime -= Time.deltaTime;
            Set_Fill();
        }
        remainTime = 0;
        End_CoolTime();
    }
}
