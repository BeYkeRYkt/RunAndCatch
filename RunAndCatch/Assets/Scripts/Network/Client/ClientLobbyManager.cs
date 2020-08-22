
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

    // timer
    private CooldownTimer cooldownTimer;
    public float cooldownTimeInSeconds = 5; // in secs

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
        cooldownTimer = new CooldownTimer(cooldownTimeInSeconds);
        cooldownTimer.TimerCompleteEvent += OnTimerComplete;

        if (!PhotonNetwork.IsConnected)
        {
            ConnectToPhotonServer();
        } else
        {
            OnConnectedToMaster();
        }
    }

    public void ConnectToPhotonServer()
    {
        isConnecting = true;

        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        // setting photon
        //PhotonNetwork.SerializationRate = 5;

        // Open connecting screen
        UIManager uiManager = UIManager.Instance;
        uiManager.OpenGUI(MobileConnectToServerScreen.ID);
        MobileConnectToServerScreen connectScreen = (MobileConnectToServerScreen) uiManager.GetCurrentScreen();
        connectScreen.showConnectingState();
    }

    public override void OnConnectedToMaster()
    {
        if (!isConnected)
        {
            // open main menu screen
            UIManager uiManager = UIManager.Instance;
            uiManager.CloseGUI();
            uiManager.OpenGUI(MobileMainMenuScreen.ID);
        }

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

        // open connecting screen again
        UIManager uiManager = UIManager.Instance;
        if (uiManager.GetCurrentScreen().mId != MobileConnectToServerScreen.ID)
        {
            uiManager.CloseGUI();
            uiManager.OpenGUI(MobileConnectToServerScreen.ID);
        }
        MobileConnectToServerScreen connectScreen = (MobileConnectToServerScreen)uiManager.GetCurrentScreen();
        connectScreen.SetTime((int)cooldownTimeInSeconds);
        connectScreen.showFailedState();

        // start timer
        cooldownTimer.Start();
    }

    public void Update()
    {
        // update timer
        cooldownTimer.Update(Time.deltaTime);

        if (!isConnected || !isConnecting)
        {
            if (cooldownTimer.IsActive)
            {
                UIManager uiManager = UIManager.Instance;
                MobileConnectToServerScreen connectScreen = (MobileConnectToServerScreen) uiManager.GetCurrentScreen();
                connectScreen.SetTime((int)cooldownTimer.TimeRemaining + 1);
            }
        }
    }

    public void OnTimerComplete()
    {
        // try reconnect
        ConnectToPhotonServer();
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

    public bool JoinRoom()
    {
        if (isConnected)
        {
            if(!PhotonNetwork.IsConnectedAndReady)
            {
                return false;
            }

            Hashtable table = new Hashtable();
            table.Add("skin", "0");
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);

            Hashtable customRoomProperties = new Hashtable() { { "IsGameRunning", false } };
            return PhotonNetwork.JoinRandomRoom(customRoomProperties, 5);
        }

        return false;
    }

    public override void OnJoinedRoom()
    {
        // update ui to room state
        UIManager uiManager = UIManager.Instance;
        MobilePlayMenuScreen screen = (MobilePlayMenuScreen) uiManager.GetCurrentScreen();
        screen.ShowInRoomState();

        Log("Joined the room");
        string level = (string) PhotonNetwork.CurrentRoom.CustomProperties["Level"];
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //PhotonNetwork.LoadLevel(level);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log("Error code: " + returnCode + " msg: " + message);
        CreateRoom();
    }

    public bool LeaveRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            return PhotonNetwork.LeaveRoom();
        }
        return false;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        // update ui to idle state
        UIManager uiManager = UIManager.Instance;
        MobilePlayMenuScreen screen = (MobilePlayMenuScreen)uiManager.GetCurrentScreen();
        screen.ShowIdleState();
    }

    private void Log(string msg)
    {
        Debug.Log(msg);
    }
}
