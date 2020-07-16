using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;

public class ChatController : MonoBehaviourPunCallbacks
{
    public Transform chatContainer;
    public GameObject chatMessage;
    public Scrollbar vertScrollbar;

    public TMPro.TMP_InputField messageInput;

    public void PostMessage()
    {
        GameObject chat = Instantiate(chatMessage, chatContainer);
        ChatMessage msg = chat.GetComponent<ChatMessage>();

        msg.UpdateMessageText(messageInput.text);

        photonView.RPC("ChatRPC", RpcTarget.OthersBuffered, messageInput.text);

        messageInput.text = "";

        EventSystem.current.SetSelectedGameObject(messageInput.gameObject, null);
        messageInput.OnPointerClick(new PointerEventData(EventSystem.current));

        StartCoroutine(DoSomething());
    }

    IEnumerator DoSomething()
    {
        yield return new WaitForSeconds(0.2f);
        vertScrollbar.value = 0.0f;
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        GameObject chat = Instantiate(chatMessage, chatContainer);
        ChatMessage mswg = chat.GetComponent<ChatMessage>();

        mswg.UpdateMessageText(msg);

        StartCoroutine(DoSomething());
    }
}
