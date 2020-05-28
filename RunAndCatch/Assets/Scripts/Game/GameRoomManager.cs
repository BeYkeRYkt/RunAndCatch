using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/**
 * Class for GameRoom
 */
public class GameRoomManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // Server properties
    public string MapName;
    public string HunterNickname;
    public double StartTime;
    public bool TimerEnable = false;

    // minimum players for start game
    public int minPlayersForStart = 1;

    // Game room state
    public GameRoomState state = GameRoomState.WAITING;

    // Timer
    private double timerDecrementValue;
    public float cooldownTimeInSeconds = 20; // in secs

    private List<IGameRoomListener> mListeners = new List<IGameRoomListener>();

    public static GameRoomManager Instance { get; private set; }

    void Awake()
    {
        CreateSingleton();
        timerDecrementValue = cooldownTimeInSeconds;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    protected void CreateSingleton()
    {
        Instance = this;
    }

    void Update()
    {
        // Update cooldown timer
        if (TimerEnable)
        {
            double timerIncrementValue = PhotonNetwork.Time - StartTime;
            timerDecrementValue = cooldownTimeInSeconds - timerIncrementValue;

            if (timerDecrementValue <= 0)
            {
                //Timer Completed
                OnTimerComplete();
            }
        }

        if (state != GameRoomState.RUNNING)
        {
            UpdateRoomState();
        }
    }

    public void UpdateRoomState()
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
                if (TimerEnable)
                {
                    // TODO: update UI ?
                }
                break;
            case GameRoomState.RUNNING:
                // TODO: update UI ?
                break;
        }
    }

    public int GetTimeCooldown()
    {
        return (int) timerDecrementValue;
    }

    public void StartTimer()
    {
        if (!TimerEnable)
        {
            double startTime = PhotonNetwork.Time;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_TIMER_START, startTime, raiseEventOptions, sendOptions);
        }
    }

    public void StopTimer()
    {
        if (TimerEnable)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_TIMER_STOP, null, raiseEventOptions, sendOptions);
        }
    }

    public void OnTimerComplete()
    {
        if(state == GameRoomState.STARTING)
        {
            // Start game
            StartGame();
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

    public void StartGame()
    {
        state = GameRoomState.RUNNING;

        if (PhotonNetwork.IsMasterClient)
        {
            // stop timer
            StopTimer();

            // spawn player
            SpawnPlayer();
        }

        // call game room listeners
        foreach (IGameRoomListener listener in mListeners)
        {
            listener.OnGameRoomStarted();
        }
    }

    public void StopGame()
    {
        state = GameRoomState.ENDING;

        // call game room listeners
        foreach (IGameRoomListener listener in mListeners)
        {
            listener.OnGameRoomStarted();
        }
    }

    public void SpawnPlayer()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_SPAWN, null, raiseEventOptions, sendOptions);
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
            if (propertiesThatChanged["MapName"] != null)
            {
                string mapName = (string)propertiesThatChanged["MapName"];
                MapName = mapName;
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
        if (TimerEnable)
        {
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_TIMER_START, StartTime, raiseEventOptions, sendOptions);
        }
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

        if(state == GameRoomState.STARTING)
        {
            // randomize only for master client
            if (PhotonNetwork.IsMasterClient)
            {
                // restart timer
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                double startTime = PhotonNetwork.Time;
                PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_ROOM_TIMER_RESTART, startTime, raiseEventOptions, sendOptions);

                // randomize again
                RandomizeHunter();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if(state == GameRoomState.STARTING)
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
    }

    // Receiver data
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventConstant.EVENT_ID_ROOM_SPAWN)
        {
            // spawn player
            ClientGameManager clientGameManager = ClientGameManager.Instance;
            clientGameManager.SpawnEntityPlayer();
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_TIMER_START)
        {
            // set timer
            double time = (double)photonEvent.CustomData;
            StartTime = time;
            TimerEnable = true;
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_TIMER_STOP)
        {
            // stop timer
            TimerEnable = false;
            timerDecrementValue = cooldownTimeInSeconds;
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_TIMER_RESTART)
        {
            // restart
            double time = (double)photonEvent.CustomData;
            StartTime = time;
            timerDecrementValue = cooldownTimeInSeconds;
            TimerEnable = true;
        }
        else if (eventCode == EventConstant.EVENT_ID_ROOM_SET_HUNTER)
        {
            // set hunter
            HunterNickname = (string)photonEvent.CustomData;
        }
    }
}
