using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChatController : MonoBehaviourPunCallbacks
{
    public Transform chatContainer;
    public GameObject chatMessage;

    public TMPro.TMP_InputField messageInput;

    public void PostMessage()
    {
        GameObject chat = Instantiate(chatMessage, chatContainer);
        ChatMessage msg = chat.GetComponent<ChatMessage>();

        msg.UpdateMessageText(messageInput.text);

        photonView.RPC("ChatRPC", RpcTarget.OthersBuffered, messageInput.text);

        messageInput.text = "";
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        GameObject chat = Instantiate(chatMessage, chatContainer);
        ChatMessage mswg = chat.GetComponent<ChatMessage>();

        mswg.UpdateMessageText(msg);
    }
}
