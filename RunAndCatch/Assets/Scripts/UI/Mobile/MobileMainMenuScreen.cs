using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class MobileMainMenuScreen : UIScreen
{
    public static string ID = "ui_mobile_main_menu";

    public Text pingText;
    public Text LogText;

    public Text reconnectText;
    private CooldownTimer cooldownTimer;
    public float cooldownTimeInSeconds = 5; // in secs

    private bool buttonsEnabled;
    private ClientLobbyManager lobbyManager;

    // buttons
    public List<Button> btns = new List<Button>();

    // Update is called once per frame
    void Update()
    {
        // update timer
        cooldownTimer.Update(Time.deltaTime);

        // update ping text
        pingText.text = "Ping: " + PhotonNetwork.GetPing();

        // check master server
        if (lobbyManager.IsConnected())
        {
            // enable buttons
            if (!buttonsEnabled)
            {
                EnableButtons();
            }
        }
        else if(lobbyManager.IsConnected() || lobbyManager.IsConnecting())
        {
            // hide reconnect text
            if (reconnectText.gameObject.activeSelf)
            {
                reconnectText.gameObject.SetActive(false);
            }
        }
        else if (!lobbyManager.IsConnected() && !lobbyManager.IsConnecting())
        {
            if (buttonsEnabled)
            {
                DisableButtons();
            }

            // try reconnect
            if (!cooldownTimer.IsActive)
            {
                reconnectText.gameObject.SetActive(true);
                reconnectText.text = "Try reconnect to Photon servers: " + cooldownTimeInSeconds + "s";
                cooldownTimer.Start();
            }

            // update reconnect message
            reconnectText.text = "Try reconnect to Photon servers: " + (int)cooldownTimer.TimeRemaining + "s";
        }
    }

    public override void Initialize()
    {
        mId = ID;

        if (lobbyManager == null)
        {
            lobbyManager = ClientLobbyManager.Instance;
        }

        cooldownTimer = new CooldownTimer(cooldownTimeInSeconds);
        cooldownTimer.TimerCompleteEvent += OnTimerComplete;

        // disable by default
        reconnectText.gameObject.SetActive(false);

        // disable buttons by default
        DisableButtons();
    }

    void OnTimerComplete()
    {
        reconnectText.gameObject.SetActive(false);
        lobbyManager.ConnectToPhotonServer();
    }

    public void DisableButtons()
    {
        for(int i = 0; i < btns.Capacity; i++)
        {
            Button btn = btns[i];
            btn.interactable = false;
        }
        buttonsEnabled = false;
    }

    public void EnableButtons()
    {
        for (int i = 0; i < btns.Capacity; i++)
        {
            Button btn = btns[i];
            btn.interactable = true;
        }
        buttonsEnabled = true;
    }

    public void DebugLog(string msg)
    {
        Debug.Log(msg);
        LogText.text += "\n";
        LogText.text += msg;
    }

    public void OnCreateRoomButtonPressed()
    {
        UIManager uiManager = UIManager.Instance;
        //uiManager.CloseGUI();
        lobbyManager.CreateRoom();
    }

    public void OnJoinRoomButtonPressed()
    {
        UIManager uiManager = UIManager.Instance;
        //uiManager.CloseGUI();
        lobbyManager.JoinRoom();
    }
}
