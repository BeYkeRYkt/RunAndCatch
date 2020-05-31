using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/**
 * Main game logic for Client
 */
public class ClientGameManager : MonoBehaviour
{
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

    public static ClientGameManager Instance { get; private set; }

    void Awake()
    {
        CreateSingleton();
    }

    protected void CreateSingleton()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
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
        isSessionRunning = true;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameStarted();
        }
    }

    public void StopGame()
    {
        isSessionRunning = false;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameStopped();
        }
    }

    public void PauseGame()
    {
        if (!isSessionRunning)
        {
            return;
        }

        isPaused = true;

        // Open pause menu
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        uiManager.OpenGUI(PCPauseMenuScreen.ID);

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

        // TODO: Open gameplay mobile UI
        uiManager.OpenGUI(PCGameplayScreen.ID);

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameUnpaused();
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameOver();
        }

        // show game over UI
        if (isSessionRunning)
        {
            //UIHome.instance.ShowGameOver();
            // TODO: Open gameover menu
            UIManager uiManager = UIManager.Instance;
            uiManager.OpenGUI(PCPauseMenuScreen.ID);
            //uiManager.OpenGUI();
        }
    }

    public void SpawnEntityPlayer()
    {
        if (playerRole == PlayerRole.HUNTER)
        {
            SpawnEntityPlayer(hunterPrefab);
            return;
        }
        else
        {
            int index = Random.Range(0, playerPrefabs.Count);
            GameObject prefab = playerPrefabs[index];
            SpawnEntityPlayer(prefab);
        }
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
        Vector3 pos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        playerObject = PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.identity);
        //Log("Created new prefab for " + PhotonNetwork.NickName);

        // set nickname
        EntityPlayer player = playerObject.GetComponent<EntityPlayer>();
        player.SetDisplayName(PhotonNetwork.NickName);

        // set camera for prefab
        CharacterMovement mvnt = playerObject.GetComponent<CharacterMovement>();
        mvnt.cameraObject = cameraObject.gameObject;
        //cameraObject.transform.parent = playerObject.transform;
        //cameraObject.transform.parent.transform.localPosition = new Vector3(0f, 5, -4);
        //cameraObject.transform.localPosition = new Vector3(-1.71722f, 12.51777f, 1.094146f);
        //cameraObject.transform.Rotate(54.866f, 114.615f, 0.003f);

        // talk to camera follow game object
        CameraFollower flwr = cameraObject.GetComponent<CameraFollower>();
        flwr.target = playerObject;

        // set hunter/victim role
        string hunterName = GameRoomManager.Instance.HunterNickname;
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
    }

    // Player events
    public void OnPlayerDeath()
    {
        isPlayerDead = true;
        if(cameraObject != null)
        {
            //cameraObject.transform.parent = null;
            CameraFollower flwr = cameraObject.transform.GetComponent<CameraFollower>();
            if (flwr != null)
            {
                flwr.ResetPos();
            }
        }
        GameOver();
    }

    private void Update()
    {
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

}
