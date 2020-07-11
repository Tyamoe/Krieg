using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class MainMenuHandler : MonoBehaviourPunCallbacks, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int MenuButton = 0;

    private bool Connected = false;

    public MainMenuController menuCtrl;

    private GameObject[] Highlights;

    public TextAsset myServer;

    void Start()
    {
        Highlights = menuCtrl.Highlights;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {

    }

    public override void OnConnectedToMaster()
    {
        Connected = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlights[MenuButton].SetActive(true);

        transform.parent.Find("Label").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlights[MenuButton].SetActive(false);

        transform.parent.Find("Label").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 1, 1, 0.4f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Connected)
        {
            if (MenuButton == 0)
            {
                menuCtrl.Matchmaking();
            }
            else if (MenuButton == 1)
            {
                menuCtrl.Host();
            }
            else if (MenuButton == 2)
            {
                menuCtrl.List();
            }
            else if (MenuButton == 3)
            {
                menuCtrl.Profile();
            }
            else if (MenuButton == 4)
            {
                menuCtrl.Settings();
            }
            else
            {
                return;
            }

            Highlights[MenuButton].SetActive(false);
        }
        else
        {
            IEnumerator coroutine;
            coroutine = NotConnected(Highlights[MenuButton]);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator NotConnected(GameObject obj)
    {
        GameObject g = obj.transform.Find("Error").gameObject;
        g.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        g.SetActive(false);
        obj.SetActive(false);
    }
}
