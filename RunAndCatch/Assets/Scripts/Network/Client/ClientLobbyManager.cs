
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientLobbyManager : MonoBehaviourPunCallbacks
{
    public Text LogText;
    private bool isConnecting = false;
    private bool isConnected = false;
    public List<string> mapPool = new List<string>();

    public static ClientLobbyManager Instance { get; private set; }

    void Awake()
    {
        CreateSingleton();
    }

    protected void CreateSingleton()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectToPhotonServer();
    }

    public void ConnectToPhotonServer()
    {
        isConnecting = true;

        PhotonNetwork.NickName = "default" + Random.Range(0, 100);
        Log("My name is " + PhotonNetwork.NickName);

        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to master!");
        isConnecting = false;
        isConnected = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Log("Disconnected! Cause: " + cause.ToString());
        isConnecting = false;
        isConnected = false;
    }

    public bool IsConnecting()
    {
        return isConnecting;
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public void CreateRoom()
    {
        if (isConnected)
        {
            // get random map from map pool
            int randomId = Random.Range(0, mapPool.Capacity);
            string levelName = mapPool[randomId];

            Hashtable tournamentPropierties = new Hashtable() { { "Level", levelName }, { "IsGameRunning", false } };
            string[] lobbyProperties = { "Level", "IsGameRunning" };

            RoomOptions opt = new RoomOptions
            {
                MaxPlayers = 5,
                IsOpen = true,
                IsVisible = true,
                CustomRoomPropertiesForLobby = lobbyProperties,
                CustomRoomProperties = tournamentPropierties
            };

            PhotonNetwork.CreateRoom(null, opt);
        }
    }

    public void JoinRoom()
    {
        if (isConnected)
        {
            Hashtable table = new Hashtable();
            table.Add("skin", "0");
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);

            Hashtable customRoomProperties = new Hashtable() { { "IsGameRunning", false } };
            PhotonNetwork.JoinRandomRoom(customRoomProperties, 5);
        }
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the room");
        string level = (string) PhotonNetwork.CurrentRoom.CustomProperties["Level"];
        PhotonNetwork.LoadLevel(level);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log("Error code: " + returnCode + " msg: " + message);
        CreateRoom();
    }

    private void Log(string msg)
    {
        Debug.Log(msg);

        if(LogText == null) {
            MobileMainMenuScreen mainMenu = (MobileMainMenuScreen)UIManager.Instance.GetScreenById(MobileMainMenuScreen.ID);
            LogText = mainMenu.LogText;
        }

        LogText.text += "\n";
        LogText.text += msg;
    }
}
