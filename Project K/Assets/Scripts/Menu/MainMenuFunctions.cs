using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using System;
using System.IO;
using System.Net;
using System.Text;

public class MainMenuFunctions : MonoBehaviourPunCallbacks, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int MenuButton = 0;

    private bool Connected = false;

    public MainMenuController menuCtrl;

    private GameObject[] Highlights;

    public TextAsset myServer;

    void Start()
    {
        Highlights = menuCtrl.Highlights;
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
                Matchmaking();
            }
            else if (MenuButton == 1)
            {
                Host();
            }
            else if (MenuButton == 2)
            {
                List();
            }
            else if (MenuButton == 3)
            {
                Profile();
            }
            else if (MenuButton == 4)
            {
                Settings();
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

    private void Matchmaking()
    {
        string roomName = "Room123";

        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOpt, new TypedLobby(null, LobbyType.Default));

        /*WebClient client = new WebClient();
        client.Credentials = new NetworkCredential("u89290814", "VJDaRTKXu9S2mY");
        client.UploadFile("home681734469.1and1-data.host/krieg/" + roomName + ".txt", @"C:\local\path\file.zip");*/

        //FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.tyamoe.com/krieg/rooms/" + roomName + ".txt");
        /*FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://home681734469.1and1-data.host/krieg/rooms/" + roomName + ".txt");
        request.Method = WebRequestMethods.Ftp.UploadFile;

        request.Credentials = new NetworkCredential("u89290814", "VJDaRTKXu9S2mY");

        byte[] fileContents = myServer.bytes;

        request.ContentLength = fileContents.Length;

        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(fileContents, 0, fileContents.Length);
        }

        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        {
            Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
        }*/
    }

    private void Host()
    {

    }

    private void List()
    {
        menuCtrl.MainMenuPanel.SetActive(false);
        menuCtrl.MatchListPanel.SetActive(true);
    }

    private void Profile()
    {

    }

    private void Settings()
    {

    }
}
