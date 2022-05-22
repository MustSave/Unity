using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    PlayerMove pm;

    private void Awake() {
        pm = GetComponentInParent<PlayerMove>();
    }

    // public void CanMove(bool movable)
    // {
    //     pm.CanMove(movable);
    // }
}
