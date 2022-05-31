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
    public GameObject btn;
    public int loadCompletePlayers;

    private void Start() 
    {
        instance = this;
        // PhotonNetwork.AutomaticallySyncScene = false;

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
    private void Spawn(int team)
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

        
        GetComponent<PhotonView>().RPC("Spawn", RpcTarget.All, Random.Range(0, 2));
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
        btn.SetActive(true);

    }

    public void BackToLobby()
    {
        StartCoroutine(BackToLobbyI());
    }

    IEnumerator BackToLobbyI()
    {
        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Leaving)
        {
            print("Leaving");
            yield return null;
        }
        Destroy(PlayerSettings.instance.gameObject);
        PhotonNetwork.LoadLevel(0);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B))
        {
            btn.SetActive(true);
        }
    }
}
