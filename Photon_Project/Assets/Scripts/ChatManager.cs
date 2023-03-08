using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;
    protected PhotonView pv;
    public TMP_InputField inputField;
    public GameObject chatArea;
    protected TMP_Text[] chatText;
    public bool isFocused;

    private void Awake() {
        instance = this;
        pv = GetComponent<PhotonView>();
    }
    private void Start() {
        chatText = chatArea.GetComponentsInChildren<TMP_Text>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isFocused)
            {
                SendChat();
            }
            else
            {
                StartChat();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndChat();
        }
    }

    protected virtual void StartChat()
    {
        inputField.ActivateInputField();
        isFocused = true;
    }

    protected virtual void SendChat()
    {
        Send();
        EndChat();
    }

    protected virtual void EndChat()
    {
        inputField.DeactivateInputField();
        isFocused = false;
    }

    public void Init()
    {
        isFocused = false;
        ClearChat();
    }

    public void Send()
    {
        print("Send");
        if (inputField.text.Equals(""))
            return;
        
        pv.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + inputField.text);
        inputField.text = "";
    }

    [PunRPC]
    public virtual void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < chatText.Length; i++)
        {
            if (chatText[i].text == "")
            {
                isInput = true;
                chatText[i].text = msg;
                break;
            }
        }

        if (!isInput)
        {
            for (int i = 1; i < chatText.Length; i++)
                chatText[i-1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
    }

    public void ClearChat()
    {
        inputField.text = "";
        for (int i = 0; i < chatText.Length; i++)
        {
            chatText[i].text = "";
        }
    }

}
