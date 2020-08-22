using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Class for GameRoom
 */
public class RoomManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // Server properties
    public string MapName;
    public string HunterNickname;
    public double StartTime;
    public bool TimerEnable = false;

    // minimum players for start game
    public int minPlayersForStart = 1;

    // loaded players
    public int loadedPlayers = 0;

    // Game room state
    public GameRoomState state = GameRoomState.DISCONNECTED;

    // Timer
    private int timerDecrementValue;
    public int cooldownTimeInSeconds = 10; // in secs
    public int roundTime = 60; // in secs

    private List<IGameRoomListener> mListeners = new List<IGameRoomListener>();

    public static RoomManager Instance { get; private set; }

    void Awake()
    {
        CreateSingleton();
        timerDecrementValue = cooldownTimeInSeconds;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
        // reset player loads
        loadedPlayers = 0;

        if (!aScene.name.Equals("MainMenu"))
        {
            if (state == GameRoomState.RUNNING)
            {
                loadedPlayers++;
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_LEVEL_LOADED, null, raiseEventOptions, sendOptions);
            }
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        // Update timer
        UpdateTimer();

        // Update room state
        if (state == GameRoomState.WAITING || state == GameRoomState.STARTING)
        {
            UpdateRoomState();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        OnConnectedToRoom();
    }

    private void OnConnectedToRoom()
    {
        state = GameRoomState.WAITING;

        // update player list
        UIManager uiManager = UIManager.Instance;
        MobilePlayMenuScreen screen = (MobilePlayMenuScreen)uiManager.GetCurrentScreen();
        screen.ParseFromPhotonNetwork();

        // sync with master client
        if (PhotonNetwork.CurrentRoom.CustomProperties["Level"] != null)
        {
            string level = (string) PhotonNetwork.CurrentRoom.CustomProperties["Level"];
            MapName = level;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        OnDisconnectedFromRoom();
    }

    private void OnDisconnectedFromRoom()
    {
        // reset
        state = GameRoomState.DISCONNECTED;
        MapName = null;
        HunterNickname = null;
        StartTime = 0;
        TimerEnable = false;
        timerDecrementValue = cooldownTimeInSeconds;
    }

    /*
    private void UpdateUI ()
    {
        // update timer
        if (state == GameRoomState.STARTING || state == GameRoomState.WAITING)
        {
            UIManager uiManager = UIManager.Instance;
            MobilePlayMenuScreen screen = (MobilePlayMenuScreen)uiManager.GetCurrentScreen();
            float value = (float) timerDecrementValue / (float) cooldownTimeInSeconds * 100f;
            screen.SetCircleValue(value);
        }
    }
    */

    private void UpdateTimer()
    {
        if (TimerEnable)
        {
            int timerIncrementValue = (int)(PhotonNetwork.Time - StartTime);
            if (IsGameStarting())
            {
                timerDecrementValue = cooldownTimeInSeconds - timerIncrementValue;
            }
            //else if (IsGameRunning())
            // {
            //    timerDecrementValue = roundTime - timerIncrementValue;
            //}

            if (timerDecrementValue == 0)
            {
                TimerEnable = false;
                OnTimerComplete();
            }
        }
    }

    private void UpdateRoomState()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        switch (state)
        {
            case GameRoomState.WAITING:
                if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersForStart)
                {
                    // set state
                    state = GameRoomState.STARTING;

                    // The timer is available only for the client master
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // start timer
                        StartTimer();

                        // randomize
                        RandomizeHunter();

                        // update properties
                        UpdateServerProperties();
                    }
                }
                break;
            case GameRoomState.STARTING:
                if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayersForStart)
                {
                    // set state
                    state = GameRoomState.WAITING;

                    // The timer is available only for the client master
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // stop timer
                        StopTimer();
                    }
                }
                break;
        }
    }

    public void UpdateServerProperties()
    {
        bool flag = IsGameRunning() || timerDecrementValue <= 2;
        Hashtable table = new Hashtable
        {
            { "Level", MapName },
            { "IsGameRunning", flag}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    public int GetTimeCooldown()
    {
        return timerDecrementValue;
    }

    private void StartTimer()
    {
        double startTime = PhotonNetwork.Time;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_SET_TIME, startTime, raiseEventOptions, sendOptions);
    }

    private void StopTimer()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_RESET_TIME, null, raiseEventOptions, sendOptions);
    }

    private void OnTimerComplete()
    {
        if (state == GameRoomState.STARTING)
        {
            // Start load game
            LoadLevel();
        }
        else if (state == GameRoomState.RUNNING)
        {
            // end game
            StopGame();
        }
    }

    public void RegisterListener(IGameRoomListener listener)
    {
        if (!mListeners.Contains(listener))
        {
            mListeners.Add(listener);
        }
    }

    public void UnregisterListener(IGameRoomListener listener)
    {
        if (mListeners.Contains(listener))
        {
            mListeners.Remove(listener);
        }
    }

    public bool IsGameStarting()
    {
        return state == GameRoomState.STARTING;
    }

    public bool IsGameRunning()
    {
        return state == GameRoomState.RUNNING;
    }

    public void LoadLevel()
    {
        state = GameRoomState.RUNNING;

        if (PhotonNetwork.IsMasterClient)
        {
            // update properties
            UpdateServerProperties();

            // stop timer
            StopTimer();

            // spawn player
            //SpawnPlayer();

            // send event to other players
            //double startTime = PhotonNetwork.Time;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_LOAD_LEVEL, null, raiseEventOptions, sendOptions);

            // Start round timer
            //StartTimer();
        }
    }

    public void StopGame()
    {
        state = GameRoomState.ENDING;
    }

    public void RandomizeHunter()
    {
        // generate random id
        int i = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        string hunter = PhotonNetwork.PlayerList[i].NickName;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_SET_HUNTER, hunter, raiseEventOptions, sendOptions);
    }

    // Player events
    public void OnPlayerDeath(EntityPlayer player)
    {
        // call game state listeners
        foreach (IGameRoomListener listener in mListeners)
        {
            listener.OnPlayerDeath(player);
        }
    }

    // Photon
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged != null)
        {
            // set map name
            if (propertiesThatChanged["Level"] != null)
            {
                string mapName = (string)propertiesThatChanged["Level"];
                MapName = mapName;
            }

            // is Running ?
            if (propertiesThatChanged["IsGameRunning"] != null)
            {
                bool started = (bool)propertiesThatChanged["IsGameRunning"];
                if (started)
                {
                    state = GameRoomState.RUNNING;
                }
            }
        }
    }

    private void SyncPlayerData(Player newPlayer)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new[] { newPlayer.ActorNumber } };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        // send hunter
        PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_SET_HUNTER, HunterNickname, raiseEventOptions, sendOptions);

        // send timer data
        PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_SET_TIME, StartTime, raiseEventOptions, sendOptions);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // sync with master client
        if (PhotonNetwork.IsMasterClient)
        {
            // send data for new player
            SyncPlayerData(newPlayer);
        }

        if (state == GameRoomState.WAITING || state == GameRoomState.STARTING)
        {
            UIManager uiManager = UIManager.Instance;
            MobilePlayMenuScreen screen = (MobilePlayMenuScreen) uiManager.GetCurrentScreen();
            screen.AddPlayerToPlayerList(newPlayer.NickName);
        }

        if (state == GameRoomState.STARTING)
        {
            // randomize only for master client
            if (PhotonNetwork.IsMasterClient)
            {
                // restart timer
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                double startTime = PhotonNetwork.Time;
                PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_RESTART_TIME, startTime, raiseEventOptions, sendOptions);

                // randomize again
                RandomizeHunter();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        loadedPlayers--;

        if (state == GameRoomState.WAITING || state == GameRoomState.STARTING)
        {
            UIManager uiManager = UIManager.Instance;
            MobilePlayMenuScreen screen = (MobilePlayMenuScreen)uiManager.GetCurrentScreen();
            screen.RemovePlayerFromPlayerList(otherPlayer.NickName);
        }

        if (state == GameRoomState.STARTING)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersForStart)
            {
                // randomize only for master client
                if (PhotonNetwork.IsMasterClient)
                {
                    // randomize again
                    RandomizeHunter();
                }
            }
        }
        else if (state == GameRoomState.RUNNING)
        {
            if (otherPlayer.NickName.Equals(HunterNickname))
            {
                HunterNickname = "";
            }
            /*
            // Stop game if left player is hunter or playerCount <= 1
            if (otherPlayer.NickName.Equals(HunterNickname) || PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                StopGame();
            }
            */
        }
    }

    // Receiver data
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case EventConstant.EVENT_ID_ROOM_SET_TIME:
                {
                    Debug.Log("EVENT_ID_ROOM_SET_TIME");
                    // set timer
                    double time = (double)photonEvent.CustomData;
                    StartTime = time;
                    Debug.Log("Game Running: " + IsGameRunning());
                    timerDecrementValue = IsGameRunning() ? roundTime : cooldownTimeInSeconds;
                    TimerEnable = true;
                    break;
                }
            case EventConstant.EVENT_ID_ROOM_RESET_TIME:
                {
                    Debug.Log("EVENT_ID_ROOM_RESET_TIME");
                    // stop timer
                    TimerEnable = false;
                    timerDecrementValue = IsGameRunning() ? roundTime : cooldownTimeInSeconds;
                    break;
                }
            case EventConstant.EVENT_ID_ROOM_RESTART_TIME:
                {
                    Debug.Log("EVENT_ID_ROOM_RESTART_TIME");
                    // restart
                    double time = (double)photonEvent.CustomData;
                    StartTime = time;
                    timerDecrementValue = IsGameRunning() ? roundTime : cooldownTimeInSeconds;
                    TimerEnable = true;
                    break;
                }
            case EventConstant.EVENT_ID_ROOM_SET_HUNTER:
                {
                    Debug.Log("EVENT_ID_ROOM_SET_HUNTER");
                    // set hunter
                    HunterNickname = (string)photonEvent.CustomData;
                    break;
                }
            case EventConstant.EVENT_ID_ROOM_LOAD_LEVEL:
                {
                    Debug.Log("EVENT_ID_ROOM_LOAD_LEVEL");
                    state = GameRoomState.RUNNING;

                    // set timer
                    //double time = (double)photonEvent.CustomData;
                    //StartTime = time;
                    //timerDecrementValue = roundTime;
                    //TimerEnable = true;

                    // call game room listeners
                    foreach (IGameRoomListener listener in mListeners)
                    {
                        listener.OnGameRoomStarted();
                    }

                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.LoadLevel(MapName);
                    }
                    break;
                }
            case EventConstant.EVENT_ID_ROOM_LEVEL_LOADED:
                {
                    loadedPlayers++;
                    break;
                }
            default:
                break;
        }
        /*
        if (eventCode == EventConstant.EVENT_ID_GAME_SPAWN_PLAYERS)
        {
            Debug.Log("EVENT_ID_GAME_SPAWN_PLAYERS");
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_SET_TIME)
        {
            Debug.Log("EVENT_ID_ROOM_SET_TIME");
            // set timer
            double time = (double)photonEvent.CustomData;
            StartTime = time;
            Debug.Log("Game Running: " + IsGameRunning());
            timerDecrementValue = IsGameRunning() ? roundTime : cooldownTimeInSeconds;
            TimerEnable = true;
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_RESET_TIME)
        {
            Debug.Log("EVENT_ID_ROOM_RESET_TIME");
            // stop timer
            TimerEnable = false;
            timerDecrementValue = IsGameRunning() ? roundTime : cooldownTimeInSeconds;
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_RESTART_TIME)
        {
            Debug.Log("EVENT_ID_ROOM_RESTART_TIME");
            // restart
            double time = (double)photonEvent.CustomData;
            StartTime = time;
            timerDecrementValue = IsGameRunning() ? roundTime : cooldownTimeInSeconds;
            TimerEnable = true;
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_SET_HUNTER)
        {
            Debug.Log("EVENT_ID_ROOM_SET_HUNTER");
            // set hunter
            HunterNickname = (string)photonEvent.CustomData;
        }
        else if (eventCode == EventConstant.EVENT_ID_GAME_START_GAME)
        {
            Debug.Log("EVENT_ID_GAME_START_GAME");
            state = GameRoomState.RUNNING;

            // set timer
            double time = (double)photonEvent.CustomData;
            StartTime = time;
            //timerDecrementValue = roundTime;
            //cooldownTimeInSeconds = roundTime;
            //TimerEnable = true;

            // call game room listeners
            foreach (IGameRoomListener listener in mListeners)
            {
                listener.OnGameRoomStarted();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(MapName);
            }
        }
        else if (eventCode == EventConstant.EVENT_ID_GAME_STOP_GAME)
        {
            Debug.Log("EVENT_ID_ROOM_STOP_GAME");
            state = GameRoomState.ENDING;

            // stop timer
            TimerEnable = false;
            timerDecrementValue = 0;

            // call game room listeners
            foreach (IGameRoomListener listener in mListeners)
            {
                listener.OnGameRoomStarted();
            }
        }
        */
    }

    
}
