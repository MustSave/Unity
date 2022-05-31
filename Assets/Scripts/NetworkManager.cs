using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_Text statusText;
    public PhotonView PV;
    [Header("Offline")]
    public GameObject offlinePanel;

    [Header("Lobby")]
    public GameObject lobbyPanel;
    public TMP_Text lobbyInfoText;
    public TMP_InputField roomName;
    public TMP_InputField nickNameInput;
    public TMP_Text welcomeText;
    public Button prevButton;
    public Button nextButton;
    public Button[] cellBtn;
    

    [Header("Room")]
    public GameObject roomPanel;
    public TMP_Text playerListText;
    public TMP_Text RoomInfoText;

    [Header("")]
    private List<RoomInfo> myRoomList = new List<RoomInfo>();
    private int maxPage;
    private int curPage = 1;
    private int multiple;

    public GameObject startButton;

#region RoomList
    public void MyListClick(int num)
    {
        if (num == -2) curPage--;
        else if (num == -1) curPage++;
        else PhotonNetwork.JoinRoom(myRoomList[multiple + num].Name);

        RoomListRenewal();
    }

    void RoomListRenewal()
    {
        maxPage = (myRoomList.Count % cellBtn.Length == 0) ? myRoomList.Count / cellBtn.Length : myRoomList.Count / cellBtn.Length + 1;

        prevButton.interactable = (curPage <= 1) ? false : true;
        nextButton.interactable = (curPage >= maxPage) ? false : true;

        multiple = (curPage - 1) * cellBtn.Length;
        for (int i = 0; i < cellBtn.Length; i++)
        {
            cellBtn[i].interactable = (multiple + i < myRoomList.Count) ? true : false;
            cellBtn[i].transform.GetChild(0).GetComponent<TMP_Text>().text = (multiple + i < myRoomList.Count) ? myRoomList[multiple + i].Name : "";
            cellBtn[i].transform.GetChild(1).GetComponent<TMP_Text>().text = (multiple + i < myRoomList.Count) ? myRoomList[multiple + i].PlayerCount + "/" + myRoomList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int  i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myRoomList.Contains(roomList[i])) myRoomList.Add(roomList[i]);
                else myRoomList[myRoomList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myRoomList.IndexOf(roomList[i]) != -1) myRoomList.RemoveAt(myRoomList.IndexOf(roomList[i]));
        }
        RoomListRenewal();
    }
#endregion

    void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString();
        lobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "Players in Lobby / " + PhotonNetwork.CountOfPlayers + "Players is Online";
    }

#region NetworkConnect
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        offlinePanel.SetActive(false);

        if (PhotonNetwork.LocalPlayer.NickName.Equals("") == true)
        {
            PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
        }

        welcomeText.text = "Welcome " + PhotonNetwork.LocalPlayer.NickName;

        myRoomList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        offlinePanel.SetActive(true);
    }
#endregion

#region Room
    public void CreateRoom() 
    {
        PhotonNetwork.CreateRoom(roomName.text == "" ? "Room" + Random.Range(0, 100) : roomName.text, new RoomOptions {MaxPlayers = 2});
    }
    
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        offlinePanel.SetActive(false);
        RoomRenewal();
        ChatManager.instance.Init();

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
            PlayerSettings.instance.team = (Team)Random.Range(0, 2);
            PlayerSettings.instance.SetTeamColor();

        }
        else
        {
            startButton.SetActive(false);
        }
    }

    private void RoomRenewal()
    {
        playerListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        }
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " | " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomName.text = ""; CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        roomName.text = ""; CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatManager.instance.ChatRPC("<color=yellow>" + newPlayer.NickName + "Entered</color>");
        PV.RPC("SetTeam", newPlayer, PlayerSettings.instance.team == Team.Red ? 1 : 0);
    }

    [PunRPC]
    private void SetTeam(int team)
    {
        PlayerSettings.instance.team = (Team)team;
        PlayerSettings.instance.SetTeamColor();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatManager.instance.ChatRPC("<color=yellow>" + otherPlayer.NickName + "Leaved</color>");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.Equals(newMasterClient))
        {
            startButton.SetActive(true);
        }
    }

#endregion

    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            
            // PV.RPC("ChangeScene", RpcTarget.All);
            PhotonNetwork.LoadLevel(1);
        }
    }

    private void Start() {
        PhotonNetwork.AutomaticallySyncScene = true;
        // Screen.fullScreen = true;
    }
}