using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Status : MonoBehaviour
{
    private const string STEP = "_Steps";
    private const string RATIO = "_HSRatio";
    private const string WIDTH = "_Width";
    private const string THICKNESS = "_Thickness";

    private static readonly int floatSteps = Shader.PropertyToID(STEP);
    private static readonly int floatRatio = Shader.PropertyToID(RATIO);
    private static readonly int floatWidth = Shader.PropertyToID(WIDTH);
    private static readonly int floatThickness = Shader.PropertyToID(THICKNESS);

    [Range(0, 2800f)] public float Hp = 583;
    [Range(0, 2800f)] public float MaxHp = 583;
	[Range(0, 920f)] public float Sp = 0f;
	[Range(0, 10f)] public float dropDownSpeed = 3f;
    [Range(0, 500f)] public float perBlockHp = 100f;

    public float hpShieldRatio;
    public float RectWidth = 100f;
    [Range(0, 10f)] public float Thickness = 2f;

    public Image hp;
    public Image damaged;
    public Image sp;
    public Image separator;

    [ContextMenu("Create Material")]
    private void CreateMaterial()
    {
        separator.material = new Material(Shader.Find("ABS/UI/Health Separator"));
    }

    private float regenHp = 6.5f;
    public float defence = 32;
    public float magicDefence = 32;
    private PhotonView pv;
    public TMP_Text nickNameText;
    private PlayerMove pm;
    private Animator anim;
    private NavMeshAgent navAgent;
    public GameObject canvas;

    private void Awake() 
    {
        pv = GetComponent<PhotonView>();
        pm = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        Hp = 583;

        if (pv.IsMine)
        {
            pv.RPC("SetName", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
        }
        else
        {
            ColorUtility.TryParseHtmlString("#BF3B4D", out Color color);
            hp.color = color;
        }

        //CreateMaterial();
    }

    private void Update() 
    {
        if (MaxHp <= Hp)
        {
            Hp = MaxHp;
        }
        else
        {
            Hp += regenHp * Time.deltaTime;
        }

        float step = 0;

        if (Sp > 0)
        {
            if (Hp + Sp > MaxHp)
            {
                hpShieldRatio = Hp / (Hp + Sp);
                sp.fillAmount = 1f;
                step = (Hp) / perBlockHp;
                hp.fillAmount = Hp / (Hp + Sp);
            }
            else
            {
                sp.fillAmount = (Hp + Sp) / MaxHp;
                hpShieldRatio = Hp / MaxHp;
                step = Hp / perBlockHp;
                hp.fillAmount = Hp / MaxHp;
            }
        }
        else
        {
            sp.fillAmount = 0f;
            step = MaxHp / perBlockHp;
            hpShieldRatio = 1f;
            hp.fillAmount = Hp / MaxHp;
        }

        damaged.fillAmount = Mathf.Lerp(damaged.fillAmount, hp.fillAmount, Time.deltaTime * dropDownSpeed);

        separator.material.SetFloat(floatSteps, step);
        separator.material.SetFloat(floatRatio, hpShieldRatio);
        separator.material.SetFloat(floatWidth, RectWidth);
        separator.material.SetFloat(floatThickness, Thickness);
    }

    [PunRPC]
    private void SetName(string name)
    {
        nickNameText.text = name;
    }

    private void OnDeath()
    {
        pm.canMove = false;
        pm.canRotate = false;
        pm.canSkill = false;
        navAgent.enabled = false;
        canvas.SetActive(false);
        anim.SetTrigger("Death");

        InGameManager.instance.GameResult(!pv.IsMine);
    }

    [PunRPC]
    private void GetDamage(int viewID)
    {
        GameObject other = PhotonNetwork.GetPhotonView(viewID).gameObject;
        Hp -= other.GetComponent<Knife>().GiveDamage(Hp);
        // audioSource.PlayOneShot(onHitClip);
        SoundManager.instance.PlayOneShot("QHit");
        if (Hp <= 0)
        {
            OnDeath();
        }

        Destroy(other);
    }

    public void SetHp(float value)
    {
        pv.RPC("RPCSetHp", RpcTarget.All, value);
    }

    [PunRPC]
    private void RPCSetHp(float value)
    {
        Hp = value;
    }

    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Damageable"))
        {
            pv.RPC("GetDamage", RpcTarget.All, other.GetComponentInParent<PhotonView>().ViewID);
            
        }
    }
}
