using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum Team {Red, Blue};
public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    GameObject playerPrefab;
    public Transform[] spawnPlace;

    public GameObject WinnerPanel;
    public GameObject LoserPanel;

    private void Start() 
    {
        instance = this;

        Cursor.lockState = CursorLockMode.Confined;

        if (PlayerSettings.instance.team == Team.Red)
        {
            PhotonNetwork.Instantiate("DrMundo", spawnPlace[(int)Team.Red].position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("DrMundo", spawnPlace[(int)Team.Blue].position, Quaternion.identity);
        }
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
