using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ChatManagerInGame : ChatManager
{
    public bool focusing;
    private void Start() {
        chatText = chatArea.GetComponentsInChildren<TMP_Text>();
        Init();
        chatArea.SetActive(false);

        StartCoroutine(AutoDisableChat());
    }

    protected override void StartChat()
    {
        print("Start");
        chatArea.SetActive(true);
        focusing = true;
        base.StartChat();
    }

    protected override void SendChat()
    {
        chatArea.SetActive(true);
        Send();
        inputField.DeactivateInputField();
        isFocused = false;
    }

    protected override void EndChat()
    {
        base.EndChat();
        chatArea.SetActive(false);
        focusing = false;
    }

    public void OnEndEdit()
    {
        //inputField.DeactivateInputField();
        focusing = false;
    }

    float t = 0;
    IEnumerator AutoDisableChat()
    {
        t = 0;
        while(true)
        {
            if (isFocused == true && focusing == false)
            {
                t += Time.deltaTime;
                if (t > 5)
                {
                    EndChat();
                    t = 0;
                }
            }

            yield return null;
        }
    }

    [PunRPC]
    public override void ChatRPC(string msg)
    {
        chatArea.SetActive(true);
        isFocused = true;
        base.ChatRPC(msg);
    }
}
