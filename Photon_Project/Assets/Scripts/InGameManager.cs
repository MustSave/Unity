using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum Team {Red, Blue};
[RequireComponent(typeof(PhotonView))]
public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    GameObject playerPrefab;
    public Transform[] spawnPlace;

    public GameObject WinnerPanel;
    public GameObject LoserPanel;
    public int loadCompletePlayers;

    private void Start() 
    {
        instance = this;

        Cursor.lockState = CursorLockMode.Confined;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ReadyToStart());
        }

        GetComponent<PhotonView>().RPC("LoadComplete", RpcTarget.MasterClient);   
    }

    [PunRPC]
    private void LoadComplete()
    {
        loadCompletePlayers++;
    }

    [PunRPC]
    private void Spawn()
    {
        if (PlayerSettings.instance.team == Team.Red)
        {
            PhotonNetwork.Instantiate("DrMundo", spawnPlace[(int)Team.Red].position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("DrMundo", spawnPlace[(int)Team.Blue].position, Quaternion.identity);
        }
    }

    IEnumerator ReadyToStart()
    {
        while (PhotonNetwork.CountOfPlayersInRooms != loadCompletePlayers)
        {
            yield return null;
        }

        GetComponent<PhotonView>().RPC("Spawn", RpcTarget.All);
    }

    public void GameResult(bool win)
    {
        StartCoroutine(ShowWinner(win));
    }

    IEnumerator ShowWinner(bool win)
    {
        yield return new WaitForSeconds(3);

        if (win)
            WinnerPanel.SetActive(true);
        else
            LoserPanel.SetActive(true);
    }
}
