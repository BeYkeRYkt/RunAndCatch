using System.Collections.Generic;
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
        // TODO: Open gameplay UI
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();

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

    // Player events
    public void onPlayerDeath()
    {
        isPlayerDead = true;
        if(cameraObject != null)
        {
            cameraObject.transform.parent.parent = null;
            CameraFollower flwr = cameraObject.transform.parent.GetComponent<CameraFollower>();
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
