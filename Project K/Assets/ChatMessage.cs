using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatMessage : MonoBehaviour
{
    public Text MessageText;

    public void UpdateMessageText(string msg)
    {
        MessageText.text = msg;
    }
}
