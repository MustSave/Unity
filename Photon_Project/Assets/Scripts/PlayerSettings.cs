using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    public NetworkManager nmng;
    public static PlayerSettings instance;
    public Image teamButtonImg;
    public bool useSmartKey {get; private set;}
    public Team team;
    public bool symmetryCamera;
    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            print("AA");
            // PlayerSettings.instance.InitSettingValues();
            // Destroy(gameObject);
            Destroy(PlayerSettings.instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ToggleSmartKey(Toggle t)
    {
        useSmartKey = t.isOn;
    }

    public void ToggleSymmetryCamera(Toggle t)
    {
        symmetryCamera = t.isOn;
    }

    public void SetTeamColor()
    {
        teamButtonImg.color = team == Team.Red ? Color.red : Color.blue;
    }

    public bool IsSymmetryCamera
    {
        get 
        {
            return symmetryCamera && team == Team.Red;
        }
    }

    private void InitSettingValues()
    {
        team = Team.Red;
        symmetryCamera = false;
        useSmartKey = false;
    }
}
