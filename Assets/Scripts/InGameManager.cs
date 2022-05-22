using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InGameManager : MonoBehaviour
{
    GameObject playerPrefab;
    private void Start() 
    {
        PhotonNetwork.Instantiate("DrMundo", new Vector3(1.046f,0,5.83099985f), Quaternion.identity);
    }
}
