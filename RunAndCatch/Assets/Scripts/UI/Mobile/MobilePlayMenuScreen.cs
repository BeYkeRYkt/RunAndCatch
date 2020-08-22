
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePlayMenuScreen : UIScreen
{
    public static string ID = "ui_mobile_play_menu";

    public LobbyState lobbyState;
    public Button mainButton;
    public Button backButton;

    private bool interactiveEnabled = false;

    // icon sprite for searching
    public Sprite ic_search;

    // icon sprite for timer
    public Sprite ic_timer;

    // ping text
    public Text pingText;

    // header text
    public Text headerText;

    // button text
    public Text buttonText;

    // loading bar
    public Image LoadingBar;
    public Image LoadingBarIcon;
    public Color disabledColor;
    public Color enabledColor;

    // local player nickname
    public Text playerText;

    // for others players
    public List<string> othersPlayersNicknames = new List<string>();
    public List<Text> numberTexts = new List<Text>();
    public List<Text> playerTexts = new List<Text>();
    public Color disabledLabelColor;
    public Color enabledLabelColor;

    private int multiply = 1;
    public float currentValue = 0;
    public float searchProgressSpeed;
    public float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!interactiveEnabled) return;
            interactiveEnabled = false;

            if (lobbyState == LobbyState.IN_ROOM)
            {
                ClientLobbyManager.Instance.LeaveRoom();
            }
            else
            {
                StopCurrentProccess();
                UIManager uiManager = UIManager.Instance;
                uiManager.CloseGUI();
                uiManager.OpenGUI(MobileMainMenuScreen.ID);
            }
        }

        UpdateUI();
    }

    private void UpdateUI ()
    {
        // update ping text
        if (ClientLobbyManager.Instance.IsConnected())
        {
            pingText.text = LanguageManager.Instance.langReader.getString("ui_ping_text") + ": " + PhotonNetwork.GetPing() + " ms";
        }

        if (lobbyState == LobbyState.SEARCHING)
        {
            RectTransform rectTransform = LoadingBar.gameObject.GetComponent<RectTransform>();
            rectTransform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));

            if (currentValue <= 0)
            {
                multiply = 1;
                LoadingBar.fillClockwise = false;
            }
            else if (currentValue >= 100)
            {
                multiply = -1;
                LoadingBar.fillClockwise = true;
            }

            // animate progress bar
            currentValue += (float)searchProgressSpeed * Time.deltaTime * multiply;
            SetCircleValue(currentValue);
        }
        else if (lobbyState == LobbyState.IN_ROOM)
        {
            RoomManager roomManager = RoomManager.Instance;
            // update timer
            if (roomManager.state == GameRoomState.WAITING || roomManager.state == GameRoomState.STARTING)
            {
                float value = (float)roomManager.GetTimeCooldown() / (float)roomManager.cooldownTimeInSeconds * 100f;
                SetCircleValue(value);
            }

            // block buttons when time <= 1 seconds
            if (roomManager.GetTimeCooldown() <= 1)
            {
                // disable buttons
                mainButton.interactable = false;
                backButton.interactable = false;
            }
            else
            {
                // enable buttons
                if (mainButton.interactable)
                {
                    mainButton.interactable = true;
                }

                if (backButton.interactable)
                {
                    backButton.interactable = true;
                }
            }
        }
    }

    public override void OnShowScreen()
    {
        base.OnShowScreen();
        playerText.text = PhotonNetwork.NickName;
        UpdateUI();
    }

    public override void Initialize()
    {
        mId = ID;
        ShowIdleState();

        // set default text
        headerText.text = LanguageManager.Instance.langReader.getString("ui_play_menu_player_list_text").ToUpper();
    }

    public override void OnHideScreen()
    {
        ShowIdleState();
    }

    public void OnButtonPress()
    {
        if (!interactiveEnabled) return;
        interactiveEnabled = false;

        switch(lobbyState)
        {
            case LobbyState.IDLE:
                // start searching room
                if (ClientLobbyManager.Instance.JoinRoom())
                {
                    ShowSearchState();
                } else
                {
                    interactiveEnabled = true;
                }
                break;
            case LobbyState.SEARCHING:
                ShowIdleState();
                break;
            case LobbyState.IN_ROOM:
                // tell leave room
                if (!ClientLobbyManager.Instance.LeaveRoom())
                {
                    interactiveEnabled = true;
                }
                break;
        }
    }

    public void OnBackButtonPress()
    {
        if (!interactiveEnabled) return;
        interactiveEnabled = false;

        if (lobbyState == LobbyState.IN_ROOM)
        {
            ClientLobbyManager.Instance.LeaveRoom();
        }
        else
        {
            StopCurrentProccess();
            UIManager uiManager = UIManager.Instance;
            uiManager.CloseGUI();
            uiManager.OpenGUI(MobileMainMenuScreen.ID);
        }
    }

    public void StopCurrentProccess()
    {
        // set idle view
        ShowIdleState();
    }

    public void ShowIdleState()
    {
        // set lobby state
        lobbyState = LobbyState.IDLE;

        interactiveEnabled = true;

        // reset loading bar
        LoadingBar.fillClockwise = false;
        LoadingBar.color = disabledColor;
        SetCircleValue(100);
        currentValue = 0;

        RectTransform rectTransform = LoadingBar.gameObject.GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, 180);

        // set icon
        LoadingBarIcon.sprite = ic_search;

        // set player names
        for (int i = 0; i < numberTexts.Count; i++)
        {
            Text numberLabel = numberTexts[i];
            numberLabel.color = disabledLabelColor;

            Text playerLabel = playerTexts[i];
            playerLabel.color = disabledLabelColor;
            playerLabel.text = LanguageManager.Instance.langReader.getString("ui_play_menu_empty_slot_text");
        }
   
        // enable buttons
        mainButton.interactable = true;
        backButton.interactable = true;

        // set button text
        buttonText.text = LanguageManager.Instance.langReader.getString("ui_play_menu_button_search_text").ToUpper();
    }

    public void ShowSearchState()
    {
        // set lobby state
        lobbyState = LobbyState.SEARCHING;

        interactiveEnabled = true;

        // enable loading bar
        SetCircleValue(100);
        currentValue = 0;
        LoadingBar.color = enabledColor;
        LoadingBar.fillClockwise = false;
        RectTransform rectTransform = LoadingBar.gameObject.GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, 180);

        // set icon
        LoadingBarIcon.sprite = ic_search;

        // set player names
        for (int i = 0; i < numberTexts.Count; i++)
        {
            Text numberLabel = numberTexts[i];
            numberLabel.color = disabledLabelColor;

            Text playerLabel = playerTexts[i];
            playerLabel.color = disabledLabelColor;
            playerLabel.text = LanguageManager.Instance.langReader.getString("ui_play_menu_player_searching_text");
        }

        // disable buttons
        mainButton.interactable = false;
        backButton.interactable = false;

        // set button text
        buttonText.text = LanguageManager.Instance.langReader.getString("ui_play_menu_button_stop_search_text").ToUpper();
    }

    public void ShowInRoomState()
    {
        // set lobby state
        lobbyState = LobbyState.IN_ROOM;

        interactiveEnabled = true;

        // set timer icon
        SetCircleValue(100);
        currentValue = 0;
        RectTransform rectTransform = LoadingBar.gameObject.GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, 180);

        LoadingBar.fillClockwise = false;
        LoadingBar.color = enabledColor;
        LoadingBarIcon.sprite = ic_timer;

        // set player names
        if (othersPlayersNicknames.Count == 0)
        {
            for (int i = 0; i < numberTexts.Count; i++)
            {
                Text playerLabel = playerTexts[i];
                playerLabel.text = LanguageManager.Instance.langReader.getString("ui_play_menu_player_waiting_text");
            }
        }

        // enable button
        mainButton.interactable = true;
        backButton.interactable = true;

        // set button text
        buttonText.text = LanguageManager.Instance.langReader.getString("ui_play_menu_button_leave_text").ToUpper();
    }

    public void SetCircleValue(float currentValue)
    {
        if (currentValue > 100)
        {
            currentValue = 100;
        }
        LoadingBar.fillAmount = currentValue / 100;
    }

    public float GetCircleValue()
    {
        return LoadingBar.fillAmount * 100;
    }

    public void ParseFromPhotonNetwork()
    {
        othersPlayersNicknames.Clear();

        // receive new player list
        if (PhotonNetwork.CurrentRoom != null)
        {
            int size = PhotonNetwork.PlayerListOthers.Length;
            for (int i = 0; i < size; i++)
            {
                Player photonPlayer = PhotonNetwork.PlayerListOthers[i];
                if (!photonPlayer.NickName.Equals(PhotonNetwork.NickName))
                {
                    othersPlayersNicknames.Add(photonPlayer.NickName);
                }
            }
        }

        UpdatePlayerList();
    }

    public void AddPlayerToPlayerList(string playerName)
    {
        othersPlayersNicknames.Add(playerName);
        UpdatePlayerList();
    }

    public void RemovePlayerFromPlayerList(string playerName)
    {
        if (othersPlayersNicknames.Contains(playerName))
        {
            othersPlayersNicknames.Remove(playerName);
            UpdatePlayerList();
        }
    }

    private void UpdatePlayerList()
    {
        // update ui
        if (othersPlayersNicknames.Count > 0)
        {
            int size = othersPlayersNicknames.Count;
            Debug.Log("size: " + size);
            for (int i = 0; i < size; i++)
            {
                Text numberLabel = numberTexts[i];
                numberLabel.color = enabledLabelColor;

                Text playerLabel = playerTexts[i];
                playerLabel.color = enabledLabelColor;
                playerLabel.text = othersPlayersNicknames[i];
            }

            for (int i = size; i < numberTexts.Count; i++)
            {
                Text numberLabel = numberTexts[i];
                numberLabel.color = disabledLabelColor;

                Text playerLabel = playerTexts[i];
                playerLabel.color = disabledLabelColor;
                playerLabel.text = LanguageManager.Instance.langReader.getString("ui_play_menu_player_waiting_text");
            }
        }
        else
        {
            for (int i = 0; i < numberTexts.Count; i++)
            {
                Text numberLabel = numberTexts[i];
                numberLabel.color = disabledLabelColor;

                Text playerLabel = playerTexts[i];
                playerLabel.color = disabledLabelColor;
                playerLabel.text = LanguageManager.Instance.langReader.getString("ui_play_menu_player_waiting_text");
            }
        }
    }
}

public enum LobbyState {
    IDLE,
    SEARCHING,
    IN_ROOM
}