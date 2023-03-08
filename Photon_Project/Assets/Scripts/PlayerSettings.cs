using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings instance;
    public Image teamButtonImg;
    public bool useSmartKey {get; private set;}
    public Team team;
    private void Awake() 
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ToggleSmartKey(Toggle t)
    {
        useSmartKey = t.isOn;
    }

    public void ToggleTeamColor()
    {
        team = team == Team.Red ? Team.Blue : Team.Red;
        teamButtonImg.color = team == Team.Red ? Color.red : Color.blue;
    }
}
