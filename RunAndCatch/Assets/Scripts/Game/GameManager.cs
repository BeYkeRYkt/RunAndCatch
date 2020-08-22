
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IOnEventCallback
{
    // Music
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Timer
    public double masterStartTime = 0;
    public bool enableTimer = false;
    private int timerDecrementValue;
    public int roundTime = 60; // default 60 sec

    // DEBUG ONLY
    public bool isOnline;
    public bool isSessionRunning;
    public bool isPaused;
    public bool isPlayerDead;
    public bool isGameOver;

    public GameObject playerObject;
    public GameObject cameraObject;

    public PlayerRole playerRole;

    public GameObject hunterPrefab;
    public List<GameObject> playerPrefabs = new List<GameObject>();

    private List<IGameStateListener> mListeners = new List<IGameStateListener>();

    // spawn points for players
    public List<GameObject> hunterSpawnPoints = new List<GameObject>();
    public List<GameObject> victimsSpawnPoints = new List<GameObject>();

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        CreateSingleton();
    }

    protected void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLogic();

        UpdateTimer();

        // handle key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    private void UpdateLogic()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!PhotonNetwork.InRoom || RoomManager.Instance.state != GameRoomState.RUNNING) return;
            if (RoomManager.Instance.state == GameRoomState.RUNNING)
            {
                if (isSessionRunning)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount < RoomManager.Instance.minPlayersForStart)
                    {
                        // Stop game if left player is hunter or playerCount <= 1
                        StopGame(EndReason.FEW_PLAYERS);
                        return;
                    }
                    else
                    {
                        if (RoomManager.Instance.HunterNickname.Equals(""))
                        {
                            // Stop game if hunter is leaved
                            StopGame(EndReason.HUNTER_LEAVED);
                            return;
                        }
                    }
                }
                else
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
                    {
                        // wait another players
                        if (RoomManager.Instance.loadedPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
                        {
                            StartGame();
                            SpawnPlayers();
                        }
                    }
                }
            }
        }
    }

    private void UpdateTimer()
    {
        if (enableTimer)
        {
            int timerIncrementValue = (int)(PhotonNetwork.Time - masterStartTime);
            timerDecrementValue = roundTime - timerIncrementValue;

            if (timerDecrementValue == 0)
            {
                enableTimer = false;
                OnTimerComplete();
                Debug.Log("OnTimerComplete");
            }
        }
    }

    public int GetTimeCooldown()
    {
        return timerDecrementValue;
    }

    private bool IsHunterWin()
    {
        EntityManager entityManager = EntityManager.Instance;
        int alive = 0;
        for(int i = 0; i < entityManager.GetPlayerEntities().Count; i++)
        {
            EntityPlayer player = entityManager.GetPlayerEntities()[i];
            if(player.playerRole == PlayerRole.VICTIM)
            {
                alive++;
            }
        }
        return alive == 1;
    }

    private void OnTimerComplete()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // send event to other players
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_GAME_STOP_GAME, EndReason.VICTIMS_WIN, raiseEventOptions, sendOptions);
        }
    }

    private void SpawnPlayers()
    {
        if (!isSessionRunning) return;
        if (PhotonNetwork.IsMasterClient)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_GAME_SPAWN_PLAYERS, null, raiseEventOptions, sendOptions);
        }
    }

    public void RegisterListener(IGameStateListener listener)
    {
        if (!mListeners.Contains(listener))
        {
            mListeners.Add(listener);
        }
    }

    public void UnregisterListener(IGameStateListener listener)
    {
        if (mListeners.Contains(listener))
        {
            mListeners.Remove(listener);
        }
    }

    public void StartGame()
    {
        // send messages
        if (PhotonNetwork.IsMasterClient)
        {
            isSessionRunning = true;
            double startTime = PhotonNetwork.Time;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_GAME_START_GAME, startTime, raiseEventOptions, sendOptions);
        }
    }

    public void OnStartGame()
    {
        isSessionRunning = true;

        // update player role
        RoomManager manager = RoomManager.Instance;
        PlayerRole role = PlayerRole.VICTIM;
        if (PhotonNetwork.NickName.Equals(manager.HunterNickname))
        {
            role = PlayerRole.HUNTER;
        }
        playerRole = role;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameStarted();
        }

        // start music
        audioSource.loop = true;
        audioSource.volume = 0.1f;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void StopGame(EndReason reason)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //isSessionRunning = false;
            OnStopGame(reason);

            // send event to other players
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(EventConstant.EVENT_ID_GAME_STOP_GAME, reason, raiseEventOptions, sendOptions);
        }
    }

    public void OnStopGame(EndReason reason)
    {
        Debug.Log(reason);
        isSessionRunning = false;

        RoomManager.Instance.StopGame();

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameStopped();
        }

        // TODO: Show result screen
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        MobileGameoverScreen screen = (MobileGameoverScreen) uiManager.GetScreenById(MobileGameoverScreen.ID);
        screen.UpdateUI(reason);
        uiManager.OpenGUI(MobileGameoverScreen.ID);

        /*
        switch(reason)
        {
            case EndReason.HUNTER_WIN:
                break;
            case EndReason.VICTIMS_WIN:
                break;
            case EndReason.FEW_PLAYERS:
            default:
                PhotonNetwork.LeaveRoom();
                break;
            case EndReason.HUNTER_LEAVED:
                break;
        }
        */

        // stop music
        StartCoroutine(AudioFadeOut.FadeOut(audioSource, 2.0f));

        //audioSource.loop = false;
        //audioSource.Stop();
    }

    public void PauseGame()
    {
        if (isPlayerDead)
        {
            return;
        }

        isPaused = true;

        // Open pause menu
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        uiManager.OpenGUI(MobilePauseMenuScreen.ID);

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGamePaused();
        }
    }

    public void UnpauseGame()
    {
        isPaused = false;

        // Close pause menu
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        uiManager.OpenGUI(MobileGameplayScreen.ID);

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameUnpaused();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        isPaused = true;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameOver();
        }

        // show game over UI
        if (isSessionRunning)
        {
            // TODO: Open gameover menu
            if(EntityManager.Instance.GetPlayerEntities().Count > 2)
            {
                UIManager uiManager = UIManager.Instance;
                uiManager.CloseGUI();
                uiManager.OpenGUI(MobileGameoverScreen.ID);
            }
        }

        if (cameraObject != null)
        {
            CameraFollower flwr = cameraObject.transform.GetComponent<CameraFollower>();
            EntityPlayer player = (EntityPlayer) EntityManager.Instance.GetEntityByName(RoomManager.Instance.HunterNickname);
            if (flwr != null)
            {
                flwr.target = player.gameObject;
            }
        }
    }

    public void SpawnEntityPlayer()
    {
        if (playerRole == PlayerRole.HUNTER)
        {
            SpawnEntityPlayer(hunterPrefab);
        }
        else
        {
            int index = Random.Range(0, playerPrefabs.Count);
            GameObject prefab = playerPrefabs[index];
            SpawnEntityPlayer(prefab);
        }

        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        uiManager.OpenGUI(MobileGameplayScreen.ID);
    }

    private void SpawnEntityPlayer(GameObject prefab)
    {
        // kill another player
        if (playerObject != null)
        {
            EntityPlayer ep = playerObject.GetComponent<EntityPlayer>();
            ep.KillEntity();
        }

        // spawn player
        // take random spawnpoint
        GameObject spawnPoint = null;
        if (playerRole == PlayerRole.HUNTER)
        {
            int randomId = Random.Range(0, hunterSpawnPoints.Capacity);
            spawnPoint = hunterSpawnPoints[randomId];
        }
        else
        {
            int randomId = Random.Range(0, victimsSpawnPoints.Capacity);
            spawnPoint = victimsSpawnPoints[randomId];
        }

        Vector3 pos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        if (spawnPoint != null)
        {
            pos = spawnPoint.transform.position;
        }

        playerObject = PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.identity);

        // set nickname
        EntityPlayer player = playerObject.GetComponent<EntityPlayer>();
        player.SetDisplayName(PhotonNetwork.NickName);

        // set camera for prefab
        CharacterMovement mvnt = playerObject.GetComponent<CharacterMovement>();
        mvnt.cameraObject = cameraObject.gameObject;

        // talk to camera follow game object
        CameraFollower flwr = cameraObject.GetComponent<CameraFollower>();
        flwr.target = playerObject;

        // set hunter/victim role
        string hunterName = RoomManager.Instance.HunterNickname;
        if (player.GetDisplayName().Equals(hunterName))
        {
            player.SetPlayerRole(PlayerRole.HUNTER);
            playerRole = PlayerRole.HUNTER;
        }
        else
        {
            player.SetPlayerRole(PlayerRole.VICTIM);
            playerRole = PlayerRole.VICTIM;
        }

        // unset player dead or game over
        isPlayerDead = false;
        isGameOver = false;
    }

    // Player events
    private void CheckHunterIsWin()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (isSessionRunning)
            {
                if (IsHunterWin())
                {
                    // Stop game if hunter win
                    StopGame(EndReason.HUNTER_WIN);
                    return;
                }
            }
        }
    }

    public void OnRemotePlayerDeath()
    {
        CheckHunterIsWin();
    }

    public void OnLocalPlayerDeath()
    {
        isPlayerDead = true;
        if (cameraObject != null)
        {
            //cameraObject.transform.parent = null;
            CameraFollower flwr = cameraObject.transform.GetComponent<CameraFollower>();
            if (flwr != null)
            {
                flwr.ResetPos();
            }
        }
        GameOver();
        CheckHunterIsWin();
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case EventConstant.EVENT_ID_GAME_SPAWN_PLAYERS:
                {
                    Debug.Log("EVENT_ID_GAME_SPAWN_PLAYERS");
                    SpawnEntityPlayer();
                    break;
                }
            case EventConstant.EVENT_ID_GAME_START_GAME:
                {
                    Debug.Log("EVENT_ID_GAME_START_GAME");

                    // set timer
                    double time = (double)photonEvent.CustomData;
                    masterStartTime = time;
                    roundTime = RoomManager.Instance.GetTimeCooldown();
                    timerDecrementValue = roundTime;
                    //cooldownTimeInSeconds = roundTime;
                    enableTimer = true;
                    OnStartGame();
                    break;
                }
            case EventConstant.EVENT_ID_GAME_STOP_GAME:
                {
                    Debug.Log("EVENT_ID_GAME_STOP_GAME");
                    enableTimer = false;
                    EndReason reason = (EndReason) photonEvent.CustomData;
                    OnStopGame(reason);
                    break;
                }
            default:
                break;
        }
    }
}
