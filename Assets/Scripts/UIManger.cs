using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillKey {Q, D, F};
public class UIManger : MonoBehaviour
{
    public static UIManger instance;
    public GameObject[] skillList;

    private void Awake() {
        instance = this;
    }
}
